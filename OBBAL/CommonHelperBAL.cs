using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OBDAL;

namespace OBBAL
{
    public class CommonHelperBAL
    {
        public bool BackupDB(string EXEPath, string DirectoryPath, string FileName, ref string Result)
        {
            return new CommonHelperDAL().BackupDB(EXEPath, DirectoryPath, FileName, ref Result);
        }

        public System.Data.Common.DbConnectionStringBuilder GetConnectionStringBuilder()
        {
            return new CommonHelperDAL().GetConnectionStringBuilder();
        }
    }
}
