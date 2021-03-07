using Akka.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AkkaServer.Core
{

    public static class HoconLoader
    {
        public static Config ParseConfig(string hoconPath)
        {
            return ConfigurationFactory.ParseString(File.ReadAllText(hoconPath));
        }
    }
}
