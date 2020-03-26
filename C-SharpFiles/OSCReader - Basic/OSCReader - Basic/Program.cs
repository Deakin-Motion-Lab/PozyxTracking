/*
   Basic OSC (Open Sound Control) reader to demonstrate how to read OSC packets transmitted via Pozyx devices on to the 
   local host (127.0.0.1:8888)

   The Tag ID, Position (x, y, z) and Orientation (p, r, yaw) are outputted to console in real-time

   NOTE: includes Nuget package Rug.Osc
   https://bitbucket.org/rugcode/rug.osc

   Written by Paul Hammond 7/11/2019
*/

using System;
using System.Net;
using Rug.Osc;
using System.Globalization;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace OSCReader
{
    class Program
    {
        static void StartLoop(OscReceiver oscReceiver)
        {
            OscPacket packet = null;
            string[] extractData = null;
            const string START_LINE = "/position";

            // Attempt to establish an OSC connection
            try
            {
                do
                {
                    if (oscReceiver.State == OscSocketState.Connected)
                    {
                        // Attempt receive OSC packet
                        packet = oscReceiver.Receive();

                        // Extract position data from OSC packet
                        if (packet.ToString().StartsWith(START_LINE))
                        {
                            extractData = packet.ToString().Split(',');
                        }

                        foreach (string dataItem in extractData)
                        {
                            Console.Write(dataItem + "\t");

                        }
                        Console.Write("TagID: " + Convert.ToInt16(extractData[1]).ToString("X"));  // Converts decimal (26400) into hexadecimal (6270)
                        Console.WriteLine();
                    }
                } while (true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static void Main(string[] args)
        {
            IPAddress IPaddress = IPAddress.Parse("127.0.0.1");
            int port = 8888;

            OscReceiver oscReceiver = new OscReceiver(IPaddress, port);

            oscReceiver.Connect();
            Console.Write("Waiting for OSC connection...\n\n");

            StartLoop(oscReceiver);

            oscReceiver.Close();

            Console.WriteLine("OSC Connection closed.");

            Console.ReadKey();
        }
    }
}