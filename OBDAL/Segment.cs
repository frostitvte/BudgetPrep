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
    
    public partial class Segment
    {
        public Segment()
        {
            this.SegmentDetails = new HashSet<SegmentDetail>();
        }
    
        public int SegmentID { get; set; }
        public string SegmentName { get; set; }
        public Nullable<int> SegmentOrder { get; set; }
        public string ShapeFormat { get; set; }
        public string Status { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedTimeStamp { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedTimeStamp { get; set; }
    
        public virtual ICollection<SegmentDetail> SegmentDetails { get; set; }
    }
}
