/*
   This class establishes an OSC (Open Sound Control) Protocol connection on the local host to enable communciation between
   the Unity Project and the Pozyx system through a Python-based script.

   Written by Paul Hammond, 14/11/2018
*/

using UnityEngine;
using Rug.Osc;
using System;
using System.Net;

public class OSCReader
{
    // Private Read-only Attributes
    private IPAddress _IPaddress = IPAddress.Parse("127.0.0.1");
    private const int _PORT = 8888;
    private OscReceiver _OSCReceiver;
    private Position _TagPosition;
    private string[] _OSCPacket;

    // Custom Constructor
    public OSCReader()
    {
        _OSCReceiver = new OscReceiver(_IPaddress, _PORT);
        _OSCReceiver.Connect();
    }

    #region Methods
    /// <summary>
    /// Receives the next available OSC packet, splits the csv data values, and stores into a string array
    /// </summary>
    public void GetOSCPacket()
    {
        try
        {
            if (_OSCReceiver.State == OscSocketState.Connected)
            {
                OscPacket packet = _OSCReceiver.Receive();
                _OSCPacket = packet.ToString().Split(',');
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            _OSCReceiver.Close();
        }
    }

    /// <summary>
    /// Extracts a single-value position data (x, y, or z) from an OSC packet and returns an integer value
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public int Position(Values value)
    {
        int positionValue = 0;

        // Extract appropriate value from OSC packet
        switch (value)
        {
            case Values.X:
                positionValue = int.Parse(_OSCPacket[2]);
                break;
            case Values.Y:
                positionValue = int.Parse(_OSCPacket[3]);
                break;
            case Values.Z:
                positionValue = int.Parse(_OSCPacket[4]);
                break;

        }

        return positionValue;
    }

    /// <summary>
    /// Extracts a single-value orientation data (pitch, roll, or yaw) from an OSC packet and returns an float value
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public float Orientation(Values value)
    {
        const char FLOAT_SYMBOL = 'f';
        float orientationValue = 0;

        // Extract appropriate value from OSC packet
        switch (value)
        {
            case Values.P:
                orientationValue = float.Parse(_OSCPacket[5].TrimEnd(FLOAT_SYMBOL));
                break;
            case Values.R:
                orientationValue = float.Parse(_OSCPacket[6].TrimEnd(FLOAT_SYMBOL));
                break;
            case Values.Yaw:
                orientationValue = float.Parse(_OSCPacket[7].TrimEnd(FLOAT_SYMBOL));
                break;

        }

        return orientationValue;
    }
    #endregion
}

