using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBDAL
{
    public class SegmentDetailsDAL
    {
        OBEntities db = new OBEntities();

        public IQueryable<SegmentDetail> GetSegmentDetails()
        {
            return db.SegmentDetails.Select(x => x);
        }

        public List<SegmentDetail> GetSegmentDetails(int SegmentID)
        {
            return db.SegmentDetails.Where(x => x.SegmentID == SegmentID).Select(x => x).ToList();
        }

        public bool IsBudgetEditable(List<int> SegmentDetailIDs)
        {
            List<int> parentids = db.SegmentDetails.Select(x => x.ParentDetailID.HasValue ? x.ParentDetailID.Value : int.MinValue).ToList();

            return parentids.Where(x => SegmentDetailIDs.Contains(x)).Count() == 0;
        }

        public bool InsertSegmentDetail(SegmentDetail objSegmentDetail)
        {
            try
            {
                db.SegmentDetails.Add(objSegmentDetail);
                db.SaveChanges();

                BPEventLog bpe = new BPEventLog();
                bpe.Object = "Segment";
                bpe.ObjectName = objSegmentDetail.DetailCode;
                bpe.ObjectChanges = string.Empty;
                bpe.EventMassage = "Success";
                bpe.Status = "A";
                bpe.CreatedBy = objSegmentDetail.CreatedBy;
                bpe.CreatedTimeStamp = objSegmentDetail.CreatedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                return true;
            }
            catch (Exception ex)
            {
                BPEventLog bpe = new BPEventLog();
                bpe.Object = "SegmentDetail";
                bpe.ObjectName = objSegmentDetail.DetailCode;
                bpe.ObjectChanges = string.Empty;
                bpe.EventMassage = "Failure";
                bpe.Status = "A";
                bpe.CreatedBy = objSegmentDetail.CreatedBy;
                bpe.CreatedTimeStamp = objSegmentDetail.CreatedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                throw ex;
            }
        }

        public bool UpdateSegmentDetail(SegmentDetail objSegmentDetail)
        {
            SegmentDetail obj = db.SegmentDetails.Where(x => x.SegmentDetailID == objSegmentDetail.SegmentDetailID).FirstOrDefault();
            string changes = new EventLogDAL().ObjectDifference(obj, objSegmentDetail);
            try
            {
                if (obj != null)
                {
                    obj.SegmentID = objSegmentDetail.SegmentID;
                    obj.DetailCode = objSegmentDetail.DetailCode;
                    obj.DetailDesc = objSegmentDetail.DetailDesc;
                    obj.ParentDetailID = objSegmentDetail.ParentDetailID;
                    obj.Status = objSegmentDetail.Status;
                    obj.ModifiedBy = objSegmentDetail.ModifiedBy;
                    obj.ModifiedTimeStamp = objSegmentDetail.ModifiedTimeStamp;
                    db.SaveChanges();

                    BPEventLog bpe = new BPEventLog();
                    bpe.Object = "Segment";
                    bpe.ObjectName = objSegmentDetail.DetailCode;
                    bpe.ObjectChanges = changes;
                    bpe.EventMassage = "Success";
                    bpe.Status = "A";
                    bpe.CreatedBy = objSegmentDetail.ModifiedBy;
                    bpe.CreatedTimeStamp = objSegmentDetail.ModifiedTimeStamp;
                    new EventLogDAL().AddEventLog(bpe);
                }
                return true;
            }
            catch (Exception ex)
            {
                BPEventLog bpe = new BPEventLog();
                bpe.Object = "Segment";
                bpe.ObjectName = objSegmentDetail.DetailCode;
                bpe.ObjectChanges = changes;
                bpe.EventMassage = "Success";
                bpe.Status = "A";
                bpe.CreatedBy = objSegmentDetail.ModifiedBy;
                bpe.CreatedTimeStamp = objSegmentDetail.ModifiedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                throw ex;
            }
        }
                
        public List<int> AllLeafDetails(int SegDetailID)
        {
            List<SegmentDetail> lstdetl = db.SegmentDetails.ToList();
            List<int> parents = new List<int>() { SegDetailID };
            List<int> childs = new List<int>();
            List<int> result = new List<int>();

            while (parents.Count > 0)
            {
                childs.Clear();
                foreach (int p in parents)
                {
                    if (lstdetl.Where(x => x.ParentDetailID == p && x.Status=="A").Count() > 0)
                    {
                        foreach (int x in lstdetl.Where(x => x.ParentDetailID == p).Select(x => x.SegmentDetailID))
                        {
                            childs.Add(x);
                        }
                    }
                    else
                    {
                        result.Add(p);
                    }
                }
                parents.Clear();
                foreach (int x in childs)
                {
                    parents.Add(x);
                }
            } 

            return result;
        }

        public List<int> AllLeafDetails(Dictionary<int, int> SegmentAndDetailPair)
        {
            List<SegmentDetail> lstdetl = db.SegmentDetails.Where(x=>x.Status=="A").ToList();

            List<int> segDtlIDs = new List<int>();

            foreach (KeyValuePair<int, int> pair in SegmentAndDetailPair)
            {
                List<int> result = new List<int>();
                if (pair.Value == 0)
                {
                    result = Logic(lstdetl, lstdetl.Where(x => x.SegmentID == pair.Key && x.ParentDetailID == 0).Select(x => x.SegmentDetailID).ToList());
                }
                else
                    result = Logic(lstdetl, new List<int>() { pair.Value });

                foreach (int x in result)
                {
                    segDtlIDs.Add(x);
                }
            }

            return segDtlIDs;
        }

        private List<int> Logic(List<SegmentDetail> lstdetl, List<int> parents)
        {
            //List<int> parents = lstdetl.Where(x => x.SegmentID == pair.Key && x.ParentDetailID == 0).Select(x => x.SegmentDetailID).ToList();
            List<int> childs = new List<int>();
            List<int> result = new List<int>();

            while (parents.Count > 0)
            {
                childs.Clear();
                foreach (int p in parents)
                {
                    if (lstdetl.Where(x => x.ParentDetailID == p && x.Status == "A").Count() > 0)
                    {
                        foreach (int x in lstdetl.Where(x => x.ParentDetailID == p).Select(x => x.SegmentDetailID))
                        {
                            childs.Add(x);
                        }
                    }
                    else
                    {
                        result.Add(p);
                    }
                }
                parents.Clear();
                foreach (int x in childs)
                {
                    parents.Add(x);
                }
            }
            return result;
        }
    }
}
