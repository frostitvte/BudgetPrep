using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OBDAL;

namespace OBBAL
{
    public class AccountCodeBAL
    {
        public IQueryable<AccountCode> GetAccountCodes()
        {
            return new AccountCodeDAL().GetAccountCodes();
        }

        public AccountCode GetParentAccountCode(AccountCode AccountCode)
        {
            return new AccountCodeDAL().GetParentAccountCode(AccountCode);
        }

        public bool InsertAccountCode(AccountCode objSegment)
        {
            try
            {
                return new AccountCodeDAL().InsertAccountCode(objSegment);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateAccountCode(AccountCode objSegment)
        {
            try
            {
                return new AccountCodeDAL().UpdateAccountCode(objSegment);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
