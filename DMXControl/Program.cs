using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace DMXConsole
{
    class Program
    {

        static DmxDriver dmxControl;

        static void Main(string[] args)
        {

            //  Random rand = new Random();
            // double a = 0.00, b = 0.00, c = 0.00, d = 0.00;

            dmxControl = new DmxDriver(0);
            dmxControl.OpenPort();
            CommandParser commandParser = new CommandParser();

            //for(int t = 0; t < 512; t++)
            //dmxControl.ChangeValue(t, 255);

            //dmxControl.SendData();


            while (true)
            {
                /*
                //color strength
                double t = ((Math.Sin(a += 0.01) * .5 + .5)*.5+.5) * 255;
                dmxControl.ChangeValue(361, (byte)t);
                
                t = ((Math.Sin(b += 0.02) * .5 + .5) * .5 + .5) * 255;
                dmxControl.ChangeValue(362, (byte)t);
                
                t = ((Math.Sin(c += 0.03) * .5 + .5) * .5 + .5) * 255;
                dmxControl.ChangeValue(363, (byte)t);

                t = ((Math.Sin(d += 0.04) * .5 + .5) * .5 + .5) * 255;
                dmxControl.ChangeValue(364, (byte)t);
                */

                commandParser.Parse(Console.ReadLine(), dmxControl);
            }

        }
    }
}
