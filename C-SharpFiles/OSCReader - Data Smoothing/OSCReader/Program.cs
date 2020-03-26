using System;
using System.Net;
using Rug.Osc;
using System.Collections;
using System.Diagnostics;

namespace OSCReader
{
    class Program
    {
        static void StartLoop(OscReceiver oscReceiver)
        {
            // Create Moving Average objects for each data value (x, y, z, p, r, yw)
            //MovingAverage maX = new MovingAverage(5);
            //MovingAverage maY = new MovingAverage(5);
            //MovingAverage maZ = new MovingAverage(5);
            MovingAverage maP = new MovingAverage(5);
            MovingAverage maR = new MovingAverage(5);
            MovingAverage maYw = new MovingAverage(5, true);
            OscPacket packet = null;
            string[] extractData;
            const string START_LINE = "/position";
            const int MAX_TIME_ELAPSED = 10000;     // 10 seconds
            bool connectionNotification = true;
            Stopwatch sw = new Stopwatch();

            // Attempt to establish an OSC connection
            try
            {
                // Start stopwatch
                sw.Start();

                do
                {
                    if (oscReceiver.State == OscSocketState.Connected)
                    {
                        if (connectionNotification)
                        {
                            Console.WriteLine("connection established");
                            connectionNotification = false;
                        }
                        
                        // Attempt receive OSC packet
                        if (oscReceiver.TryReceive(out packet))
                        {
                            sw.Reset();
                            sw.Start();

                            // Extract position data from OSC packet
                            if (packet.ToString().StartsWith(START_LINE))
                            {
                                extractData = packet.ToString().Split(',');
                                //maX.SmoothOrientationData(extractData[2], Values.X);
                                //maY.SmoothOrientationData(extractData[3], Values.Y);
                                //maZ.SmoothOrientationData(extractData[4], Values.Z);
                                maP.SmoothOrientationData(extractData[5], Values.P);
                                maR.SmoothOrientationData(extractData[6], Values.R);
                                maYw.SmoothOrientationData(extractData[7], Values.Yw);
                            }
                        }
                    }

                } while (sw.ElapsedMilliseconds < MAX_TIME_ELAPSED);
                sw.Stop();
                Console.WriteLine("No OSC packets detected after {0} seconds", sw.ElapsedMilliseconds / 1000);
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
            Console.Write("Waiting for OSC connection...");

            StartLoop(oscReceiver);

            oscReceiver.Close();

            Console.WriteLine("OSC Connection closed.");

            Console.ReadKey();
        }
    }
}