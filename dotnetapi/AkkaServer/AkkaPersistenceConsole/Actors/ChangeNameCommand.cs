using System;
using System.Collections.Generic;
using System.Text;

namespace AkkaPersistenceConsole.Actors
{
    public class ChangeNameCommand
    {
        public ChangeNameCommand(long id, string reName)
        {
            Id = id;
            ReName = reName;
        }

        public long Id { get; }

        public string ReName { get; }
    }


    public class ChangeNameEvent
    {
        public ChangeNameEvent(long id, string reName)
        {
            Id = id;
            ReName = reName;
        }

        public long Id { get; }

        public string ReName { get; }
    }

}
