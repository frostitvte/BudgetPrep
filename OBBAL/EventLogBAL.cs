using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OBDAL;

namespace OBBAL
{
    public class EventLogBAL
    {
        public bool AddEventLog(BPEventLog objEventLog)
        {
            return new EventLogDAL().AddEventLog(objEventLog);
        }

        public List<InboxHelper> GetInboxList(MasterUser User)
        {
            return new EventLogDAL().GetInboxList(User);
        }

        public List<InboxHelper> GetMailDetails(string Title, string Object)
        {
            return new EventLogDAL().GetMailDetails(Title, Object);
        }

        public List<InboxHelper> GetMailDetails(string Title, string Object, MasterUser User)
        {
            return new EventLogDAL().GetMailDetails(Title, Object, User);
        }
    }
}
