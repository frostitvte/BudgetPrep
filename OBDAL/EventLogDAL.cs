using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OBDAL
{
    public class InboxHelper
    {
        public string Title { get; set; }
        public int NoCount { get; set; }
        public string Object { get; set; }
        public DateTime LastModDateTime { get; set; }
        public string Detail { get; set; }
        public string ModifiedBy { get; set; }
    }

    public class EventLogDAL
    {
        OBEntities db = new OBEntities();

        public bool AddEventLog(BPEventLog objEventLog)
        {
            try
            {
                if (objEventLog.ObjectChanges == null)
                    objEventLog.ObjectChanges = string.Empty;

                db.BPEventLogs.Add(objEventLog);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                //throw ex;
            }
            return false;
        }

        public bool AddEventLogWithoutSubmit(BPEventLog objEventLog)
        {
            try
            {
                if (objEventLog.ObjectChanges == null)
                    objEventLog.ObjectChanges = string.Empty;

                db.BPEventLogs.Add(objEventLog);
                return true;
            }
            catch (Exception ex)
            {
                //throw ex;
            }
            return false;
        }

        private List<string> GetSearchStrings(MasterUser User)
        {
            List<string> search = (User.JuncUserRoles.First().RoleID == 1) ?
                new List<string>() { string.Empty } :
                (
                    (User.JuncUserRoles.First().RoleID == 2) ?
                    new List<string>() { "<td>O</td>", "<td>S</td>", "<td>P</td>", "<td>X</td>", "<td>Y</td>" } :
                    (
                        (User.JuncUserRoles.First().RoleID == 3) ?
                        new List<string>() { "<td>P</td>", "<td>R</td>", "<td>X</td>", "<td>Y</td>" } :
                        new List<string>() { "<td>R</td>", "<td>A</td>", "<td>Y</td>" }
                    )
                );

            return search;
        }

        private bool IsStringExists(string Phrase, List<string> LstSearch)
        {
            foreach (string s in LstSearch)
            {
                if (Phrase.ToUpper().Contains(s.ToUpper()))
                    return true;
            }
            return false;
        }

        public List<InboxHelper> GetInboxList(MasterUser User)
        {
            try
            {
                string Status = (User.JuncUserRoles.First().RoleID == 1) ? "A" : "I";

                List<string> lstsearch = GetSearchStrings(User);

                DateTime dtlimit = DateTime.Now.AddDays(30);
                var data = db.BPEventLogs.Where(x => x.Status == Status && x.ObjectChanges != null); //&& x.CreatedTimeStamp > dtlimit
                List<BPEventLog> EventData = data.ToList().Where(x => IsStringExists(x.ObjectChanges, lstsearch)).ToList();
                return (from x in EventData
                        group x by new { x.Object, x.ObjectName } into g
                        select new InboxHelper
                        {
                            Title = g.Key.Object,
                            Object = g.Key.ObjectName,
                            NoCount = g.Count(),
                            LastModDateTime = Convert.ToDateTime(g.Max(y => y.CreatedTimeStamp))
                        }).OrderByDescending(x => x.LastModDateTime).Take(100).ToList();

                //string search = (User.JuncUserRoles.First().RoleID == 1) ? string.Empty :
                //    ((User.JuncUserRoles.First().RoleID == 2) ? "<td>S</td>" :
                //    ((User.JuncUserRoles.First().RoleID == 3) ? "<td>P</td>" : "<td>R</td>"));

                //return (from x in db.BPEventLogs.ToList()
                //        where x.Status == Status && x.ObjectChanges != null && x.ObjectChanges.ToUpper().Contains(search.ToUpper())
                //        group x by new { x.Object, x.ObjectName } into g
                //        select new InboxHelper
                //        {
                //            Title = g.Key.Object,
                //            Object = g.Key.ObjectName,
                //            NoCount = g.Count(),
                //            LastModDateTime = Convert.ToDateTime(g.Max(y => y.CreatedTimeStamp))
                //        }).OrderByDescending(x => x.LastModDateTime).Take(100).ToList();
            }
            catch
            {
                return null;
            }
        }

        public List<InboxHelper> GetMailDetails(string Title, string Object)
        {
            try
            {
                return db.BPEventLogs.Where(x => x.Object == Title && x.ObjectName == Object).ToList()
                            .Join(db.MasterUsers.ToList(), x => x.CreatedBy, y => y.UserID, (x, y) => new InboxHelper()
                            {
                                Title = x.Object,
                                Object = x.ObjectName,
                                Detail = "<table class=\"table table-bordered\"><tr><th>Field</th><th>Old</th><th>New</th></tr>" + x.ObjectChanges.Trim() + "</table>",
                                ModifiedBy = y.UserName,
                                LastModDateTime = Convert.ToDateTime(x.CreatedTimeStamp)
                            }).OrderByDescending(x => x.LastModDateTime).ToList();
            }
            catch
            {
                return null;
            }
        }

        public List<InboxHelper> GetMailDetails(string Title, string Object, MasterUser User)
        {
            try
            {
                string Status = (User.JuncUserRoles.First().RoleID == 1) ? "A" : "I";

                List<string> lstsearch = GetSearchStrings(User);

                DateTime dtlimit = DateTime.Now.AddDays(30);
                var data = db.BPEventLogs.Where(x => x.Status == Status && x.Object == Title && x.ObjectName == Object && x.ObjectChanges != null); //&& x.CreatedTimeStamp > dtlimit
                List<BPEventLog> EventData = data.ToList().Where(x => IsStringExists(x.ObjectChanges, lstsearch)).ToList();
                
                int role = Convert.ToInt32(User.JuncUserRoles.First().RoleID);
                return EventData.Join(db.MasterUsers.ToList(), x => x.CreatedBy, y => y.UserID, 
                    (x, y) => new InboxHelper()
                            {
                                Title = x.Object,
                                Object = x.ObjectName,
                                Detail = "<table class=\"table table-bordered\"><tr><th>Field</th><th>Old</th><th>New</th></tr>" + StatusStringReplace(x.ObjectChanges.Trim(), role) + "</table>",
                                ModifiedBy = y.UserName,
                                LastModDateTime = Convert.ToDateTime(x.CreatedTimeStamp)
                            }).OrderByDescending(x => x.LastModDateTime).ToList();


                //string search = (User.JuncUserRoles.First().RoleID == 1) ? string.Empty :
                //    ((User.JuncUserRoles.First().RoleID == 2) ? "<td>S</td>" :
                //    ((User.JuncUserRoles.First().RoleID == 3) ? "<td>P</td>" : "<td>R</td>"));
                //int role = Convert.ToInt32(User.JuncUserRoles.First().RoleID);
                //return db.BPEventLogs.Where(x => x.Object == Title && x.ObjectName == Object && x.ObjectChanges.ToUpper().Contains(search.ToUpper())).ToList()
                //            .Join(db.MasterUsers.ToList(), x => x.CreatedBy, y => y.UserID, (x, y) => new InboxHelper()
                //            {
                //                Title = x.Object,
                //                Object = x.ObjectName,
                //                Detail = "<table class=\"table table-bordered\"><tr><th>Field</th><th>Old</th><th>New</th></tr>" + StatusStringReplace(x.ObjectChanges.Trim(), role) + "</table>",
                //                ModifiedBy = y.UserName,
                //                LastModDateTime = Convert.ToDateTime(x.CreatedTimeStamp)
                //            }).OrderByDescending(x => x.LastModDateTime).ToList();
            }
            catch
            {
                return null;
            }
        }

        private string StatusStringReplace(string Changes, int RoleID)
        {
            if (RoleID == 1)
                Changes = Changes.Replace("<td>A</td>", "<td>Active</td>").Replace("<td>D</td>", "<td>Inactive</td>");
            else
                Changes = Changes.Replace("<td>O</td>", "<td>Open</td>").Replace("<td>S</td>", "<td>Saved</td>").Replace("<td>P</td>", "<td>Prepared</td>")
                    .Replace("<td>R</td>", "<td>Reviewed</td>").Replace("<td>A</td>", "<td>Approved</td>")
                    .Replace("<td>X</td>", "<td>ReviewerRejected</td>").Replace("<td>Y</td>", "<td>ApproverRejected</td>");

            return Changes;
        }

        public string ObjectDifference(object first, object second)
        {
            if (first == null || second == null)
            {
                return string.Empty;
            }
            Type firstType = first.GetType();
            Type secondType = second.GetType();
            if (secondType != firstType)
            {
                //return string.Empty; 
            }

            string msg = string.Empty;
            foreach (PropertyInfo propInfo in firstType.GetProperties().Where(x => !x.GetMethod.IsVirtual))
            {
                if (propInfo.Name.ToUpper() != ("CreatedBy").ToUpper() && propInfo.Name.ToUpper() != ("CreatedTimeStamp").ToUpper()
                    && propInfo.Name.ToUpper() != ("ModifiedBy").ToUpper() && propInfo.Name.ToUpper() != ("ModifiedTimeStamp").ToUpper()
                    && propInfo.Name.ToUpper() != ("BudgetMengurusID").ToUpper() && propInfo.Name.ToUpper() != ("BudgetPerjawatanID").ToUpper())
                {
                    if (propInfo.CanRead)
                    {
                        object firstValue = propInfo.GetValue(first, null);
                        object secondValue = propInfo.GetValue(second, null);
                        string firstVal = (firstValue == null) ? string.Empty : firstValue.ToString().Trim();
                        string secondVal = (secondValue == null) ? string.Empty : secondValue.ToString().Trim();
                        if (!string.Equals(firstVal, secondVal))
                        {
                            msg = msg + "<tr><td>" + propInfo.Name + "</td><td>" + firstVal + "</td><td>" + secondVal + "</td></tr>";
                        }
                    }
                }
            }
            return msg;
        }
    }
}
