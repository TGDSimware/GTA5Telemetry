using System;
using log4net;
using System.Reflection;

namespace GTA5Reader
{
    public static class Logging
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static ILog Current
        {
            get
            {
                return Logging.Log;
            }
        }
    }
}
