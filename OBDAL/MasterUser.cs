//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OBDAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class MasterUser
    {
        public MasterUser()
        {
            this.JuncUserRoles = new HashSet<JuncUserRole>();
            this.UserPerjawatanWorkflows = new HashSet<UserPerjawatanWorkflow>();
            this.UserSegDtlWorkflows = new HashSet<UserSegDtlWorkflow>();
            this.UserMengurusWorkflows = new HashSet<UserMengurusWorkflow>();
        }
    
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Passcode { get; set; }
        public int GroupID { get; set; }
        public string UserEmail { get; set; }
        public string SecQuestion { get; set; }
        public string SecAnswer { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedTimeStamp { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedTimeStamp { get; set; }
        public string ICNO { get; set; }
        public string Title { get; set; }
        public string FullName { get; set; }
        public string PositionGrade { get; set; }
        public string PhoneNO { get; set; }
        public string Fax { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string PeriodOfService { get; set; }
        public string OfficeAddress { get; set; }
        public string Status { get; set; }
        public string Language { get; set; }
    
        public virtual ICollection<JuncUserRole> JuncUserRoles { get; set; }
        public virtual MasterGroup MasterGroup { get; set; }
        public virtual ICollection<UserPerjawatanWorkflow> UserPerjawatanWorkflows { get; set; }
        public virtual ICollection<UserSegDtlWorkflow> UserSegDtlWorkflows { get; set; }
        public virtual ICollection<UserMengurusWorkflow> UserMengurusWorkflows { get; set; }
    }
}