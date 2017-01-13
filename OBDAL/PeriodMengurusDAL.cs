using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBDAL
{
    public class PeriodMengurusDAL
    {
        OBEntities db = new OBEntities();

        public List<PeriodMenguru> GetPeriodMengurus()
        {
            return db.PeriodMengurus.Where(x => x.FieldMenguru.Status == "A").ToList();
        }

        public List<PeriodMenguru> GetAllPeriodMengurus()
        {
            return db.PeriodMengurus.ToList();
        }

        public bool InsertPeriodMenguru(PeriodMenguru objPeriodMenguru)
        {
            FieldMenguru objfm = db.FieldMengurus.Where(x => x.FieldMengurusID == objPeriodMenguru.FieldMengurusID).FirstOrDefault();
            try
            {
                db.PeriodMengurus.Add(objPeriodMenguru);
                db.SaveChanges();

                BPEventLog bpe = new BPEventLog();
                bpe.Object = "Period Mengurus";
                bpe.ObjectName = objfm.FieldMengurusDesc + " - " + objPeriodMenguru.MengurusYear;
                bpe.ObjectChanges = string.Empty;
                bpe.EventMassage = "Success";
                bpe.Status = "A";
                bpe.CreatedBy = objPeriodMenguru.CreatedBy;
                bpe.CreatedTimeStamp = objPeriodMenguru.CreatedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                return true;
            }
            catch (Exception ex)
            {
                BPEventLog bpe = new BPEventLog();
                bpe.Object = "Period Mengurus";
                bpe.ObjectName = objfm.FieldMengurusDesc + " - " + objPeriodMenguru.MengurusYear;
                bpe.ObjectChanges = string.Empty;
                bpe.EventMassage = "Failure";
                bpe.Status = "A";
                bpe.CreatedBy = objPeriodMenguru.CreatedBy;
                bpe.CreatedTimeStamp = objPeriodMenguru.CreatedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                throw ex;
            }
        }

        public bool UpdatePeriodMenguru(PeriodMenguru objPeriodMenguru)
        {
            PeriodMenguru obj = db.PeriodMengurus.Where(x => x.PeriodMengurusID == objPeriodMenguru.PeriodMengurusID).FirstOrDefault();
            string changes = new EventLogDAL().ObjectDifference(obj, objPeriodMenguru);
            try
            {
                if (obj != null)
                {
                    obj.FieldMengurusID = objPeriodMenguru.FieldMengurusID;
                    obj.MengurusYear = objPeriodMenguru.MengurusYear;
                    obj.Status = objPeriodMenguru.Status;
                    obj.ModifiedBy = objPeriodMenguru.ModifiedBy;
                    obj.ModifiedTimeStamp = objPeriodMenguru.ModifiedTimeStamp;
                    db.SaveChanges();

                    BPEventLog bpe = new BPEventLog();
                    bpe.Object = "Period Mengurus";
                    bpe.ObjectName = obj.FieldMenguru.FieldMengurusDesc + " - " + objPeriodMenguru.MengurusYear;
                    bpe.ObjectChanges = changes;
                    bpe.EventMassage = "Success";
                    bpe.Status = "A";
                    bpe.CreatedBy = objPeriodMenguru.ModifiedBy;
                    bpe.CreatedTimeStamp = objPeriodMenguru.ModifiedTimeStamp;
                    new EventLogDAL().AddEventLog(bpe);
                }
                return true;
            }
            catch (Exception ex)
            {
                BPEventLog bpe = new BPEventLog();
                bpe.Object = "Period Mengurus";
                bpe.ObjectName = obj.FieldMenguru.FieldMengurusDesc + " - " + objPeriodMenguru.MengurusYear;
                bpe.ObjectChanges = changes;
                bpe.EventMassage = "Failure";
                bpe.Status = "A";
                bpe.CreatedBy = objPeriodMenguru.ModifiedBy;
                bpe.CreatedTimeStamp = objPeriodMenguru.ModifiedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                throw ex;
            }
        }
    }
}
