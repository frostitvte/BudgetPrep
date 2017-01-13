using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OBSecurity;

namespace OBDAL
{
    public class UserDAL
    {
        OBEntities db = new OBEntities();

        public MasterUser GetValidUser(string UserName, string Password)
        {
            try
            {
                string encstr = Security.Encrypt(Password);
                return db.MasterUsers.Where(x => x.Status == "A" && x.UserName == UserName && x.Passcode == encstr).FirstOrDefault();
            }
            catch (Exception ex)
            {
                 throw ex;
            }
        }

        public string GetSecQuestion(string UserName)
        {
            MasterUser user = db.MasterUsers.Where(x => x.Status == "A" && x.UserName == UserName).FirstOrDefault();

            if (user == null) return null;
            if (user.SecQuestion == null) return string.Empty;

            return user.SecQuestion;
        }

        public MasterUser VerifyAnswer(string UserName, string Answer)
        {
            string pwd = Security.Encrypt(Answer.ToUpper());
            return db.MasterUsers.Where(x => x.Status == "A" && x.UserName == UserName && x.SecAnswer.Trim() == pwd).FirstOrDefault();
        }

        public bool ResetPassword(string UserName, string Password)
        {
            MasterUser user = db.MasterUsers.Where(x => x.UserName == UserName).FirstOrDefault();
            try
            {
                string encstr = Security.Encrypt(Password);
                user.Passcode = encstr;
                user.ModifiedBy = user.UserID;
                user.ModifiedTimeStamp = DateTime.Now;
                db.SaveChanges();

                BPEventLog bpe = new BPEventLog();
                bpe.Object = "User - Reset Password";
                bpe.ObjectName = user.UserName;
                bpe.ObjectChanges = string.Empty;
                bpe.EventMassage = "Success";
                bpe.Status = "A";
                bpe.CreatedBy = user.ModifiedBy;
                bpe.CreatedTimeStamp = user.ModifiedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);
            }
            catch
            {
                BPEventLog bpe = new BPEventLog();
                bpe.Object = "User - Reset Password";
                bpe.ObjectName = user.UserName;
                bpe.ObjectChanges = string.Empty;
                bpe.EventMassage = "Failure";
                bpe.Status = "A";
                bpe.CreatedBy = user.ModifiedBy;
                bpe.CreatedTimeStamp = user.ModifiedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                return false;
            }

            return true;
        }

        public bool SaveSecurityInfo(string UserName, string Question, string Answer)
        {
            MasterUser user = db.MasterUsers.Where(x => x.UserName == UserName).FirstOrDefault();
            try
            {
                string encAnswer = Security.Encrypt(Answer.ToUpper());
                user.SecQuestion = Question;
                user.SecAnswer = encAnswer;
                user.ModifiedBy = user.UserID;
                user.ModifiedTimeStamp = DateTime.Now;
                db.SaveChanges();

                BPEventLog bpe = new BPEventLog();
                bpe.Object = "User - Security Info";
                bpe.ObjectName = user.UserName;
                bpe.ObjectChanges = string.Empty;
                bpe.EventMassage = "Success";
                bpe.Status = "A";
                bpe.CreatedBy = user.ModifiedBy;
                bpe.CreatedTimeStamp = user.ModifiedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);
            }
            catch
            {
                BPEventLog bpe = new BPEventLog();
                bpe.Object = "User - Security Info";
                bpe.ObjectName = user.UserName;
                bpe.ObjectChanges = string.Empty;
                bpe.EventMassage = "Failure";
                bpe.Status = "A";
                bpe.CreatedBy = user.ModifiedBy;
                bpe.CreatedTimeStamp = user.ModifiedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                return false;
            }

