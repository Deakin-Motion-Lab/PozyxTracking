/*
   This static class establishes an OSC (Open Sound Control) Protocol connection on the local host to enable communciation between
   the Pozyx Tags (Unity GameObjects) and the Pozyx hardware system through a Python-based script (multitagOSC.py).

   Written by Paul Hammond, 14/11/2018 - 22/11/2018
*/

using Rug.Osc;
using UnityEngine;
using System;
using System.Net;

public static class OSC
{
    // Private Read-only Attributes
    private static IPAddress _IPaddress = IPAddress.Parse("127.0.0.1");
    private static int _Port = 8888;
    private static OscReceiver _OSCReceiver = new OscReceiver(_IPaddress, _Port);

    /// <summary>
    /// Receives the next available OSC packet relative to the Pozyx TagID,
    /// splits the csv data values, and stores into a string array
    /// </summary>
    public static string[] GetOSCPacket(string tagID)
    {
        OscPacket packet;
        string[] extractData = null;
        const string START_LINE = "/position";
        bool gotCorrectPacket = false;

        // Attempt to establish an OSC connection and receive packet
        try
        {
            // Connect to OSC
            _OSCReceiver.Connect();

            do
            {
                if (_OSCReceiver.State == OscSocketState.Connected)
                {
                    // Attempt to receive OSC packet
                    packet = _OSCReceiver.Receive();

                    // Extract data from OSC packet
                    if (packet.ToString().StartsWith(START_LINE))
                    {
                        extractData = packet.ToString().Split(',');

                        // Multi-Tag Functionality 
                        // Checks packet "TagID" matches GameObject "TagID" (in Hexadecimal)
                        if (Convert.ToInt32(extractData[1]).ToString("X") == tagID)
                        {
                            gotCorrectPacket = true;
                        }
                    }
                }
            } while (!gotCorrectPacket);

            // Close OSC
            _OSCReceiver.Close();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

        return extractData;
    }
}

