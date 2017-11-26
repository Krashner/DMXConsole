using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTD2XX_NET;
using System.Runtime.InteropServices;

namespace DMXConsole
{
    class DmxDriver
    {
        /*
        private const int SET_DMX_RX_MODE = 5;
        protected const int SET_DMX_TX_MODE = 6;
        private const byte DMX_START_CODE = 0x7E;
        protected const byte DMX_END_CODE = 0xE7;
        private const int DMX_PACKET_SIZE = 512;
        */
        public bool connected { get; private set; }
        public FTDI device { get; private set; }
        private int startAddr;

        private byte[] header, data, footer;

        public DmxDriver(int baseDmxAddr)
        {
            startAddr = baseDmxAddr;
            device = new FTDI();
        }

        ~DmxDriver()
        {
            device.Close();
        }

        public void CyclePort()
        {
            device.CyclePort();
        }

        public void PrintDeviceData()
        {
            Console.WriteLine();

            uint deviceID = 0;
            FTDI.FT_DEVICE ftDevice = new FTDI.FT_DEVICE();
            string des;

            device.GetDeviceID(ref deviceID);
            device.GetDeviceType(ref ftDevice);
            device.GetDescription(out des);

            Console.WriteLine("Device ID: " + deviceID);
            Console.WriteLine("Device Type: " + ftDevice);
            Console.WriteLine("Device Description: " + des);

            Console.WriteLine();
        }

        public void OpenPort()
        {
            //this opens the first dmx device it finds, maybe add command to specify
            FTDI.FT_STATUS result = device.OpenByIndex(0);
            if (result == FTDI.FT_STATUS.FT_OK && device.IsOpen)
            {
                device.SetTimeouts(5000, 5000);

                connected = true;
                Console.WriteLine("DMX Connected");

                header = new byte[4];
                data = new byte[513];
                footer = new byte[1] { 0xE7 };//end packet

                //header data
                header[0] = 0x7E;     //start packet
                header[1] = 06;       //tx mode
                header[2] = 01;       //???
                header[3] = 02;       //start code

                SendData();
            }
            else
            {
                connected = false;
                Console.WriteLine("DMX CONNECTION FAILED");
            }
        }

        public void ClosePort()
        {
            device.Close();
            Console.WriteLine("DMX Disconnected");
        }

        public bool deviceConnected()
        {
            return connected;
        }

        public void ChangeValue(int address, byte value)
        {
            if (address < 512)
            {
                data[address] = value;
            }
        }

        public void SendData()
        {
            FTDI.FT_STATUS result;
            uint bytes_written = 0;

            if (!device.IsOpen)
            {
                Console.WriteLine("Could not write to device, Port is closed");
                return;
            }

            byte[] packet = header.Concat(data.Concat(footer)).ToArray();
            result = device.Write(packet, packet.Length, ref bytes_written);
            if (result != FTDI.FT_STATUS.FT_OK)
            {
                Console.WriteLine("Could not write to device");
            }
        }

    }
}
