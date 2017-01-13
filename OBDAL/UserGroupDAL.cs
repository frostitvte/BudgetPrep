using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBDAL
{
    public class UserGroupDAL
    {
        OBEntities db = new OBEntities();

        public List<MasterGroup> GetMasterGroups()
        {
            return db.MasterGroups.Select(x => x).ToList();
        }

        public bool InsertMasterGroup(MasterGroup objMasterGroup)
        {
            try
            {
                db.MasterGroups.Add(objMasterGroup);
                db.SaveChanges();

                BPEventLog bpe = new BPEventLog();
                bpe.Object = "UserGroup";
                bpe.ObjectName = objMasterGroup.GroupName;
                bpe.ObjectChanges = string.Empty;
                bpe.EventMassage = "Success";
                bpe.Status = "A";
                bpe.CreatedBy = objMasterGroup.CreatedBy;
                bpe.CreatedTimeStamp = objMasterGroup.CreatedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                return true;
            }
            catch (Exception ex)
            {
                BPEventLog bpe = new BPEventLog();
                bpe.Object = "UserGroup";
                bpe.ObjectName = objMasterGroup.GroupName;
                bpe.ObjectChanges = string.Empty;
                bpe.EventMassage = "Failure";
                bpe.Status = "A";
                bpe.CreatedBy = objMasterGroup.CreatedBy;
                bpe.CreatedTimeStamp = objMasterGroup.CreatedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                throw ex;
            }
        }

        public bool UpdateMasterGroup(MasterGroup objMasterGroup)
        {
            MasterGroup obj = db.MasterGroups.Where(x => x.GroupID == objMasterGroup.GroupID).FirstOrDefault();
            string changes = new EventLogDAL().ObjectDifference(obj, objMasterGroup);
            try
            {
                if (obj != null)
                {
                    obj.GroupName = objMasterGroup.GroupName;
                    obj.Status = objMasterGroup.Status;
                    obj.ModifiedBy = objMasterGroup.ModifiedBy;
                    obj.ModifiedTimeStamp = objMasterGroup.ModifiedTimeStamp;
                    db.SaveChanges();

                    BPEventLog bpe = new BPEventLog();
                    bpe.Object = "UserGroup";
                    bpe.ObjectName = objMasterGroup.GroupName;
                    bpe.ObjectChanges = changes;
                    bpe.EventMassage = "Success";
                    bpe.Status = "A";
                    bpe.CreatedBy = objMasterGroup.ModifiedBy;
                    bpe.CreatedTimeStamp = objMasterGroup.ModifiedTimeStamp;
                    new EventLogDAL().AddEventLog(bpe);
                }
                return true;
            }
            catch (Exception ex)
            {
                BPEventLog bpe = new BPEventLog();
                bpe.Object = "UserGroup";
                bpe.ObjectName = objMasterGroup.GroupName;
                bpe.ObjectChanges = changes;
                bpe.EventMassage = "Failure";
                bpe.Status = "A";
                bpe.CreatedBy = objMasterGroup.ModifiedBy;
                bpe.CreatedTimeStamp = objMasterGroup.ModifiedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                throw ex;
            }
        }
    }
}
