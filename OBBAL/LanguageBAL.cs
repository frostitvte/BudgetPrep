using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OBDAL;

namespace OBBAL
{
    public class LanguageBAL
    {
        public IQueryable<BPLanguage> GetLanguages()
        {
            return new LanguageDAL().GetLanguages();
        }
    }
}
