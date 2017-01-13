using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBDAL
{
    public class FieldMenguruDAL
    {
        OBEntities db = new OBEntities();

        public List<FieldMenguru> GetFieldMengurus()
        {
            return db.FieldMengurus.Select(x => x).ToList();
        }

        public bool InsertFieldMenguru(FieldMenguru objFieldMenguru)
        {
            try
            {
                objFieldMenguru.FieldMengurusSDesc = string.Empty;

                db.FieldMengurus.Add(objFieldMenguru);
                db.SaveChanges();

                BPEventLog bpe = new BPEventLog();
                bpe.Object = "FieldMengurus";
                bpe.ObjectName = objFieldMenguru.FieldMengurusDesc;
                bpe.ObjectChanges = string.Empty;
                bpe.EventMassage = "Success";
                bpe.Status = "A";
                bpe.CreatedBy = objFieldMenguru.CreatedBy;
                bpe.CreatedTimeStamp = objFieldMenguru.CreatedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                return true;
            }
            catch (Exception ex)
            {
                BPEventLog bpe = new BPEventLog();
                bpe.Object = "FieldMengurus";
                bpe.ObjectName = objFieldMenguru.FieldMengurusDesc;
                bpe.ObjectChanges = string.Empty;
                bpe.EventMassage = "Failure";
                bpe.Status = "A";
                bpe.CreatedBy = objFieldMenguru.CreatedBy;
                bpe.CreatedTimeStamp = objFieldMenguru.CreatedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                throw ex;
            }
        }

        public bool UpdateFieldMenguru(FieldMenguru objFieldMenguru)
        {
            FieldMenguru obj = db.FieldMengurus.Where(x => x.FieldMengurusID == objFieldMenguru.FieldMengurusID).FirstOrDefault();
            string changes = new EventLogDAL().ObjectDifference(obj, objFieldMenguru);
            try
            {
                if (obj != null)
                {
                    obj.FieldMengurusID = objFieldMenguru.FieldMengurusID ;
                    //obj.FieldMengurusSDesc = objFieldMenguru.FieldMengurusSDesc;
                    obj.FieldMengurusSDesc = string.Empty;
                    obj.FieldMengurusDesc = objFieldMenguru.FieldMengurusDesc;
                    obj.Status = objFieldMenguru.Status;
                    obj.ModifiedBy = objFieldMenguru.ModifiedBy;
                    obj.ModifiedTimeStamp = objFieldMenguru.ModifiedTimeStamp;
                    db.SaveChanges();

                    BPEventLog bpe = new BPEventLog();
                    bpe.Object = "FieldMenguru";
                    bpe.ObjectName = objFieldMenguru.FieldMengurusDesc;
                    bpe.ObjectChanges = changes;
                    bpe.EventMassage = "Success";
                    bpe.Status = "A";
                    bpe.CreatedBy = objFieldMenguru.ModifiedBy;
                    bpe.CreatedTimeStamp = objFieldMenguru.ModifiedTimeStamp;
                    new EventLogDAL().AddEventLog(bpe);
                }
                return true;
            }
            catch (Exception ex)
            {
                BPEventLog bpe = new BPEventLog();
                bpe.Object = "FieldMenguru";
                bpe.ObjectName = objFieldMenguru.FieldMengurusDesc;
                bpe.ObjectChanges = changes;
                bpe.EventMassage = "Failure";
                bpe.Status = "A";
                bpe.CreatedBy = objFieldMenguru.ModifiedBy;
                bpe.CreatedTimeStamp = objFieldMenguru.ModifiedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);
                throw ex;
            }
        }
    }
}
