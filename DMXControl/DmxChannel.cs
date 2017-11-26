using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMXConsole
{

    class DmxGroup
    {
        public string Name { get; set; }
        public List<int> Channels { get; set; }

        public DmxGroup()
        {
            Name = "Default";
            Channels = new List<int>();
        }

    }
}
