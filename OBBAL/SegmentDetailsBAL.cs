using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OBDAL;

namespace OBBAL
{
    public class SegmentDetailsBAL
    {
        public IQueryable<SegmentDetail> GetSegmentDetails()
        {
            return new SegmentDetailsDAL().GetSegmentDetails();
        }

        public List<SegmentDetail> GetSegmentDetails(int SegmentID)
        {
            return new SegmentDetailsDAL().GetSegmentDetails(SegmentID);
        }

        public bool IsBudgetEditable(List<int> SegmentDetailIDs)
        {
            return new SegmentDetailsDAL().IsBudgetEditable(SegmentDetailIDs);
        }

        public bool InsertSegmentDetail(SegmentDetail objSegment)
        {
            try
            {
                return new SegmentDetailsDAL().InsertSegmentDetail(objSegment);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateSegmentDetail(SegmentDetail objSegment)
        {
            try
            {
                return new SegmentDetailsDAL().UpdateSegmentDetail(objSegment);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
