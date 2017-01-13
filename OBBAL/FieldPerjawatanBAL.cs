using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OBDAL;

namespace OBBAL
{
    public class FieldPerjawatanBAL
    {
        public List<FieldPerjawatan> GetFieldPerjawatans()
        {
            return new FieldPerjawatanDAL().GetFieldPerjawatans();
        }

        public bool InsertFieldPerjawatan(FieldPerjawatan objFieldPerjawatan)
        {
            try
            {
                return new FieldPerjawatanDAL().InsertFieldPerjawatan(objFieldPerjawatan);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateFieldPerjawatan(FieldPerjawatan objFieldPerjawatan)
        {
            try
            {
                return new FieldPerjawatanDAL().UpdateFieldPerjawatan(objFieldPerjawatan);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
