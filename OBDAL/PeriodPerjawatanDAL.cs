using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBDAL
{
    public class PeriodPerjawatanDAL
    {
        OBEntities db = new OBEntities();

        public List<PeriodPerjawatan> GetPeriodPerjawatans()
        {
            return db.PeriodPerjawatans.Where(x => x.FieldPerjawatan.Status == "A").ToList();
        }

        public List<PeriodPerjawatan> GetAllPeriodPerjawatans()
        {
            return db.PeriodPerjawatans.ToList();
        }

        public bool InsertPeriodPerjawatan(PeriodPerjawatan objPeriodPerjawatan)
        {
            FieldPerjawatan obj = db.FieldPerjawatans.Where(x => x.FieldPerjawatanID == objPeriodPerjawatan.FieldPerjawatanID).FirstOrDefault();
            try
            {
                db.PeriodPerjawatans.Add(objPeriodPerjawatan);
                db.SaveChanges();

                BPEventLog bpe = new BPEventLog();
                bpe.Object = "Period Perjawatan";
                bpe.ObjectName = obj.FieldPerjawatanDesc + " - " + objPeriodPerjawatan.PerjawatanYear;
                bpe.ObjectChanges = string.Empty;
                bpe.EventMassage = "Success";
                bpe.Status = "A";
                bpe.CreatedBy = objPeriodPerjawatan.CreatedBy;
                bpe.CreatedTimeStamp = objPeriodPerjawatan.CreatedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                return true;
            }
            catch (Exception ex)
            {
                BPEventLog bpe = new BPEventLog();
                bpe.Object = "Period Perjawatan";
                bpe.ObjectName = obj.FieldPerjawatanDesc + " - " + objPeriodPerjawatan.PerjawatanYear;
                bpe.ObjectChanges = string.Empty;
                bpe.EventMassage = "Failure";
                bpe.Status = "A";
                bpe.CreatedBy = objPeriodPerjawatan.CreatedBy;
                bpe.CreatedTimeStamp = objPeriodPerjawatan.CreatedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                throw ex;
            }
        }

        public bool UpdatePeriodPerjawatan(PeriodPerjawatan objPeriodPerjawatan)
        {
            PeriodPerjawatan obj = db.PeriodPerjawatans.Where(x => x.PeriodPerjawatanID == objPeriodPerjawatan.PeriodPerjawatanID).FirstOrDefault();
            string changes = new EventLogDAL().ObjectDifference(obj, objPeriodPerjawatan);
            try
            {
                if (obj != null)
                {
                    obj.FieldPerjawatanID = objPeriodPerjawatan.FieldPerjawatanID;
                    obj.PerjawatanYear = objPeriodPerjawatan.PerjawatanYear;
                    obj.Status = objPeriodPerjawatan.Status;
                    obj.ModifiedBy = objPeriodPerjawatan.ModifiedBy;
                    obj.ModifiedTimeStamp = objPeriodPerjawatan.ModifiedTimeStamp;
                    db.SaveChanges();

                    BPEventLog bpe = new BPEventLog();
                    bpe.Object = "Period Perjawatan";
                    bpe.ObjectName = obj.FieldPerjawatan.FieldPerjawatanDesc + " - " + objPeriodPerjawatan.PerjawatanYear;
                    bpe.ObjectChanges = changes;
                    bpe.EventMassage = "Success";
                    bpe.Status = "A";
                    bpe.CreatedBy = objPeriodPerjawatan.ModifiedBy;
                    bpe.CreatedTimeStamp = objPeriodPerjawatan.ModifiedTimeStamp;
                    new EventLogDAL().AddEventLog(bpe);
                }
                return true;
            }
            catch (Exception ex)
            {
                BPEventLog bpe = new BPEventLog();
                bpe.Object = "Period Perjawatan";
                bpe.ObjectName = obj.FieldPerjawatan.FieldPerjawatanDesc + " - " + objPeriodPerjawatan.PerjawatanYear;
                bpe.ObjectChanges = changes;
                bpe.EventMassage = "Failure";
                bpe.Status = "A";
                bpe.CreatedBy = objPeriodPerjawatan.ModifiedBy;
                bpe.CreatedTimeStamp = objPeriodPerjawatan.ModifiedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                throw ex;
            }
        }
    }
}
