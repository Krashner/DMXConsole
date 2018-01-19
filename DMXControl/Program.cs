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


            dmxControl = new DmxDriver(0);
            dmxControl.OpenPort();
            CommandParser commandParser = new CommandParser();

            //if port is closed, try to open every second
            while (!dmxControl.device.IsOpen)
            {
                dmxControl.OpenPort();
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }

          //  Pulse();

            while (true)
            {
                commandParser.Parse(Console.ReadLine(), dmxControl);
            }
        }

        public static void Pulse()
        {
            bool canChange = false;
            int i = 1;
            double a = 0.00, b = 0.00, c = 0.00, d = 0.00;
            while (true)
            {
                double t = ((Math.Sin(a += 0.01) * .5 + .5));
                if (t <= 0.01 && canChange)
                {
                    if (i < 4)
                        i++;
                    else
                        i = 1;
                    canChange = false;
                }

                if (t >= 0.99)
                    canChange = true;

                dmxControl.ChangeValue(i, (byte)(t * 255));
                dmxControl.SendData();
            }

            /*
            //color strength

            /*
            t = ((Math.Sin(b += 0.01) * .5 + .5)) * 255;

            dmxControl.ChangeValue(2, (byte)t);

            t = ((Math.Sin(c += 0.01) * .5 + .5)) * 255;
            dmxControl.ChangeValue(3, (byte)t);

            t = ((Math.Sin(d += 0.01) * .5 + .5)) * 255;
            dmxControl.ChangeValue(4, (byte)t);
            */
        }
    }
}
