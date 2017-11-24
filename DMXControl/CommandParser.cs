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

        public CommandParser()
        {
            Console.WriteLine("----------------------------------------------------------------------------------");
            UpdateData();
        }

        public void UpdateData()
        {
            Console.WriteLine("Mode: " + mode + " | Range: " + rangeStart + " " + rangeEnd + " | Red: " + red + ", Green: " + green + ", Blue: " + blue + ", White: " + white);
        }


        public void Parse(string input, DmxDriver dmxControl)
        {
            string[] args = input.Split(' ');
            //to do, remove blanks
            int channel = 0;

            switch (args[0].ToLower())
            {
                case "range":
                    if (args.Length < 2)
                    {
                        Console.WriteLine("Enter Valid Range");
                        return;
                    }
                    if (int.TryParse(args[1], out rangeStart))
                    {
                        if (args.Length < 3)
                            rangeEnd = rangeStart;
                        else if (int.TryParse(args[2], out rangeEnd))
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
                        if (args[1].ToLower() == "all")
                        {
                            rangeStart = 1;
                            rangeEnd = 512;
                        }
                        else
                            Console.WriteLine("Invalid Start Range");
                    }

                    break;
                case "set":
                    if (args.Length > 1)
                        switch (args[1].ToLower())
                        {
                            case "red":
                            case "r": SetChannels(args, ref redChannels); break;
                            case "green":
                            case "g": SetChannels(args, ref greenChannels); break;
                            case "blue":
                            case "b": SetChannels(args, ref blueChannels); break;
                            case "white":
                            case "w": SetChannels(args, ref whiteChannels); break;
                        }
                    break;
                case "mode":
                    if (!FullCommand(args)) break;
                    if (args[1].ToLower() == "inclusive" || args[1].ToLower() == "i")
                        mode = "Inclusive";
                    else if (args[1].ToLower() == "exclusive" || args[1].ToLower() == "e")
                        mode = "Exclusive";
                    break;
                case "red":
                case "r":
                    if (!FullCommand(args)) break;
                    if (!SetColor(channel = 1, args[1], dmxControl, ref red)) break;
                    SetOff(channel, dmxControl);
                    break;
                case "green":
                case "g":
                    if (!FullCommand(args)) break;
                    if (!SetColor(channel = 2, args[1], dmxControl, ref green)) break;
                    SetOff(channel, dmxControl);
                    break;
                case "blue":
                case "b":
                    if (!FullCommand(args)) break;
                    if (!SetColor(channel = 3, args[1], dmxControl, ref blue)) break;
                    SetOff(channel, dmxControl);
                    break;
                case "white":
                case "w":
                    if (!FullCommand(args)) break;
                    if (!SetColor(channel = 4, args[1], dmxControl, ref white)) break;
                    SetOff(channel, dmxControl);
                    break;
                case "all":
                    if (!FullCommand(args)) break;
                    if (!SetColor(channel = 1, args[1], dmxControl, ref red)) break;
                    if (!SetColor(channel = 2, args[1], dmxControl, ref green)) break;
                    if (!SetColor(channel = 3, args[1], dmxControl, ref blue)) break;
                    if (!SetColor(channel = 4, args[1], dmxControl, ref white)) break;
                    break;
                case "clear": Console.Clear(); break;
                case "help":
                case "?":
                case "/h":
                    HelpCommand(); break;
                case "quit": Environment.Exit(0); break;
                case "open":
                    dmxControl.OpenPort();
                    Console.WriteLine("Port Opened");
                    break;
                case "close":
                    dmxControl.ClosePort();
                    Console.WriteLine("Port Closed");
                    return;
                default:
                    Console.WriteLine("Invalid Input");
                    return;
            }
            dmxControl.SendData();
            UpdateData();
        }

        private void HelpCommand()
        {
            Console.WriteLine("******************************************************************");
            Console.WriteLine("     Colors: RED GREEN BLUE WHITE | Values: 0-255");
            Console.WriteLine("     Set Channel Range:      range [start] [end]");
            Console.WriteLine("     Set Color Channels:     set [color] [channel]");
            Console.WriteLine("     Change light:           [color] [value]");
            Console.WriteLine("     Toggle Light:           [color] [on | off]");
            Console.WriteLine("     Change All Lights:      all [value]");
            Console.WriteLine("     Toggle All Lights:      all [on | off]");
            Console.WriteLine("     Toggle Mode:            mode [i/inclusive | e/exclusive]");
            Console.WriteLine("     Clear Console:          clear");
            Console.WriteLine("******************************************************************");
        }


        private bool FullCommand(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Invalid Input");
                return false;
            }
            return true;
        }

        private void SetChannels(string[] args, ref int[] channelArray)
        {
            channelArray = new int[args.Length - 2];
            for (int i = 2; i < args.Length; i++)
            {
                int val = 0;
                if (int.TryParse(args[i], out val))
                    channelArray[i - 2] = val;
                else
                {
                    Console.WriteLine("Invalid Channel Input");
                    return;
                }
            }
        }

        private void SetOff(int mask, DmxDriver dmxControl)
        {
            if (mode == "Inclusive")
                return;

            if (mask != 1)
                SetColor(1, "0", dmxControl, ref red);
            if (mask != 2)
                SetColor(2, "0", dmxControl, ref green);
            if (mask != 3)
                SetColor(3, "0", dmxControl, ref blue);
            if (mask != 4)
                SetColor(4, "0", dmxControl, ref white);
        }

        public int Clamp(int val, int min, int max)
        {
            if (val > max) val = max;
            if (val < min) val = min;
            return val;
        }

        public bool SetColor(int channel, string input, DmxDriver dmxControl, ref int value)
        {
            int tempVal = 0;
            for (int i = rangeStart; i <= rangeEnd; i++)
            {
                if (int.TryParse(input, out tempVal))
                {
                    value = Clamp(tempVal, 0, 255);
                    dmxControl.ChangeValue(i, (byte)value);
                }
                else if (input.ToLower() == "on")
                {
                    dmxControl.ChangeValue(i, 255);//channel
                    value = 255;
                }
                else if (input.ToLower() == "off")
                {
                    dmxControl.ChangeValue(i, 0);
                    value = 0;
                }
                else
                {
                    Console.WriteLine("Invalid Input");
                    return false;
                }
            }
            return true;
        }
    }
}
