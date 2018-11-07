using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonizingBastards.Base.MiscUtil
{
    static class IdHandler
    {

        private static long idCounter = 1;

        public static long GetUniqueId()
        {
            return idCounter++;
        }
    }
}
