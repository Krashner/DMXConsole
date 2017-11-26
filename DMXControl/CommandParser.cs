using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMXConsole
{
    class CommandParser
    {
        private string mode = "Inclusive";
        private int rangeStart = 1, rangeEnd = 512, red, green, blue, white;
        private int[] redChannels, greenChannels, blueChannels, whiteChannels;
        private XmlManager<DmxGroup> xmlManager;
        private List<DmxGroup> dmxGroups;


        public CommandParser()
        {
            redChannels = new int[0];
            greenChannels = new int[0];
            blueChannels = new int[0];
            whiteChannels = new int[0];

            dmxGroups = new List<DmxGroup>();

            Console.WriteLine("----------------------------------------------------------------------------------");
            UpdateData();
        }

        public void UpdateData()
        {
            // Console.WriteLine("Range: " + rangeStart + "-" + rangeEnd);
            // Console.WriteLine("Mode: " + mode + " | Range: " + rangeStart + " " + rangeEnd + " | Red: " + red + ", Green: " + green + ", Blue: " + blue + ", White: " + white);
        }


        public void Parse(string input, DmxDriver dmxControl)
        {
            string[] args = input.Split(' ');
            //to do, remove blanks
            //move all write commands to case write. cant currently write custom groups
            //make command to display range
            switch (args[0].ToLower())
            {
                case "write":
                    if (args.Length > 1)
                        switch (args[1].ToLower())
                        {
                            case "1":// channel, NEED TO GET INT
                                if (!FullCommand(args, 3)) break;
                                Write(args, dmxControl); break;
                            case "range":
                                if (!FullCommand(args, 3)) break;
                                WriteRange(args[2], dmxControl); break;
                            case "all":
                                if (!FullCommand(args, 3)) break;
                                WriteAll(args[2], dmxControl); break;
                            default: Console.WriteLine("Invalid Input"); break;
                        }
                    else
                    {
                        Console.WriteLine("Invalid Input");
                    }
                    break;
                case "set":
                    if (args.Length > 1)
                        switch (args[1].ToLower())
                        {
                            case "range": SetRange(args); break;
                            case "group": SetChannel(args); break;
                            default: Console.WriteLine("Invalid Input"); break;
                        }
                    else
                    {
                        Console.WriteLine("Invalid Input");
                    }
                    break;
                case "mode":
                    if (!FullCommand(args, 2)) break;
                    if (args[1].ToLower() == "inclusive" || args[1].ToLower() == "i")
                        mode = "Inclusive";
                    else if (args[1].ToLower() == "exclusive" || args[1].ToLower() == "e")
                        mode = "Exclusive";
                    break;
                case "range": Console.WriteLine("Range: " + rangeStart + "-" + rangeEnd); break;
                case "groups": ShowChannels(); break;
                case "clear": Console.Clear(); break;
                case "device": dmxControl.PrintDeviceData(); break;
                case "help":
                case "?":
                case "/h": HelpCommand(); break;
                case "quit": Environment.Exit(0); break;
                case "open":
                    dmxControl.OpenPort();
                    Console.WriteLine("Port Opened");
                    break;
                case "close":
                    dmxControl.ClosePort();
                    Console.WriteLine("Port Closed");
                    return;
                default: Console.WriteLine("Invalid Input"); return;
            }
            //UpdateData();
        }

        private void HelpCommand()
        {
            Console.WriteLine("******************************************************************");
            Console.WriteLine("     Write:                  write [address] [value | on | off]");
            Console.WriteLine("     Write All:              write all [value | on | off]");
            Console.WriteLine("     View Range:             range");
            Console.WriteLine("     Set Range:              set range [start] [end]");
            Console.WriteLine("     Write Range:            range [start] [end]");
            Console.WriteLine("     View Groups:            groups");
            Console.WriteLine("     Set Group:              set [group] [address]");
            Console.WriteLine("     Write Group:            write [group] [value]");
            Console.WriteLine("     Clear Console:          clear");
            Console.WriteLine("******************************************************************");
        }


        private void ShowChannels()
        {
            Console.WriteLine("******************************************************************");
            for (int i = 0; i < dmxGroups.Count; i++)
            {
                Console.Write(dmxGroups[i].Name + " ");
                for (int n = 0; n < dmxGroups[i].Channels.Count; n++)
                    Console.Write(dmxGroups[i].Channels[n] + " ");
                Console.WriteLine();
            }
            Console.WriteLine("******************************************************************");
        }

        private bool FullCommand(string[] args, int length)
        {
            if (args.Length < length)
            {
                Console.WriteLine("Invalid Input");
                return false;
            }
            return true;
        }

        private void SetRange(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Enter Valid Range");
                return;
            }
            if (int.TryParse(args[2], out rangeStart))
            {
                if (args.Length < 4)
                    rangeEnd = rangeStart;
                else if (int.TryParse(args[3], out rangeEnd))
                {
                    if (rangeStart > rangeEnd)
                    {
                        //maybe just check when changing value and swap there
                        //make sure start is lower than end, if not then it swaps them
                        int temp = rangeStart;
                        rangeStart = rangeEnd;
                        rangeEnd = temp;
                    }
                    if (rangeEnd > 512) rangeEnd = 512;
                    if (rangeStart < 1) rangeEnd = 1;
                }
                else
                    Console.WriteLine("Invalid End Range");
            }
            else
            {
                if (args[2].ToLower() == "all")
                {
                    rangeStart = 1;
                    rangeEnd = 512;
                }
                else
                    Console.WriteLine("Invalid Start Range");
            }

            Console.WriteLine("Range: " + rangeStart + "-" + rangeEnd);
            Console.WriteLine();
        }

        //set the channel addresses
        private void SetChannel(string[] args)
        {
            DmxGroup dmxGroup = new DmxGroup();
            dmxGroup.Name = args[2];

            Console.Write("Group " + dmxGroup.Name + ": ");

            for (int i = 3; i < args.Length; i++)
            {
                int val = 0;
                if (int.TryParse(args[i], out val))
                {
                    dmxGroup.Channels.Add(val);
                    Console.Write(val + " ");
                }
                else
                {
                    Console.WriteLine("Invalid Channel Input");
                    return;
                }
            }
            dmxGroups.Add(dmxGroup);
            Console.WriteLine();
        }

        private void Write(string[] args, DmxDriver dmxControl)
        {
            //arg 0 is base command, 1 is address, 2 is value
            int address = 0, value = 0;
            if (int.TryParse(args[1], out address))
            {
                if (int.TryParse(args[2], out value))
                {
                    dmxControl.ChangeValue(address, (byte)value);
                }
                else if (args[2].ToLower() == "on")
                {
                    dmxControl.ChangeValue(address, 255);//channel
                }
                else if (args[2].ToLower() == "off")
                {
                    dmxControl.ChangeValue(address, 0);
                }
                else
                {
                    Console.WriteLine("Invalid Input");
                }
            }
            dmxControl.SendData();
        }

        private bool WriteRange(string input, DmxDriver dmxControl)
        {
            int tempVal = 0;
            for (int i = rangeStart; i <= rangeEnd; i++)
            {
                if (int.TryParse(input, out tempVal))
                {
                    tempVal = Clamp(tempVal, 0, 255);
                    dmxControl.ChangeValue(i, (byte)tempVal);
                }
                else if (input.ToLower() == "on")
                {
                    dmxControl.ChangeValue(i, 255);//channel
                    tempVal = 255;
                }
                else if (input.ToLower() == "off")
                {
                    dmxControl.ChangeValue(i, 0);
                    tempVal = 0;
                }
                else
                {
                    Console.WriteLine("Invalid Input");
                    return false;
                }
            }
            dmxControl.SendData();
            return true;
        }

        private bool WriteAll(string input, DmxDriver dmxControl)
        {
            int tempVal = 0;
            for (int i = 1; i < 512; i++)
            {
                if (int.TryParse(input, out tempVal))
                {
                    tempVal = Clamp(tempVal, 0, 255);
                    dmxControl.ChangeValue(i, (byte)tempVal);
                }
                else if (input.ToLower() == "on")
                {
                    dmxControl.ChangeValue(i, 255);//channel
                    tempVal = 255;
                }
                else if (input.ToLower() == "off")
                {
                    dmxControl.ChangeValue(i, 0);
                    tempVal = 0;
                }
                else
                {
                    Console.WriteLine("Invalid Input");
                    return false;
                }
            }
            dmxControl.SendData();
            return true;
        }

        //write to the channel
        public bool WriteChannel(int[] colorChannel, string input, DmxDriver dmxControl)
        {
            int tempVal = 0;
            for (int i = 0; i < colorChannel.Length; i++)
            {
                if (int.TryParse(input, out tempVal))
                {
                    tempVal = Clamp(tempVal, 0, 255);
                    dmxControl.ChangeValue(colorChannel[i], (byte)tempVal);
                }
                else if (input.ToLower() == "on")
                {
                    dmxControl.ChangeValue(colorChannel[i], 255);//channel
                    tempVal = 255;
                }
                else if (input.ToLower() == "off")
                {
                    dmxControl.ChangeValue(colorChannel[i], 0);
                    tempVal = 0;
                }
                else
                {
                    Console.WriteLine("Invalid Input");
                    return false;
                }
            }
            dmxControl.SendData();
            return true;
        }

        //clamp val between min and max
        public int Clamp(int val, int min, int max)
        {
            if (val > max) val = max;
            if (val < min) val = min;
            return val;
        }
    }
}
