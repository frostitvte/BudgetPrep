using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OBDAL;

namespace OBBAL
{
    public class FieldMenguruBAL
    {
        public List<FieldMenguru> GetFieldMengurus()
        {
            return new FieldMenguruDAL().GetFieldMengurus();
        }

        public bool InsertFieldMenguru(FieldMenguru objFieldMenguru)
        {
            try
            {
                return new FieldMenguruDAL().InsertFieldMenguru(objFieldMenguru);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateFieldMenguru(FieldMenguru objFieldMenguru)
        {
            try
            {
                return new FieldMenguruDAL().UpdateFieldMenguru(objFieldMenguru);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
