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
    
    public partial class MasterGroup
    {
        public MasterGroup()
        {
            this.MasterUsers = new HashSet<MasterUser>();
        }
    
        public int GroupID { get; set; }
        public string GroupName { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedTimeStamp { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedTimeStamp { get; set; }
        public string Status { get; set; }
    
        public virtual ICollection<MasterUser> MasterUsers { get; set; }
    }
}
