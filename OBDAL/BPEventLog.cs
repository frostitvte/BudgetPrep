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
    
    public partial class BPEventLog
    {
        public int EventLogID { get; set; }
        public string Object { get; set; }
        public string ObjectName { get; set; }
        public string ObjectChanges { get; set; }
        public string EventMassage { get; set; }
        public string Status { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedTimeStamp { get; set; }
    }
}