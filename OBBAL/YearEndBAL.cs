using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OBDAL;

namespace OBBAL
{
    public class YearEndBAL
    {
        public List<YearEnd> GetYearEnds()
        {
            return new YearEndDAL().GetYearEnds();
        }

        public bool InsertYearEnd(YearEnd objYearEnd)
        {
            try
            {
                return new YearEndDAL().InsertYearEnd(objYearEnd);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateYearEnd(YearEnd objYearEnd)
        {
            try
            {
                return new YearEndDAL().UpdateYearEnd(objYearEnd);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
