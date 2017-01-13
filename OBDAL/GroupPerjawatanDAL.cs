using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBDAL
{
    public class GroupPerjawatanDAL
    {
        OBEntities db = new OBEntities();

        public IQueryable<GroupPerjawatan> GetGroupPerjawatans()
        {
            return db.GroupPerjawatans.Select(x => x);
        }

        public bool InsertGroupPerjawatan(GroupPerjawatan objGroupPerjawatan)
        {
            try
            {
                db.GroupPerjawatans.Add(objGroupPerjawatan);
                db.SaveChanges();

                BPEventLog bpe = new BPEventLog();
                bpe.Object = "ServiceGroup";
                bpe.ObjectName = objGroupPerjawatan.GroupPerjawatanCode;
                bpe.ObjectChanges = string.Empty;
                bpe.EventMassage = "Success";
                bpe.Status = "A";
                bpe.CreatedBy = objGroupPerjawatan.CreatedBy;
                bpe.CreatedTimeStamp = objGroupPerjawatan.CreatedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                return true;
            }
            catch (Exception ex)
            {
                BPEventLog bpe = new BPEventLog();
                bpe.Object = "ServiceGroup";
                bpe.ObjectName = objGroupPerjawatan.GroupPerjawatanCode;
                bpe.ObjectChanges = string.Empty;
                bpe.EventMassage = "Failure";
                bpe.Status = "A";
                bpe.CreatedBy = objGroupPerjawatan.CreatedBy;
                bpe.CreatedTimeStamp = objGroupPerjawatan.CreatedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                throw ex;
            }
        }

        public bool UpdateGroupPerjawatan(GroupPerjawatan objGroupPerjawatan)
        {
            GroupPerjawatan obj = db.GroupPerjawatans.Where(x => x.GroupPerjawatanCode == objGroupPerjawatan.GroupPerjawatanCode).FirstOrDefault();
            string changes = new EventLogDAL().ObjectDifference(obj, objGroupPerjawatan);
            try
            {
                if (obj != null)
                {
                    //obj.GroupPerjawatanCode = objGroupPerjawatan.GroupPerjawatanCode;
                    obj.GroupPerjawatanDesc = objGroupPerjawatan.GroupPerjawatanDesc;
                    obj.ParentGroupPerjawatanID = objGroupPerjawatan.ParentGroupPerjawatanID;
                    obj.Status = objGroupPerjawatan.Status;
                    obj.ModifiedBy = objGroupPerjawatan.ModifiedBy;
                    obj.ModifiedTimeStamp = objGroupPerjawatan.ModifiedTimeStamp;
                    db.SaveChanges();

                    BPEventLog bpe = new BPEventLog();
                    bpe.Object = "ServiceGroup";
                    bpe.ObjectName = objGroupPerjawatan.GroupPerjawatanCode;
                    bpe.ObjectChanges = changes;
                    bpe.EventMassage = "Success";
                    bpe.Status = "A";
                    bpe.CreatedBy = objGroupPerjawatan.ModifiedBy;
                    bpe.CreatedTimeStamp = objGroupPerjawatan.ModifiedTimeStamp;
                    new EventLogDAL().AddEventLog(bpe);
                }
                return true;
            }
            catch (Exception ex)
            {
                BPEventLog bpe = new BPEventLog();
                bpe.Object = "ServiceGroup";
                bpe.ObjectName = objGroupPerjawatan.GroupPerjawatanCode;
                bpe.ObjectChanges = changes;
                bpe.EventMassage = "Failure";
                bpe.Status = "A";
                bpe.CreatedBy = objGroupPerjawatan.ModifiedBy;
                bpe.CreatedTimeStamp = objGroupPerjawatan.ModifiedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                throw ex;
            }
        }
    }
}
