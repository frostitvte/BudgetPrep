using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OBDAL;

namespace OBBAL
{
    public class PeriodPerjawatanBAL
    {
        public List<PeriodPerjawatan> GetPeriodPerjawatans()
        {
            return new PeriodPerjawatanDAL().GetPeriodPerjawatans();
        }

        public List<PeriodPerjawatan> GetAllPeriodPerjawatans()
        {
            return new PeriodPerjawatanDAL().GetAllPeriodPerjawatans();
        }

        public bool InsertPeriodPerjawatan(PeriodPerjawatan objPeriodPerjawatan)
        {
            try
            {
                return new PeriodPerjawatanDAL().InsertPeriodPerjawatan(objPeriodPerjawatan);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdatePeriodPerjawatan(PeriodPerjawatan objPeriodPerjawatan)
        {
            try
            {
                return new PeriodPerjawatanDAL().UpdatePeriodPerjawatan(objPeriodPerjawatan);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
