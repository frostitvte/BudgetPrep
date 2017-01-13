using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace OBDAL
{
    public class SegmentDAL
    {
        OBEntities db = new OBEntities();

        public List<Segment> GetSegments()
        {
            return db.Segments.Select(x => x).ToList();
        }

        public bool InsertSegment(Segment objSegment)
        {
            try
            {
                db.Segments.Add(objSegment);
                db.SaveChanges();

                BPEventLog bpe = new BPEventLog();
                bpe.Object = "Segment";
                bpe.ObjectName = objSegment.SegmentName;
                bpe.ObjectChanges = string.Empty;
                bpe.EventMassage = "Success";
                bpe.Status = "A";
                bpe.CreatedBy = objSegment.CreatedBy;
                bpe.CreatedTimeStamp = objSegment.CreatedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                return true;
            }
            catch (Exception ex)
            {
                BPEventLog bpe = new BPEventLog();
                bpe.Object = "Segment";
                bpe.ObjectName = objSegment.SegmentName;
                bpe.ObjectChanges = string.Empty;
                bpe.EventMassage = "Failure";
                bpe.Status = "A";
                bpe.CreatedBy = objSegment.CreatedBy;
                bpe.CreatedTimeStamp = objSegment.CreatedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                throw ex;
            }
        }

        public bool UpdateSegment(Segment objSegment)
        {
            Segment obj = db.Segments.Where(x => x.SegmentID == objSegment.SegmentID).FirstOrDefault();
            string changes = new EventLogDAL().ObjectDifference(obj, objSegment);
            try
            {                
                if (obj != null)
                {
                    obj.SegmentName = objSegment.SegmentName;
                    obj.ShapeFormat = objSegment.ShapeFormat;
                    obj.SegmentOrder = objSegment.SegmentOrder;
                    obj.Status = objSegment.Status;
                    obj.ModifiedBy = objSegment.ModifiedBy;
                    obj.ModifiedTimeStamp = objSegment.ModifiedTimeStamp;
                    db.SaveChanges();

                    BPEventLog bpe = new BPEventLog();
                    bpe.Object = "Segment";
                    bpe.ObjectName = objSegment.SegmentName;
                    bpe.ObjectChanges = changes;
                    bpe.EventMassage = "Success";
                    bpe.Status = "A";
                    bpe.CreatedBy = objSegment.ModifiedBy;
                    bpe.CreatedTimeStamp = objSegment.ModifiedTimeStamp;
                    new EventLogDAL().AddEventLog(bpe);
                }
                return true;
            }
            catch (Exception ex)
            {
                BPEventLog bpe = new BPEventLog();
                bpe.Object = "Segment";
                bpe.ObjectName = objSegment.SegmentName;
                bpe.ObjectChanges = changes;
                bpe.EventMassage = "Failure";
                bpe.Status = "A";
                bpe.CreatedBy = objSegment.ModifiedBy;
                bpe.CreatedTimeStamp = objSegment.ModifiedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                throw ex;
            }
        }
    }
}
