using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OBDAL;

namespace OBBAL
{
    public class ErrorLoggerBAL
    {
        public bool LogError()
        {
            return new ErrorLoggerDAL().LogError();
        }
    }
}
