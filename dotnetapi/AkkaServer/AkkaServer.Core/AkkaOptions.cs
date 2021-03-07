using System;
using System.Collections.Generic;
using System.Text;

namespace AkkaServer.Core
{
    public class AkkaOptions
    {
        public string Configfile { get; set; }

        public bool IsDocker { get; set; }

        public string Name { get; set; }

    }
}
