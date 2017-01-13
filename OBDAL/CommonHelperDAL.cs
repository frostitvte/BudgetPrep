using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OBDAL
{
    public class CommonHelperDAL
    {
        OBEntities db = new OBEntities();

        public bool BackupDB(string EXEPath, string DirectoryPath, string FileName, ref string Result)
        {
            try
            {
                System.Data.Common.DbConnectionStringBuilder builder = new System.Data.Common.DbConnectionStringBuilder();
                builder.ConnectionString = db.Database.Connection.ConnectionString;

                string cmd = "-i -h " + builder["HOST"].ToString() +
                           " -p " + builder["PORT"].ToString() +
                           " -U " + builder["USER ID"].ToString() +
                           " -F c -b -v -f " + DirectoryPath + FileName + " " + builder["DATABASE"].ToString();

                System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
                info.FileName = EXEPath;
                info.Arguments = cmd;
                info.CreateNoWindow = true;
                info.RedirectStandardOutput = true;
                info.RedirectStandardError = true;
                info.UseShellExecute = false;
                try { info.EnvironmentVariables.Add("HOST", builder["HOST"].ToString()); }
                catch (Exception) { }
                try { info.EnvironmentVariables.Add("DATABASE", builder["DATABASE"].ToString()); }
                catch (Exception) { }
                try { info.EnvironmentVariables.Add("USER ID", builder["USER ID"].ToString()); }
                catch (Exception) { }
                try { info.EnvironmentVariables.Add("PASSWORD", builder["PASSWORD"].ToString()); }
                catch (Exception) { }
                try { info.EnvironmentVariables.Add("PORT", builder["PORT"].ToString()); }
                catch (Exception) { }

                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = info;
                proc.Start();

                CancellationTokenSource cTokenSource = new CancellationTokenSource();
                CancellationToken cToken = cTokenSource.Token;

                //Result = await proc.StandardError.ReadToEndAsync();
                Result = Task.Factory.StartNew(() => proc.StandardError.ReadToEnd(), cToken).Result;
                proc.WaitForExit();
                if (proc.ExitCode == 0)
                {
                    Result += "\nBackup terminated successfully";
                    return true;
                }
                else
                {
                    Result += "\nError Occured";
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            //DataOnly = false;
            //Blobs = true;
            //Clean = false;
            //Oids = false;
            //NoOwner = false;
            //SchemaOnly = false;
            //NoPrivileges = false;
            //public string DataOnlyCode { get { if (DataOnly)  return " -a"; return ""; } }
            //public string BlobsCode { get { if (Blobs) return " -b"; return ""; } }
            //public string CleanCode { get { if (Clean && !DataOnly)   return " -c"; return ""; } } 
            //public string OidsCode { get { if (Oids) return " -o"; return ""; } }
            //public string NoOwnerCode { get { if (NoOwner) return " -O"; return ""; } }
            //public string SchemaOnlyCode { get { if (SchemaOnly) return " -s"; return ""; } }
            //public string NoPrivilegesCode { get { if (NoPrivileges) return " -x"; return ""; } }
        }

        public System.Data.Common.DbConnectionStringBuilder GetConnectionStringBuilder()
        {
            System.Data.Common.DbConnectionStringBuilder builder = new System.Data.Common.DbConnectionStringBuilder();
            builder.ConnectionString = db.Database.Connection.ConnectionString;

            return builder;
        }
    }
}
