using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBDAL
{
    public class AccountCodeDAL
    {
        OBEntities db = new OBEntities();        

        public IQueryable<AccountCode> GetAccountCodes()
        {
            //db.Database.ExecuteSqlCommand();
            
            return db.AccountCodes.Select(x => x);
        }

        public AccountCode GetParentAccountCode(AccountCode AccountCode)
        {
            return db.AccountCodes.Where(x => x.AccountCode1 == AccountCode.ParentAccountCode).FirstOrDefault();
        }

        public bool InsertAccountCode(AccountCode objAccountCode)
        {
            try
            {
                db.AccountCodes.Add(objAccountCode);
                db.SaveChanges();

                BPEventLog bpe = new BPEventLog();
                bpe.Object = "AccountCode";
                bpe.ObjectName = objAccountCode.AccountCode1;
                bpe.ObjectChanges = string.Empty;
                bpe.EventMassage = "Success";
                bpe.Status = "A";
                bpe.CreatedBy = objAccountCode.CreatedBy;
                bpe.CreatedTimeStamp = objAccountCode.CreatedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);
                return true;
            }
            catch (Exception ex)
            {
                BPEventLog bpe = new BPEventLog();
                bpe.Object = "AccountCode";
                bpe.ObjectName = objAccountCode.AccountCode1;
                bpe.ObjectChanges = string.Empty;
                bpe.EventMassage = "Failure";
                bpe.Status = "A";
                bpe.CreatedBy = objAccountCode.CreatedBy;
                bpe.CreatedTimeStamp = objAccountCode.CreatedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);
                return true;

                throw ex;
            }
        }

        public bool UpdateAccountCode(AccountCode objAccountCode)
        {
            AccountCode obj = db.AccountCodes.Where(x => x.AccountCode1 == objAccountCode.AccountCode1).FirstOrDefault();
            string changes = new EventLogDAL().ObjectDifference(obj, objAccountCode);
            try
            {                
                if (obj != null && changes != string.Empty)
                {
                    //obj.AccountCode1 = objAccountCode.AccountCode1;
                    obj.AccountDesc = objAccountCode.AccountDesc;
                    obj.ParentAccountCode = objAccountCode.ParentAccountCode;
                    obj.Keterangan = objAccountCode.Keterangan;
                    obj.Pengiraan = objAccountCode.Pengiraan;
                    obj.Status = objAccountCode.Status;
                    obj.ModifiedBy = objAccountCode.ModifiedBy;
                    obj.ModifiedTimeStamp = objAccountCode.ModifiedTimeStamp;
                    db.SaveChanges();

                    BPEventLog bpe = new BPEventLog();
                    bpe.Object = "AccountCode";
                    bpe.ObjectName = objAccountCode.AccountCode1;
                    bpe.ObjectChanges = changes;
                    bpe.EventMassage = "Success";
                    bpe.Status = "A";
                    bpe.CreatedBy = objAccountCode.ModifiedBy;
                    bpe.CreatedTimeStamp = objAccountCode.ModifiedTimeStamp;
                    new EventLogDAL().AddEventLog(bpe);
                }
                return true;
            }
            catch (Exception ex)
            {
                BPEventLog bpe = new BPEventLog();
                bpe.Object = "AccountCode";
                bpe.ObjectName = objAccountCode.AccountCode1;
                bpe.ObjectChanges = changes;
                bpe.EventMassage = "Failure";
                bpe.Status = "A";
                bpe.CreatedBy = objAccountCode.ModifiedBy;
                bpe.CreatedTimeStamp = objAccountCode.ModifiedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                throw ex;
            }
        }
    }
}
