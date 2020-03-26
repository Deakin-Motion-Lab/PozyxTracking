/*
   This class moves the position and orientation of the Game Object based on smoothed data values (x, y, z, p, r, yaw)
   streamed into Unity from Pozyx / Python via OSC

   Written by Paul Hammond, 8/11/2018 - 16/11/2018
*/

using UnityEngine;

public class Position : MonoBehaviour
{
    // Public and Private, Read-only Attributes
    public GameObject cube;
    private OSCReader _OSCReader;
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
    public int Position_Smoothing;        // Adjust as necessary (1 = No sampling, > 1 = increased smoothing)
    public int Orientation_Smoothing;     // Adjust as necessary (1 = No sampling, > 1 = increased smoothing)

    // Use this for initialization
    void Start ()
    {
        _OSCReader = new OSCReader();
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
        // Get next OSC Packet
        _OSCReader.GetOSCPacket();

        // Position Data
        _XCoord = _AVG_X.SmoothPositionData(_OSCReader.Position(Values.X));
        // z and y axis reversed for unity reference frame
        _YCoord = _AVG_Y.SmoothPositionData(_OSCReader.Position(Values.Z));
        _ZCoord = _AVG_Z.SmoothPositionData(_OSCReader.Position(Values.Y));

        // Rotation Data
        _Pitch = _AVG_Pitch.SmoothOrientationData(_OSCReader.Orientation(Values.P));
        _Roll = _AVG_Roll.SmoothOrientationData(_OSCReader.Orientation(Values.R));
        _Yaw = _AVG_Yaw.SmoothOrientationData(_OSCReader.Orientation(Values.Yaw)) - 90;    // Rotate 90 deg to compensate for Unity Reference Frame / Pozyx difference

        //Debug.Log(string.Format("Position: [x:{0} y:{1} z:{2}], Rotation: [p:{3:f2} r:{4:f2} y:{5:f2}]", _XCoord, _YCoord, _ZCoord, _Pitch, _Roll, _Yaw));
        Debug.Log(string.Format("Max: [x:{0} y:{1} z:{2}], Min: [x:{3} y:{4} z:{5}]]", _AVG_X.GetMax(), _AVG_Y.GetMax(), _AVG_Z.GetMax(), _AVG_X.GetMin(), _AVG_Y.GetMin(), _AVG_Z.GetMin()));

        // Convert Euler (Rotation) Angles to Quaternions 
        _QT = Quaternion.Euler(_Roll, _Yaw, -_Pitch);
    }

    /// <summary>
    /// Move the game object to the position and orientation given by the Pozyx device
    /// </summary>
    private void MoveGameObject()
    {
        // convert mm values to m values for Unity
        cube.transform.position = new Vector3(_XCoord / 1000.0f, _YCoord / 1000.0f, _ZCoord / 1000.0f);
        cube.transform.rotation = _QT;
    }
}
