using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBDAL
{
    public class FieldPerjawatanDAL
    {
        OBEntities db = new OBEntities();

        public List<FieldPerjawatan> GetFieldPerjawatans()
        {
            return db.FieldPerjawatans.Select(x => x).ToList();
        }

        public bool InsertFieldPerjawatan(FieldPerjawatan objFieldPerjawatan)
        {
            try
            {
                objFieldPerjawatan.FieldPerjawatanSDesc = string.Empty;

                db.FieldPerjawatans.Add(objFieldPerjawatan);
                db.SaveChanges();

                BPEventLog bpe = new BPEventLog();
                bpe.Object = "FieldPerjawatan";
                bpe.ObjectName = objFieldPerjawatan.FieldPerjawatanDesc;
                bpe.ObjectChanges = string.Empty;
                bpe.EventMassage = "Success";
                bpe.Status = "A";
                bpe.CreatedBy = objFieldPerjawatan.CreatedBy;
                bpe.CreatedTimeStamp = objFieldPerjawatan.CreatedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                return true;
            }
            catch (Exception ex)
            {
                BPEventLog bpe = new BPEventLog();
                bpe.Object = "FieldPerjawatan";
                bpe.ObjectName = objFieldPerjawatan.FieldPerjawatanDesc;
                bpe.ObjectChanges = string.Empty;
                bpe.EventMassage = "Failure";
                bpe.Status = "A";
                bpe.CreatedBy = objFieldPerjawatan.CreatedBy;
                bpe.CreatedTimeStamp = objFieldPerjawatan.CreatedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                throw ex;
            }
        }

        public bool UpdateFieldPerjawatan(FieldPerjawatan objFieldPerjawatan)
        {
            FieldPerjawatan obj = db.FieldPerjawatans.Where(x => x.FieldPerjawatanID == objFieldPerjawatan.FieldPerjawatanID).FirstOrDefault();
            string changes = new EventLogDAL().ObjectDifference(obj, objFieldPerjawatan);
            try
            {
                if (obj != null)
                {
                    obj.FieldPerjawatanID = objFieldPerjawatan.FieldPerjawatanID;
                    //obj.FieldPerjawatanSDesc = objFieldPerjawatan.FieldPerjawatanSDesc;
                    obj.FieldPerjawatanSDesc = string.Empty;
                    obj.FieldPerjawatanDesc = objFieldPerjawatan.FieldPerjawatanDesc;
                    obj.Status = objFieldPerjawatan.Status;
                    obj.ModifiedBy = objFieldPerjawatan.ModifiedBy;
                    obj.ModifiedTimeStamp = objFieldPerjawatan.ModifiedTimeStamp;
                    db.SaveChanges();

                    BPEventLog bpe = new BPEventLog();
                    bpe.Object = "FieldPerjawatan";
                    bpe.ObjectName = objFieldPerjawatan.FieldPerjawatanDesc;
                    bpe.ObjectChanges = changes;
                    bpe.EventMassage = "Success";
                    bpe.Status = "A";
                    bpe.CreatedBy = objFieldPerjawatan.ModifiedBy;
                    bpe.CreatedTimeStamp = objFieldPerjawatan.ModifiedTimeStamp;
                    new EventLogDAL().AddEventLog(bpe);
                }
                return true;
            }
            catch (Exception ex)
            {
                BPEventLog bpe = new BPEventLog();
                bpe.Object = "FieldPerjawatan";
                bpe.ObjectName = objFieldPerjawatan.FieldPerjawatanDesc;
                bpe.ObjectChanges = changes;
                bpe.EventMassage = "Failure";
                bpe.Status = "A";
                bpe.CreatedBy = objFieldPerjawatan.ModifiedBy;
                bpe.CreatedTimeStamp = objFieldPerjawatan.ModifiedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                throw ex;
            }
        }
    }
}
