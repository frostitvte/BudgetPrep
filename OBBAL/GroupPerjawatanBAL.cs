using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OBDAL;

namespace OBBAL
{
    public class GroupPerjawatanBAL
    {
        public IQueryable<GroupPerjawatan> GetGroupPerjawatans()
        {
            return new GroupPerjawatanDAL().GetGroupPerjawatans();
        }

        public bool InsertGroupPerjawatan(GroupPerjawatan objSegment)
        {
            try
            {
                return new GroupPerjawatanDAL().InsertGroupPerjawatan(objSegment);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateGroupPerjawatan(GroupPerjawatan objSegment)
        {
            try
            {
                return new GroupPerjawatanDAL().UpdateGroupPerjawatan(objSegment);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
