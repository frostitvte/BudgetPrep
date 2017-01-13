using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBDAL
{
    public class BudgetPerjawatanYearEnd
    {
        public List<string> ListSegmentDetails { get; set; }
        public string Prefix { get; set; }
        public string GroupPerjawatanCode { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string PeriodPerjawatan { get; set; }
    }

    public class BudgetPerjawatanDAL
    {
        OBEntities db = new OBEntities();

        public IQueryable<BudgetPerjawatan> GetBudgetPerjawatans()
        {
            return db.BudgetPerjawatans;
        }

        public IQueryable<JuncBgtPerjawatanSegDtl> GetPerjawatanSegDtls()
        {
            return db.JuncBgtPerjawatanSegDtls;
        }

        public List<BudgetPerjawatan> GetBudgetPerjawatans(List<int> LstSegmentDetailIDs)
        {
            List<JuncBgtPerjawatanSegDtl> data = db.JuncBgtPerjawatanSegDtls.Where(x => LstSegmentDetailIDs.Contains(x.SegmentDetailID)).Select(x => x).ToList();
            foreach (int i in LstSegmentDetailIDs)
            {
                List<int> data1 = data.Where(y => y.SegmentDetailID == i).Select(y => y.BudgetPerjawatanID).ToList();
                data = data.Where(x => data1.Contains(x.BudgetPerjawatanID)).ToList();
            }

            List<int> budgetids = data.Select(y => y.BudgetPerjawatanID).Distinct().ToList();
            return db.BudgetPerjawatans.Where(x => budgetids.Contains(x.BudgetPerjawatanID)).Select(x => x).ToList();
        }

        public List<BudgetPerjawatan> GetBudgetPerjawatansWithTreeCalc(List<int> LstSegmentDetailIDs, ref bool CanEdit)
        {
            List<BudgetPerjawatan> BudgetData = new List<BudgetPerjawatan>();
            List<List<int>> lstIDs = new List<List<int>>();
            List<List<int>> resultcombi = new List<List<int>>();

            if (LstSegmentDetailIDs.Count > 0)
            {
                foreach (int id in LstSegmentDetailIDs)
                    lstIDs.Add(new SegmentDetailsDAL().AllLeafDetails(id));

                List<int> index = new List<int>();
                List<int> count = new List<int>();
                List<int> result = new List<int>();

                for (int i = 0; i < lstIDs.Count; i++)
                {
                    index.Add(0);
                    count.Add(lstIDs[i].Count);
                }

                while (index.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y) != count.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y))
                {
                    List<int> sdids = new List<int>();
                    for (int id = 0; id < index.Count; id++)
                        sdids.Add(lstIDs[id][index[id]]);

                    foreach (BudgetPerjawatan bm in GetBudgetPerjawatans(sdids))
                        BudgetData.Add(bm);

                    int i = index.Count - 1;
                    while (i >= 0 && index[i] < count[i])
                    {
                        index[i] = index[i] + 1;
                        i--;
                    }

                    if (index.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y) == count.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y))
                        continue;

                    for (int j = 0; j < index.Count; j++)
                    {
                        if (index[j] == count[j])
                        {
                            index[j] = 0;
                        }
                    }
                }
            }

            string s1 = lstIDs.Select(x => (x.Count) > 0 ? x[0] : 0).OrderBy(x => x).Select(x => x.ToString()).Aggregate((x, y) => x + y);
            string s2 = LstSegmentDetailIDs.OrderBy(x => x).Select(x => x.ToString()).Aggregate((x, y) => x + y);

            if (lstIDs.Where(x => x.Count > 1).Count() == 0)
            {
                if (s1 == s2)
                {
                    CanEdit = true;
                    return BudgetData;
                }
            }

            CanEdit = false;
            return BudgetData
                .GroupBy(x => new
                {
                    x.GroupPerjawatanCode,
                    x.PeriodPerjawatanID,
                    x.Status
                })
                .Select(x => new BudgetPerjawatan()
                {
                    BudgetPerjawatanID = x.Count(),
                    PeriodPerjawatanID = x.Key.PeriodPerjawatanID,
                    GroupPerjawatanCode = x.Key.GroupPerjawatanCode,
                    Amount = x.Sum(y => y.Amount),
                    Status = x.Key.Status,
                    Remarks = string.Empty
                }).ToList();
        }

        public List<BudgetPerjawatan> GetBudgetPerjawatansStatus(Dictionary<int, int> SegmentAndDetailPair, ref bool CanEdit)
        {
            List<BudgetPerjawatan> BudgetData = new List<BudgetPerjawatan>();

            List<int> segDtlIDs = new SegmentDetailsDAL().AllLeafDetails(SegmentAndDetailPair);
            List<int> budID = db.JuncBgtPerjawatanSegDtls.Where(x => segDtlIDs.Contains(x.SegmentDetailID))
                .GroupBy(x => new { x.BudgetPerjawatanID })
                //.Select(x => new { BudgetMengurusID=x.Key.BudgetMengurusID, Count = x.Count() })
                .Where(x => x.Count() == SegmentAndDetailPair.Count)
                .Select(x => x.Key.BudgetPerjawatanID).Distinct().ToList();

            BudgetData = db.BudgetPerjawatans.Where(x => budID.Contains(x.BudgetPerjawatanID)).Select(x => x).ToList();

            //List<List<int>> lstIDs = new List<List<int>>();
            //CanEdit = true;

            ////if (SegmentAndDetailPair.Count != 0)
            ////{
            ////foreach (int segid in db.Segments.Where(x => x.Status == "A").OrderBy(x => x.SegmentOrder).Select(x => x.SegmentID))
            ////    lstIDs.Add(db.SegmentDetails.Where(x => x.ParentDetailID == 0 && x.Status == "A" && x.SegmentID == segid).Select(x => x.SegmentDetailID).ToList());

            //foreach (KeyValuePair<int, int> pair in SegmentAndDetailPair)
            //{
            //    if (pair.Value == 0)
            //        lstIDs.Add(db.SegmentDetails.Where(x => x.ParentDetailID == 0 && x.Status == "A" && x.SegmentID == pair.Key).Select(x => x.SegmentDetailID).ToList());
            //    else
            //        lstIDs.Add(new List<int>() { pair.Value });
            //}

            //List<int> index = new List<int>();
            //List<int> count = new List<int>();
            //List<int> result = new List<int>();

            //for (int i = 0; i < lstIDs.Count; i++)
            //{
            //    index.Add(0);
            //    count.Add(lstIDs[i].Count);
            //}

            //while (index.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y) != count.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y))
            //{
            //    List<int> sdids = new List<int>();
            //    for (int id = 0; id < index.Count; id++)
            //        sdids.Add(lstIDs[id][index[id]]);

            //    bool flag = true;
            //    foreach (BudgetPerjawatan bm in GetBudgetPerjawatansWithTreeCalc(sdids, ref flag))
            //        BudgetData.Add(bm);

            //    CanEdit = CanEdit && flag;

            //    int i = index.Count - 1;
            //    while (i >= 0 && index[i] < count[i])
            //    {
            //        index[i] = index[i] + 1;
            //        i--;
            //    }

            //    if (index.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y) == count.Select(x => x.ToString()).Aggregate((x, y) => x + "," + y))
            //        continue;

            //    for (int j = 0; j < index.Count; j++)
            //    {
            //        if (index[j] == count[j])
            //        {
            //            index[j] = 0;
            //        }
            //    }
            //}
            ////}

            //if (lstIDs.Where(x => x.Count > 1).Count() > 0)
            //{
            //    CanEdit = false;
            //}

            return BudgetData
                .GroupBy(x => new
                {
                    x.GroupPerjawatanCode,
                    x.PeriodPerjawatanID,
                    x.Status
                })
                .Select(x => new BudgetPerjawatan()
                {
                    BudgetPerjawatanID = x.Count(),
                    PeriodPerjawatanID = x.Key.PeriodPerjawatanID,
                    GroupPerjawatanCode = x.Key.GroupPerjawatanCode,
                    Amount = x.Sum(y => y.Amount),
                    Status = x.Key.Status,
                    Remarks = string.Empty
                }).ToList();
        }

        public List<BudgetPerjawatanYearEnd> BudgetPerjawatanYearEnd(int Year)
        {
            return db.BudgetPerjawatans.Where(x => x.Status == "A" && x.PeriodPerjawatan.PerjawatanYear == Year).ToList()
                .Select(x => new BudgetPerjawatanYearEnd()
                {
                    ListSegmentDetails = x.JuncBgtPerjawatanSegDtls.OrderBy(y => y.SegmentDetail.Segment.SegmentOrder)
                                        .Select(y => y.SegmentDetail.DetailCode + "-" + y.SegmentDetail.DetailDesc).ToList(),
                    Prefix = x.JuncBgtPerjawatanSegDtls.OrderBy(y => y.SegmentDetail.Segment.SegmentOrder)
                                        .Select(y => y.SegmentDetail.DetailCode).Aggregate((a, b) => a + "-" + b),
                    GroupPerjawatanCode = x.GroupPerjawatanCode,
                    Description = x.GroupPerjawatan.GroupPerjawatanDesc,
                    Amount = x.Amount,
                    PeriodPerjawatan = x.PeriodPerjawatan.PerjawatanYear + "-" + x.PeriodPerjawatan.FieldPerjawatan.FieldPerjawatanDesc
                }).ToList();
        }

        public List<BudgetPerjawatan> GetBudgetPerjawatansForDashboard()
        {
            return db.BudgetPerjawatans.Where(x => x.Status == "A").ToList();
        }

        public bool InsertBudgetPerjawatan(BudgetPerjawatan objBudgetPerjawatan, List<JuncBgtPerjawatanSegDtl> lstBgtPerjawatanSegDtl)
        {
            PeriodPerjawatan per = db.PeriodPerjawatans.Where(x => x.PeriodPerjawatanID == objBudgetPerjawatan.PeriodPerjawatanID).FirstOrDefault();
            try
            {
                db.BudgetPerjawatans.Add(objBudgetPerjawatan);
                foreach (JuncBgtPerjawatanSegDtl obj in lstBgtPerjawatanSegDtl)
                {
                    JuncBgtPerjawatanSegDtl item = new JuncBgtPerjawatanSegDtl();
                    item.BudgetPerjawatan = objBudgetPerjawatan;
                    item.SegmentDetailID = obj.SegmentDetailID;
                    item.CreatedBy = objBudgetPerjawatan.CreatedBy;
                    item.CreatedTimeStamp = DateTime.Now;
                    item.ModifiedBy = objBudgetPerjawatan.CreatedBy;
                    item.ModifiedTimeStamp = DateTime.Now;

                    db.JuncBgtPerjawatanSegDtls.Add(item);
                }
                db.SaveChanges();

                BPEventLog bpe = new BPEventLog();
                bpe.Object = "Budget Perjawatan-" + per.PerjawatanYear + "-" + per.FieldPerjawatan.FieldPerjawatanDesc;
                bpe.ObjectName = GetGroupPerjawatanCodePrefix(lstBgtPerjawatanSegDtl) + "-" + objBudgetPerjawatan.GroupPerjawatanCode;
                bpe.ObjectChanges = "<tr><td>Status</td><td>O</td><td>P</td></tr> <tr><td>Amount</td><td>0.00</td><td>" + objBudgetPerjawatan.Amount.ToString("F") + "</td></tr>";
                bpe.EventMassage = "Success";
                bpe.Status = "I";
                bpe.CreatedBy = objBudgetPerjawatan.CreatedBy;
                bpe.CreatedTimeStamp = objBudgetPerjawatan.CreatedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);
                return true;
            }
            catch (Exception ex)
            {
                BPEventLog bpe = new BPEventLog();
                bpe.Object = "Budget Perjawatan-" + per.PerjawatanYear + "-" + per.FieldPerjawatan.FieldPerjawatanDesc;
                bpe.ObjectName = GetGroupPerjawatanCodePrefix(lstBgtPerjawatanSegDtl) + "-" + objBudgetPerjawatan.GroupPerjawatanCode;
                bpe.ObjectChanges = "<tr><td>Status</td><td>O</td><td>P</td></tr> <tr><td>Amount</td><td>0.00</td><td>" + objBudgetPerjawatan.Amount.ToString("F") + "</td></tr>";
                bpe.EventMassage = "Failure";
                bpe.Status = "I";
                bpe.CreatedBy = objBudgetPerjawatan.CreatedBy;
                bpe.CreatedTimeStamp = objBudgetPerjawatan.CreatedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                throw ex;
            }
        }

        public bool UpdateBudgetPerjawatan(BudgetPerjawatan objBudgetPerjawatan, List<int> LstSegmentDetailIDs)
        {
            BudgetPerjawatan obj = GetBudgetPerjawatans(LstSegmentDetailIDs)
                .Where(x => x.PeriodPerjawatanID == objBudgetPerjawatan.PeriodPerjawatanID && x.GroupPerjawatanCode == objBudgetPerjawatan.GroupPerjawatanCode).FirstOrDefault();
            string changes = new EventLogDAL().ObjectDifference(obj, objBudgetPerjawatan);
            try
            {
                obj.Amount = objBudgetPerjawatan.Amount;
                obj.Status = objBudgetPerjawatan.Status;
                obj.Remarks = objBudgetPerjawatan.Remarks;
                obj.ModifiedBy = objBudgetPerjawatan.ModifiedBy;
                obj.ModifiedTimeStamp = objBudgetPerjawatan.ModifiedTimeStamp;
                db.SaveChanges();

                BPEventLog bpe = new BPEventLog();
                bpe.Object = "Budget Perjawatan-" + obj.PeriodPerjawatan.PerjawatanYear + "-" + obj.PeriodPerjawatan.FieldPerjawatan.FieldPerjawatanDesc;
                bpe.ObjectName = GetGroupPerjawatanCodePrefix(LstSegmentDetailIDs) + "-" + objBudgetPerjawatan.GroupPerjawatanCode;
                bpe.ObjectChanges = changes;
                bpe.EventMassage = "Success";
                bpe.Status = "I";
                bpe.CreatedBy = objBudgetPerjawatan.ModifiedBy;
                bpe.CreatedTimeStamp = objBudgetPerjawatan.ModifiedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                return true;
            }
            catch (Exception ex)
            {
                BPEventLog bpe = new BPEventLog();
                bpe.Object = "Budget Perjawatan-" + obj.PeriodPerjawatan.PerjawatanYear + "-" + obj.PeriodPerjawatan.FieldPerjawatan.FieldPerjawatanDesc;
                bpe.ObjectName = GetGroupPerjawatanCodePrefix(LstSegmentDetailIDs) + "-" + objBudgetPerjawatan.GroupPerjawatanCode;
                bpe.ObjectChanges = changes;
                bpe.EventMassage = "Failure";
                bpe.Status = "I";
                bpe.CreatedBy = objBudgetPerjawatan.ModifiedBy;
                bpe.CreatedTimeStamp = objBudgetPerjawatan.ModifiedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                throw ex;
            }
        }

        public bool UpdateMultipleBudgetPerjawatans(List<int> LstSegmentDetailIDs, List<int> LstPeriodPerjawatanIDs, string FromStatus, string ToStatus, string Remarks, MasterUser User)
        {
            DateTime datestamp = DateTime.Now;
            List<BudgetPerjawatan> data = GetBudgetPerjawatans(LstSegmentDetailIDs).Where(x => LstPeriodPerjawatanIDs.Contains(x.PeriodPerjawatanID) && x.Status == FromStatus).ToList();
            
            if (data.Count > 0)
            {
                try
                {
                    foreach (BudgetPerjawatan obj in data)
                    {
                        string changes = string.Empty;
                        if (FromStatus != ToStatus)
                            changes = changes + "<tr><td>Status</td><td>" + FromStatus.Trim() + "</td><td>" + ToStatus.Trim() + "</td></tr>";
                        if (obj.Remarks != Remarks)
                            changes = changes + "<tr><td>Remarks</td><td>" + ((obj.Remarks == null) ? string.Empty : obj.Remarks.Trim()) + "</td><td>" + Remarks.Trim() + "</td></tr>";

                        obj.Status = ToStatus;
                        obj.Remarks = Remarks;
                        obj.ModifiedBy = User.UserID;
                        obj.ModifiedTimeStamp = datestamp;

                        BPEventLog bpe = new BPEventLog();
                        bpe.Object = "Budget Perjawatan-" + obj.PeriodPerjawatan.PerjawatanYear + "-" + obj.PeriodPerjawatan.FieldPerjawatan.FieldPerjawatanDesc;
                        bpe.ObjectName = GetGroupPerjawatanCodePrefix(LstSegmentDetailIDs) + "-" + obj.GroupPerjawatanCode;
                        //bpe.ObjectChanges = "Batch status Changed From '" + FromStatus + "' To '" + ToStatus + "', IDs : "
                        //    + data.Select(x => x.BudgetPerjawatanID.ToString()).Aggregate((x, y) => x + "," + y);
                        bpe.ObjectChanges = changes;
                        bpe.EventMassage = "Success";
                        bpe.Status = "I";
                        bpe.CreatedBy = User.UserID;
                        bpe.CreatedTimeStamp = datestamp;
                        new EventLogDAL().AddEventLog(bpe);
                    }
                    db.SaveChanges();


                    return true;
                }
                catch (Exception ex)
                {
                    BPEventLog bpe = new BPEventLog();
                    bpe.Object = "Budget Perjawatan-Batch Change";
                    bpe.ObjectName = GetGroupPerjawatanCodePrefix(LstSegmentDetailIDs);
                    //bpe.ObjectChanges = "Batch status Changed From '" + FromStatus + "' To '" + ToStatus + "', IDs : "
                    //    + data.Select(x => x.BudgetPerjawatanID.ToString()).Aggregate((x, y) => x + "," + y);
                    bpe.EventMassage = "Failure";
                    bpe.Status = "I";
                    bpe.CreatedBy = User.UserID;
                    bpe.CreatedTimeStamp = datestamp;
                    new EventLogDAL().AddEventLog(bpe);

                    throw ex;
                }
            }

            return true;
        }

        private string GetGroupPerjawatanCodePrefix(List<JuncBgtPerjawatanSegDtl> lstBgtPerjawatanSegDtl)
        {
            if (lstBgtPerjawatanSegDtl.Count > 0)
            {
                List<int> ids = lstBgtPerjawatanSegDtl.Select(x => x.SegmentDetailID).ToList();
                return GetGroupPerjawatanCodePrefix(ids);
            }
            return string.Empty;
        }

        private string GetGroupPerjawatanCodePrefix(List<int> LstSegmentDetailIDs)
        {
            List<SegmentDetail> lsd = db.SegmentDetails.Where(x => LstSegmentDetailIDs.Contains(x.SegmentDetailID)).OrderBy(x => x.Segment.SegmentOrder).ToList();
            if (lsd.Count > 0)
            {
                return lsd.Select(x => x.DetailCode).Aggregate((x, y) => x + "-" + y);
            }
            return string.Empty;
        }

        public List<int> GetBlockedPerjawatanYears()
        {
            return db.YearEnds.Where(x => x.BudgetType == "Perjawatan" && x.Status == "D").Select(x => x.BudgetYear).ToList();
        }

        public List<int> GetOpenPerjawatanYears()
        {
            return db.YearEnds.Where(x => x.BudgetType == "Perjawatan" && x.Status == "A").Select(x => x.BudgetYear).ToList();
        }
    }
}
