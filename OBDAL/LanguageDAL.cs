using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBDAL
{
    public class LanguageDAL
    {
        OBEntities db = new OBEntities();
        public IQueryable<BPLanguage> GetLanguages()
        {
            return db.BPLanguages.Select(x => x);
        }
    }
}
