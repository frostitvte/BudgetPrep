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
    
    public partial class BudgetProjek
    {
        public BudgetProjek()
        {
            this.JuncBgtProjekSegDtls = new HashSet<JuncBgtProjekSegDtl>();
        }
    
        public int BudgetProjekID { get; set; }
        public int PeriodMengurusID { get; set; }
        public string AccountCode { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedTimeStamp { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedTimeStamp { get; set; }
    
        public virtual AccountCode AccountCode1 { get; set; }
        public virtual PeriodMenguru PeriodMenguru { get; set; }
        public virtual ICollection<JuncBgtProjekSegDtl> JuncBgtProjekSegDtls { get; set; }
    }
}
