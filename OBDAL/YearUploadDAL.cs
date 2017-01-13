using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBDAL
{
    public class YearUploadDAL
    {
        OBEntities db = new OBEntities();

        public List<YearUploadSetup> GetYearUpload()
        {
            return db.YearUploadSetups.ToList();
        }

        public bool InsertYearUpload(YearUploadSetup objYearUpload)
        {
            try
            {
                db.YearUploadSetups.Add(objYearUpload);
                db.SaveChanges();

                BPEventLog bpe = new BPEventLog();
                bpe.Object = "Year Upload Setup";
                bpe.ObjectName = objYearUpload.BudgetYear + " - " + objYearUpload.BudgetYearDesc;
                bpe.ObjectChanges = "<tr><td>Status</td><td></td><td>" + ((objYearUpload.Status == "A") ? "Active" : "Ended") + "</td></tr>";
                bpe.EventMassage = "Success";
                bpe.Status = "A";
                bpe.CreatedBy = objYearUpload.CreatedBy;
                bpe.CreatedTimeStamp = objYearUpload.CreatedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                return true;
            }
            catch (Exception ex)
            {
                BPEventLog bpe = new BPEventLog();
                bpe.Object = "Year Upload Setup";
                bpe.ObjectName = objYearUpload.BudgetYear + " - " + objYearUpload.BudgetYearDesc;
                bpe.ObjectChanges = "<tr><td>Status</td><td></td><td>" + ((objYearUpload.Status == "A") ? "Active" : "Ended") + "</td></tr>";
                bpe.EventMassage = "Failure";
                bpe.Status = "A";
                bpe.CreatedBy = objYearUpload.CreatedBy;
                bpe.CreatedTimeStamp = objYearUpload.CreatedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                throw ex;
            }
        }

        public bool UpdateYearUpload(YearUploadSetup objYearUpload)
        {
            YearUploadSetup obj = db.YearUploadSetups.Where(x => x.BudgetYear == objYearUpload.BudgetYear).FirstOrDefault();
            string changes = new EventLogDAL().ObjectDifference(obj, objYearUpload);
            try
            {
                if (obj != null)
                {
                    obj.BudgetYear = objYearUpload.BudgetYear;
                    obj.BudgetYearDesc = objYearUpload.BudgetYearDesc;
                    obj.Status = objYearUpload.Status;
                    obj.ModifiedBy = objYearUpload.ModifiedBy;
                    obj.ModifiedTimeStamp = objYearUpload.ModifiedTimeStamp;
                    db.SaveChanges();

                    BPEventLog bpe = new BPEventLog();
                    bpe.Object = "Year Upload Setup";
                    bpe.ObjectName = objYearUpload.BudgetYear + " - " + objYearUpload.BudgetYearDesc; ;
                    bpe.ObjectChanges = changes;
                    bpe.EventMassage = "Success";
                    bpe.Status = "A";
                    bpe.CreatedBy = objYearUpload.ModifiedBy;
                    bpe.CreatedTimeStamp = objYearUpload.ModifiedTimeStamp;
                    new EventLogDAL().AddEventLog(bpe);
                }
                return true;
            }
            catch (Exception ex)
            {
                BPEventLog bpe = new BPEventLog();
                bpe.Object = "Year Upload Setup";
                bpe.ObjectName = objYearUpload.BudgetYear + " - " + objYearUpload.BudgetYearDesc;
                bpe.ObjectChanges = changes;
                bpe.EventMassage = "Failure";
                bpe.Status = "A";
                bpe.CreatedBy = objYearUpload.ModifiedBy;
                bpe.CreatedTimeStamp = objYearUpload.ModifiedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                throw ex;
            }
        }
    }
}
