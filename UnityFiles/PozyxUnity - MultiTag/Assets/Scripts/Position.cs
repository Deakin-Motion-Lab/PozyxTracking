/*
   This class moves the position and orientation of the Pozyx Tag (GameObject) based on smoothed data values (x, y, z, p, r, yaw)
   streamed into Unity from Pozyx / Python via OSC

   Written by Paul Hammond, 8/11/2018 - 22/11/2018
*/

using UnityEngine;
using System;

public class Position : MonoBehaviour
{
    // Public and Private, Read-only Attributes
    public GameObject PozyxTAG;
    public string TagID;        // Must be in Hexadecimal notation
    private int _XCoord;
    private int _YCoord;
    private int _ZCoord;
    private float _Pitch;
    private float _Roll;
    private float _Yaw;
    private Quaternion _QT;
    private DataSmooth _AVG_X;
    private DataSmooth _AVG_Y;
    private DataSmooth _AVG_Z;
    private DataSmooth _AVG_Pitch;
    private DataSmooth _AVG_Roll;
    private DataSmooth _AVG_Yaw;
    private string[] _PozyxData;
    
    public int Position_Smoothing = 2;       // Adjust as necessary (1 = No sampling, > 1 = increased smoothing)
    public int Orientation_Smoothing = 2;    // Adjust as necessary (1 = No sampling, > 1 = increased smoothing)

    // Use this for initialization
    void Start ()
    {
        _AVG_X = new DataSmooth(Position_Smoothing);
        _AVG_Y = new DataSmooth(Position_Smoothing);
        _AVG_Z = new DataSmooth(Position_Smoothing);
        _AVG_Pitch = new DataSmooth(Orientation_Smoothing);
        _AVG_Roll = new DataSmooth(Orientation_Smoothing);
        _AVG_Yaw = new DataSmooth(Orientation_Smoothing);
    }
	
	// Update is called once per frame
	void Update ()
    {
        ExtractData();
        MoveGameObject();
    }

    /// <summary>
    /// Obtain smoothed position (x, y, z) and orientation (p, r, y) data
    /// </summary>    
    private void ExtractData()
    {
        const char REMOVE_F = 'f';
        
        // Get next OSC Packet for this GameObject (Pozyx Tag)
        _PozyxData = OSC.GetOSCPacket(TagID);

        // Position Data
        _XCoord = _AVG_X.SmoothPositionData(Convert.ToInt32(_PozyxData[2]));
        // z and y axis reversed for unity reference frame
        _YCoord = _AVG_Y.SmoothPositionData(Convert.ToInt32(_PozyxData[4]));
        _ZCoord = _AVG_Z.SmoothPositionData(Convert.ToInt32(_PozyxData[3]));

        // Orientation Data
        _Pitch = _AVG_Pitch.SmoothOrientationData(float.Parse(_PozyxData[5].TrimEnd(REMOVE_F)));
        _Roll = _AVG_Roll.SmoothOrientationData(float.Parse(_PozyxData[6].TrimEnd(REMOVE_F)));
        _Yaw = _AVG_Yaw.SmoothOrientationData(float.Parse(_PozyxData[7].TrimEnd(REMOVE_F))) - 90;    // Rotate 90 deg to compensate for Unity Reference Frame / Pozyx difference

        Debug.Log(string.Format("TagID: [{6}] Position: [x:{0} y:{1} z:{2}], Rotation: [p:{3:f2} r:{4:f2} y:{5:f2}]", _XCoord, _YCoord, _ZCoord, _Pitch, _Roll, _Yaw, TagID));
        //Debug.Log(string.Format("Position: [x:{0} y:{1} z:{2}]", _XCoord, _YCoord, _ZCoord));

        // Convert Euler (Rotation) Angles to Quaternions 
        _QT = Quaternion.Euler(_Roll, _Yaw, -_Pitch);
    }

    /// <summary>
    /// Move the game object to the position and orientation given by the Pozyx device
    /// </summary>
    private void MoveGameObject()
    {
        // convert mm values to m values for Unity
        PozyxTAG.transform.position = new Vector3(_XCoord / 1000.0f, _YCoord / 1000.0f, _ZCoord / 1000.0f);
        PozyxTAG.transform.rotation = _QT;
    }
}