﻿using System;
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
            redChannels = new int[0];
            greenChannels = new int[0];
            blueChannels = new int[0];
            whiteChannels = new int[0];

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
            int address = 0, value = 0;

            switch (args[0].ToLower())
            {
                case "write":
                    if (int.TryParse(args[1], out address))
                    {
                        if (int.TryParse(args[2], out value))
                        {
                            dmxControl.ChangeValue(address, (byte)value);
                        }
                        else {
                            Console.WriteLine("Invalid Input");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid Input");
                    }; break;
                case "range": WriteRange(args[1], dmxControl); break;
                case "set":
                    if (args.Length > 1)
                        switch (args[1].ToLower())
                        {
                            case "range": SetRange(args); break;
                            case "red":
                            case "r": SetChannel(args, "Red", ref redChannels); break;
                            case "green":
                            case "g": SetChannel(args, "Green", ref greenChannels); break;
                            case "blue":
                            case "b": SetChannel(args, "Blue", ref blueChannels); break;
                            case "white":
                            case "w": SetChannel(args, "White", ref whiteChannels); break;
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
                    if (!WriteChannel(redChannels, args[1], dmxControl)) break;
                    break;
                case "green":
                case "g":
                    if (!FullCommand(args)) break;
                    if (!WriteChannel(greenChannels, args[1], dmxControl)) break;
                    break;
                case "blue":
                case "b":
                    if (!FullCommand(args)) break;
                    if (!WriteChannel(blueChannels, args[1], dmxControl)) break;
                    break;
                case "white":
                case "w":
                    if (!FullCommand(args)) break;
                    if (!WriteChannel(whiteChannels, args[1], dmxControl)) break;
                    break;
                case "all":
                    if (!FullCommand(args)) break;
                    if (WriteAll(args[1], dmxControl)) break;//broke
                    break;
                case "channels": ShowChannels(); break;
                case "clear": Console.Clear(); break;
                case "device": dmxControl.PrintDeviceData(); break;
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
            Console.WriteLine("     Write:                  write [address] [value]");
            Console.WriteLine("     Set Range:              set range [start] [end]");
            Console.WriteLine("     Write Range:            range [start] [end]");
            Console.WriteLine("     View Channels:          channels");
            Console.WriteLine("     Set Channels:           set [channel] [address]");
            Console.WriteLine("     Write Channels:         [channel] [value]");
            Console.WriteLine("     Toggle Channel:         [channel] [on | off]");
            Console.WriteLine("     Write All:              all [value]");
            Console.WriteLine("     Toggle All:             all [on | off]");
            Console.WriteLine("     Clear Console:          clear");
            Console.WriteLine("******************************************************************");
        }


        private void ShowChannels()
        {
            Console.WriteLine("******************************************************************");
            Console.Write("Red: ");
            for (int i = 0; i < redChannels.Length; i++)
            { Console.Write(redChannels[i] + " "); }
            Console.WriteLine();

            Console.Write("Green: ");
            for (int i = 0; i < greenChannels.Length; i++)
            { Console.Write(greenChannels[i] + " "); }
            Console.WriteLine();

            Console.Write("Blue: ");
            for (int i = 0; i < blueChannels.Length; i++)
            { Console.Write(blueChannels[i] + " "); }
            Console.WriteLine();

            Console.Write("White: ");
            for (int i = 0; i < whiteChannels.Length; i++)
            { Console.Write(whiteChannels[i] + " "); }
            Console.WriteLine();
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
            return true;

        }

        //clamp val between min and max
        public int Clamp(int val, int min, int max)
        {
            if (val > max) val = max;
            if (val < min) val = min;
            return val;
        }

        //set the channel addresses
        private void SetChannel(string[] args, string channelName, ref int[] channelArray)
        {
            channelArray = new int[args.Length - 2];
            Console.Write("Channel " + channelName + ": ");

            for (int i = 2; i < args.Length; i++)
            {
                int val = 0;
                if (int.TryParse(args[i], out val))
                {
                    channelArray[i - 2] = val;
                    Console.Write(val + " ");
                }
                else
                {
                    Console.WriteLine("Invalid Channel Input");
                    return;
                }
            }

            Console.WriteLine();
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
            return true;
        }
    }
}
