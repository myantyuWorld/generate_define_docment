using log4net;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]

namespace generate_define_docment.Model
{
    class AppBase
    {
        public static ILog log = LogManager.GetLogger(typeof(Program));

    }
}