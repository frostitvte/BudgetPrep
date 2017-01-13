using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OBDAL;

namespace OBBAL
{
    public class UserGroupBAL
    {
        public List<MasterGroup> GetMasterGroups()
        {
            return new UserGroupDAL().GetMasterGroups();
        }

        public bool InsertMasterGroup(MasterGroup objMasterGroup)
        {
            return new UserGroupDAL().InsertMasterGroup(objMasterGroup);            
        }

        public bool UpdateMasterGroup(MasterGroup objMasterGroup)
        {
            return new UserGroupDAL().UpdateMasterGroup(objMasterGroup);
        }
    }
}
