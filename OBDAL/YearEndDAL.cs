using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBDAL
{
    public class YearEndDAL
    {
        OBEntities db = new OBEntities();

        public List<YearEnd> GetYearEnds()
        {
            return db.YearEnds.ToList();
        }

        public bool InsertYearEnd(YearEnd objYearEnd)
        {
            try
            {
                db.YearEnds.Add(objYearEnd);
                db.SaveChanges();

                BPEventLog bpe = new BPEventLog();
                bpe.Object = "Year End";
                bpe.ObjectName = objYearEnd.BudgetType + " - " + objYearEnd.BudgetYear;
                bpe.ObjectChanges = "<tr><td>Status</td><td></td><td>" + ((objYearEnd.Status == "A") ? "Active" : "Ended") + "</td></tr>";
                bpe.EventMassage = "Success";
                bpe.Status = "A";
                bpe.CreatedBy = objYearEnd.CreatedBy;
                bpe.CreatedTimeStamp = objYearEnd.CreatedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                return true;
            }
            catch (Exception ex)
            {
                BPEventLog bpe = new BPEventLog();
                bpe.Object = "Year End";
                bpe.ObjectName = objYearEnd.BudgetType + " - " + objYearEnd.BudgetYear;
                bpe.ObjectChanges = "<tr><td>Status</td><td></td><td>" + ((objYearEnd.Status == "A") ? "Active" : "Ended") + "</td></tr>";
                bpe.EventMassage = "Failure";
                bpe.Status = "A";
                bpe.CreatedBy = objYearEnd.CreatedBy;
                bpe.CreatedTimeStamp = objYearEnd.CreatedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                throw ex;
            }
        }

        public bool UpdateYearEnd(YearEnd objYearEnd)
        {
            YearEnd obj = db.YearEnds.Where(x => x.BudgetType == objYearEnd.BudgetType && x.BudgetYear == objYearEnd.BudgetYear).FirstOrDefault();
            string changes = new EventLogDAL().ObjectDifference(obj, objYearEnd);
            try
            {
                if (obj != null)
                {
                    obj.BudgetType = objYearEnd.BudgetType;
                    obj.BudgetYear = objYearEnd.BudgetYear;
                    obj.Status = objYearEnd.Status;
                    obj.ModifiedBy = objYearEnd.ModifiedBy;
                    obj.ModifiedTimeStamp = objYearEnd.ModifiedTimeStamp;
                    db.SaveChanges();

                    BPEventLog bpe = new BPEventLog();
                    bpe.Object = "Year End";
                    bpe.ObjectName = objYearEnd.BudgetType + " - " + objYearEnd.BudgetYear; ;
                    bpe.ObjectChanges = changes;
                    bpe.EventMassage = "Success";
                    bpe.Status = "A";
                    bpe.CreatedBy = objYearEnd.ModifiedBy;
                    bpe.CreatedTimeStamp = objYearEnd.ModifiedTimeStamp;
                    new EventLogDAL().AddEventLog(bpe);
                }
                return true;
            }
            catch (Exception ex)
            {
                BPEventLog bpe = new BPEventLog();
                bpe.Object = "Year End";
                bpe.ObjectName = objYearEnd.BudgetType + " - " + objYearEnd.BudgetYear;
                bpe.ObjectChanges = changes;
                bpe.EventMassage = "Failure";
                bpe.Status = "A";
                bpe.CreatedBy = objYearEnd.ModifiedBy;
                bpe.CreatedTimeStamp = objYearEnd.ModifiedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                throw ex;
            }
        }
    }
}
