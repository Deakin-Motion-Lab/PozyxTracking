using System;
using System.Net;
using Rug.Osc;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Pozyx_HeightData
{
    class Program
    {
        static void StartLoop(OscReceiver oscReceiver)
        {
            // Variables
            OscPacket packet = null;
            HeightData height_Z = new HeightData();
            StringBuilder sb = new StringBuilder();
            Stopwatch sw = new Stopwatch();
            string[] extractData;
            const string START_LINE = "/position";
            bool connectionNotification = true;
            int count = 0;
            const int CYCLES = 200;

            // Attempt to establish an OSC connection
            try
            {
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
                            // Extract position data from OSC packet
                            if (packet.ToString().StartsWith(START_LINE))
                            {
                                extractData = packet.ToString().Split(',');
                                int zValue = int.Parse(extractData[4]);    // Extract 'z' position value (height)
                                Console.WriteLine("{4} - Position:\tx:{0}, y:{1}, z:{2}, \tZ_ERROR:{3} ]", extractData[2], extractData[3], extractData[4], height_Z.GetError(zValue), count);
                                sb.AppendLine(string.Format("{0},{1},{2}", extractData[2], extractData[3], extractData[4]));
                                count++;
                            }
                        }
                    }

                } while (count < CYCLES);
                sw.Stop();
                Console.WriteLine("TOTAL COUNT: " + count);
                Console.WriteLine("TOTAL TIME: " + sw.ElapsedMilliseconds / 1000);
                Console.WriteLine(count / (sw.ElapsedMilliseconds / 1000) + " Hz");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("Error average: {0}", height_Z.GetErrorAverage());
            FILE_IO.WriteLog(sb.ToString());
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