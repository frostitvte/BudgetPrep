using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OBDAL;

namespace OBBAL
{
    public class SegmentBAL
    {
        public List<Segment> GetSegments()
        {
            return new SegmentDAL().GetSegments();
        }

        public bool InsertSegment(Segment objSegment)
        {
            try
            {
                return new SegmentDAL().InsertSegment(objSegment);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateSegment(Segment objSegment)
        {
            try
            {
                return new SegmentDAL().UpdateSegment(objSegment);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
