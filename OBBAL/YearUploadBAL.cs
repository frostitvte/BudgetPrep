using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OBDAL;

namespace OBBAL
{
    public class YearUploadBAL
    {
        public List<YearUploadSetup> GetYearUpload()
        {
            return new YearUploadDAL().GetYearUpload();
        }

        public bool InsertYearUpload(YearUploadSetup objYearUpload)
        {
            try
            {
                return new YearUploadDAL().InsertYearUpload(objYearUpload);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateYearUpload(YearUploadSetup objYearUpload)
        {
            try
            {
                return new YearUploadDAL().UpdateYearUpload(objYearUpload);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
