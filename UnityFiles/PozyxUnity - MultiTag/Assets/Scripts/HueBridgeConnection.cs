using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Q42;
using Q42.HueApi;
using Q42.HueApi.Converters;
using Q42.HueApi.Interfaces;
using System.Net.Http;
using System.Net;
//using System.Diagnostics;
//using System.Windows.Forms;
using UnityEngine;

class HueBridgeConnection
{
    // Private and Read-only Attributes
    public string _appKey = "86TgDCSTTzMnHIXWbXtOoZYvstf78Q3X4n9FsTmL";    // work 
                                                                           //private string _appKey = "C6Wo7ygzCdLWL9gQ2g6zEuhw4q5WuDWa5wVPZz4d";      // home
    private string _deviceName = "OurHomeApp";
    private string _deviceType = "PaulsPC";
    private LocalHueClient _client;
    private LightCommand _command;
    private bool _connectedToHueBridge = false;
    private bool _lightsOn = false;
    private string _IPAddress = "10.0.0.2";     // work
                                                //private string _IPAddress = "192.168.1.59";     // home



    // Custom Constructor
    public HueBridgeConnection()
    {
        ConnectToHueBridge();
        _command = new LightCommand();
    }

    private async void GetKey()
    {
        _client = new LocalHueClient(_IPAddress);
        _appKey = await _client.RegisterAsync(_deviceName, _deviceType);

        if (!string.IsNullOrEmpty(_appKey))
        {
            Console.WriteLine("Success: your key is {0}", _appKey);
            //FILE_IO.Write(appKey, _deviceName, _deviceType);
        }
        else
        {
            Console.WriteLine("Error: unable to get key");
        }
    }

    /// <summary>
    /// Connects to Hue Bridge, with supplied IP address and authorised user key (app key)
    /// </summary>
    private async void ConnectToHueBridge()
    {
        // Hue Bridge IP address
        _client = new LocalHueClient(_IPAddress);

        // Establish Connection and output
        _client.Initialize(_appKey);
        _connectedToHueBridge = await _client.CheckConnection();
        Debug.Log(string.Format("Hue Bridge Connected @ {1} = {0}", _connectedToHueBridge, _IPAddress));
    }

    public void LightsOn()
    {
        // Turn lights on
        _command.On = true;
        _client.SendCommandAsync(_command);
    }

    public void LightsOff()
    {
        // Turn lights off
        _command.On = false;
        _client.SendCommandAsync(_command);
    }

    public void LightsRed()
    {
        _command.Hue = 65000;
        _client.SendCommandAsync(_command);
    }

    public void LightsBlue()
    {
        _command.Hue = 40000;
        _client.SendCommandAsync(_command);
    }

    public void LightsNormal()
    {
        _command.Hue = 9000;
        _client.SendCommandAsync(_command);
    }


    public void LightAlert(bool alert)
    {
        if (alert)
        {
            _command.Hue = 65000;
            _command.Alert = Alert.Once;

        }
        else
        {
            _command.Hue = 9000;
            _command.Alert = Alert.None;
        }

        _client.SendCommandAsync(_command);
    }
}

