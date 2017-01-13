
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OBDAL;


namespace OBBAL
{
    public class UserBAL
    {
        public MasterUser GetValidUser(string UserName, string Password)
        {
            return new UserDAL().GetValidUser(UserName, Password);
        }

        public string GetSecQuestion(string UserName)
        {
            return new UserDAL().GetSecQuestion(UserName);
        }

        public MasterUser VerifyAnswer(string UserName, string Answer)
        {
            return new UserDAL().VerifyAnswer(UserName, Answer);
        }

        public bool ResetPassword(string UserName, string Password)
        {
            return new UserDAL().ResetPassword(UserName, Password);
        }

        public bool SaveSecurityInfo(string UserName, string Question, string Answer)
        {
            return new UserDAL().SaveSecurityInfo(UserName, Question, Answer);
        }

        public bool ChangeLanguage(MasterUser AuthUser, string Language)
        {
            return new UserDAL().ChangeLanguage(AuthUser, Language);
        }

        public List<MasterUser> GetUsers()
        {
            return new UserDAL().GetUsers().ToList();
        }

        public bool InsertUser(MasterUser objMasterUser, JuncUserRole objUserRole, List<UserMengurusWorkflow> lstAccountCode, List<UserPerjawatanWorkflow> lstServiceCode, List<UserSegDtlWorkflow> lstSegDtls)
        {
            return new UserDAL().InsertUser(objMasterUser, objUserRole, lstAccountCode, lstServiceCode, lstSegDtls);
        }

        public bool UpdateUser(MasterUser objMasterUser, JuncUserRole objUserRole, List<UserMengurusWorkflow> lstAccountCode, List<UserPerjawatanWorkflow> lstServiceCode, List<UserSegDtlWorkflow> lstSegDtls)
        {
            return new UserDAL().UpdateUser(objMasterUser, objUserRole, lstAccountCode, lstServiceCode, lstSegDtls);
        }

        public List<MasterRole> GetRoles()
        {
            return new UserDAL().GetRoles();
        }

        public List<MasterGroup> GetGroups()
        {
            return new UserDAL().GetGroups();
        }
    }
}
