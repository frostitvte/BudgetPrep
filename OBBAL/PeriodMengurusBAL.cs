using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OBDAL;

namespace OBBAL
{
    public class PeriodMengurusBAL
    {
        public List<PeriodMenguru> GetPeriodMengurus()
        {
            return new PeriodMengurusDAL().GetPeriodMengurus();
        }

        public List<PeriodMenguru> GetAllPeriodMengurus()
        {
            return new PeriodMengurusDAL().GetAllPeriodMengurus();
        }

        public bool InsertPeriodMenguru(PeriodMenguru objPeriodMenguru)
        {
            try
            {
                return new PeriodMengurusDAL().InsertPeriodMenguru(objPeriodMenguru);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdatePeriodMenguru(PeriodMenguru objPeriodMenguru)
        {
            try
            {
                return new PeriodMengurusDAL().UpdatePeriodMenguru(objPeriodMenguru);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