            return true;
        }

        public bool ChangeLanguage(MasterUser AuthUser, string Language)
        {
            MasterUser user = db.MasterUsers.Where(x => x.UserID == AuthUser.UserID).FirstOrDefault();
            string oldLang = user.Language;
            try
            {
                user.Language = Language;
                db.SaveChanges();

                BPEventLog bpe = new BPEventLog();
                bpe.Object = "User - Language";
                bpe.ObjectName = user.UserName;
                bpe.ObjectChanges = string.Empty;
                    //new EventLogDAL().ObjectDifference(user, AuthUser);
                bpe.EventMassage = "Success";
                bpe.Status = "A";
                bpe.CreatedBy = user.ModifiedBy;
                bpe.CreatedTimeStamp = user.ModifiedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);
            }
            catch
            {
                BPEventLog bpe = new BPEventLog();
                bpe.Object = "User - Security Info";
                bpe.ObjectName = user.UserName;
                bpe.ObjectChanges = string.Empty;
                bpe.EventMassage = "Failure";
                bpe.Status = "A";
                bpe.CreatedBy = user.ModifiedBy;
                bpe.CreatedTimeStamp = user.ModifiedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                return false;
            }

            return true;
        }

        public List<MasterUser> GetUsers()
        {
            return db.MasterUsers.Select(x => x).OrderBy(x => x.UserName).ToList() ;
        }

        public bool InsertUser(MasterUser objMasterUser, JuncUserRole objUserRole, List<UserMengurusWorkflow> lstAccountCode, List<UserPerjawatanWorkflow> lstServiceCode, List<UserSegDtlWorkflow> lstSegDtls)
        {
            try
            {
                objMasterUser.Passcode = Security.Encrypt(objMasterUser.Passcode);

                db.MasterUsers.Add(objMasterUser);
                db.JuncUserRoles.Add(objUserRole);

                foreach (UserMengurusWorkflow o in lstAccountCode)
                    db.UserMengurusWorkflows.Add(o);

                foreach (UserPerjawatanWorkflow o in lstServiceCode)
                    db.UserPerjawatanWorkflows.Add(o);

                foreach (UserSegDtlWorkflow o in lstSegDtls)
                    db.UserSegDtlWorkflows.Add(o);

                db.SaveChanges();

                BPEventLog bpe = new BPEventLog();
                bpe.Object = "User - New User";
                bpe.ObjectName = objMasterUser.UserName;
                string mw = (lstAccountCode.Count == 0) ? string.Empty : lstAccountCode.Select(x => x.AccountCode).Aggregate((x, y) => x + "," + y);
                string pw = (lstServiceCode.Count == 0) ? string.Empty : lstServiceCode.Select(x => x.GroupPerjawatanCode).Aggregate((x, y) => x + "," + y);
                bpe.ObjectChanges = "<tr><td>Mengurus Workflow</td><td>New</td><td>" + mw + "</td></tr><tr><td>Perjawatan Workflow</td><td>New</td><td>" + pw + "</td></tr>";
                bpe.EventMassage = "Success";
                bpe.Status = "A";
                bpe.CreatedBy = objMasterUser.CreatedBy;
                bpe.CreatedTimeStamp = objMasterUser.CreatedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                return true;
            }
            catch (Exception ex)
            {
                BPEventLog bpe = new BPEventLog();
                bpe.Object = "User - New User";
                bpe.ObjectName = objMasterUser.UserName;
                bpe.ObjectChanges = string.Empty;
                bpe.EventMassage = "Failure";
                bpe.Status = "A";
                bpe.CreatedBy = objMasterUser.CreatedBy;
                bpe.CreatedTimeStamp = objMasterUser.CreatedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                throw ex;
            }
        }

        public bool UpdateUser(MasterUser objMasterUser, JuncUserRole objUserRole, List<UserMengurusWorkflow> lstAccountCode, List<UserPerjawatanWorkflow> lstServiceCode, List<UserSegDtlWorkflow> lstSegDtls)
        {
            MasterUser objuser = db.MasterUsers.Where(x => x.UserID == objMasterUser.UserID).FirstOrDefault();
            string changes = new EventLogDAL().ObjectDifference(objuser, objMasterUser);
            string rolechange = (db.JuncUserRoles.Where(x => x.UserID == objMasterUser.UserID).FirstOrDefault().RoleID == objUserRole.RoleID) ?
                string.Empty : "<tr><td>RoleID</td><td>" + db.JuncUserRoles.Where(x => x.UserID == objMasterUser.UserID).FirstOrDefault().RoleID + "</td><td>"
                + objUserRole.RoleID + "</td></tr>";
            try
            {
                if (objuser != null)
                {
                    objuser.UserName = objMasterUser.UserName;
                    objuser.GroupID = objMasterUser.GroupID;
                    objuser.UserEmail = objMasterUser.UserEmail;
                    objuser.ICNO = objMasterUser.ICNO;
                    objuser.Title = objMasterUser.Title;
                    objuser.FullName = objMasterUser.FullName;
                    objuser.PositionGrade = objMasterUser.PositionGrade;
                    objuser.PhoneNO = objMasterUser.PhoneNO;
                    objuser.Fax = objMasterUser.Fax;
                    objuser.Designation = objMasterUser.Designation;
                    objuser.Department = objMasterUser.Department;
                    objuser.PeriodOfService = objMasterUser.PeriodOfService;
                    objuser.OfficeAddress = objMasterUser.OfficeAddress;
                    objuser.Status = objMasterUser.Status;
                    objuser.ModifiedBy = objMasterUser.ModifiedBy;
                    objuser.ModifiedTimeStamp = objMasterUser.ModifiedTimeStamp;

                    JuncUserRole userrole = db.JuncUserRoles.Where(x => x.Status == "A" && x.UserID == objMasterUser.UserID).FirstOrDefault();
                    userrole.RoleID = objUserRole.RoleID;

                    string mwo = (objuser.UserMengurusWorkflows.Count() == 0) ? string.Empty : objuser.UserMengurusWorkflows.ToList().Select(x => x.AccountCode).Aggregate((x, y) => x + "," + y);
                    string pwo = (objuser.UserPerjawatanWorkflows.Count() == 0) ? string.Empty : objuser.UserPerjawatanWorkflows.ToList().Select(x => x.GroupPerjawatanCode).Aggregate((x, y) => x + "," + y);
                    string swo = (objuser.UserSegDtlWorkflows.Count() == 0) ? string.Empty : objuser.UserSegDtlWorkflows.ToList().Select(x => x.SegmentDetailID.ToString()).Aggregate((x, y) => x + "," + y);
                    string mw = (lstAccountCode.Count == 0) ? string.Empty : lstAccountCode.Select(x => x.AccountCode).Aggregate((x, y) => x + "," + y);
                    string pw = (lstServiceCode.Count == 0) ? string.Empty : lstServiceCode.Select(x => x.GroupPerjawatanCode).Aggregate((x, y) => x + "," + y);
                    string sw = (lstSegDtls.Count == 0) ? string.Empty : lstSegDtls.Select(x => x.SegmentDetailID.ToString()).Aggregate((x, y) => x + "," + y);
                    
                    string wochanges = string.Empty;
                    if(mwo != mw)
                        wochanges = wochanges + "<tr><td>Mengurus Workflow</td><td>" + mwo + "</td><td>" + mw + "</td></tr>";
                    if (pwo != pw)
                        wochanges = wochanges + "<tr><td>Perjawatan Workflow</td><td>" + pwo + "</td><td>" + pw + "</td></tr>";
                    if (swo != sw)
                        wochanges = wochanges + "<tr><td>SegmentDetails Workflow</td><td>" + swo + "</td><td>" + sw + "</td></tr>";

                    foreach (UserMengurusWorkflow o in db.UserMengurusWorkflows.Where(x => x.UserID == objMasterUser.UserID).ToList())
                        db.UserMengurusWorkflows.Remove(o);
                    foreach (UserPerjawatanWorkflow o in db.UserPerjawatanWorkflows.Where(x => x.UserID == objMasterUser.UserID).ToList())
                        db.UserPerjawatanWorkflows.Remove(o);
                    foreach (UserSegDtlWorkflow o in db.UserSegDtlWorkflows.Where(x => x.UserID == objMasterUser.UserID).ToList())
                        db.UserSegDtlWorkflows.Remove(o);

                    foreach (UserMengurusWorkflow o in lstAccountCode)
                        db.UserMengurusWorkflows.Add(new UserMengurusWorkflow()
                        {
                            AccountCode = o.AccountCode,
                            UserID = objMasterUser.UserID,
                            Status = "A"
                        });
                    foreach (UserPerjawatanWorkflow o in lstServiceCode)
                        db.UserPerjawatanWorkflows.Add(new UserPerjawatanWorkflow()
                        {
                            GroupPerjawatanCode = o.GroupPerjawatanCode,
                            UserID = objMasterUser.UserID,
                            Status = "A"
                        });
                    foreach (UserSegDtlWorkflow o in lstSegDtls)
                        db.UserSegDtlWorkflows.Add(new UserSegDtlWorkflow()
                        {
                            SegmentDetailID = o.SegmentDetailID,
                            UserID = objMasterUser.UserID,
                            Status = "A"
                        });

                    db.SaveChanges();

                    BPEventLog bpe = new BPEventLog();
                    bpe.Object = "User - Updated";
                    bpe.ObjectName = objMasterUser.UserName;
                    //changes = changes + ((rolechange == string.Empty) ? string.Empty : ", " + rolechange) + ((wochanges == string.Empty) ? string.Empty : ", " + wochanges);
                    changes = changes + rolechange + wochanges;
                    bpe.ObjectChanges = changes;
                    bpe.EventMassage = "Success";
                    bpe.Status = "A";
                    bpe.CreatedBy = objMasterUser.ModifiedBy;
                    bpe.CreatedTimeStamp = objMasterUser.ModifiedTimeStamp;
                    new EventLogDAL().AddEventLog(bpe);
                }
                return true;
            }
            catch (Exception ex)
            {
                BPEventLog bpe = new BPEventLog();
                bpe.Object = "User - Updated";
                bpe.ObjectName = objMasterUser.UserName;
                bpe.ObjectChanges = string.Empty;
                bpe.EventMassage = "Failure";
                bpe.Status = "A";
                bpe.CreatedBy = objMasterUser.ModifiedBy;
                bpe.CreatedTimeStamp = objMasterUser.ModifiedTimeStamp;
                new EventLogDAL().AddEventLog(bpe);

                throw ex;
            }
        }

        public List<MasterRole> GetRoles()
        {
            return db.MasterRoles.Select(x => x).ToList();
        }

        public List<MasterGroup> GetGroups()
        {
            return db.MasterGroups.Select(x => x).ToList();
        }
    }
}
