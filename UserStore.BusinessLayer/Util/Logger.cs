using log4net;
using log4net.Config;

namespace UserStore.BusinessLayer.Util
{
    public static class Logger
    {
        private static readonly ILog log = LogManager.GetLogger(typeof (Logger));

        public static ILog Log
        {
            get { return log; }
        }

        public static void InitLogger()
        {
            XmlConfigurator.Configure();
        }
    }
}
