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
    private float _QuartX;
    private float _QuartY;
    private float _QuartZ;
    private float _QuartW;
    private Quaternion _QT;
    private DataSmooth _AVG_X;
    private DataSmooth _AVG_Y;
    private DataSmooth _AVG_Z;
    private DataSmooth _AVG_QtX;
    private DataSmooth _AVG_QtY;
    private DataSmooth _AVG_QtZ;
    private DataSmooth _AVG_QtW;
    private const int _POSITION_SMOOTHING = 2;       // Adjust as necessary (1 = No sampling, > 1 = increased smoothing)
    private const int _ORIENTATION_SMOOTHING = 3;    // Adjust as necessary (1 = No sampling, > 1 = increased smoothing)

    // Use this for initialization
    void Start ()
    {
        _OSCReader = new OSCReader();
        _AVG_X = new DataSmooth(_POSITION_SMOOTHING);
        _AVG_Y = new DataSmooth(_POSITION_SMOOTHING);
        _AVG_Z = new DataSmooth(_POSITION_SMOOTHING);
        _AVG_QtX = new DataSmooth(_ORIENTATION_SMOOTHING);
        _AVG_QtY = new DataSmooth(_ORIENTATION_SMOOTHING);
        _AVG_QtZ = new DataSmooth(_ORIENTATION_SMOOTHING);
        _AVG_QtW = new DataSmooth(_ORIENTATION_SMOOTHING);
    }
	
	// Update is called once per frame
	void Update ()
    {
        ExtractData();
        MoveGameObject();
    }

    /// <summary>
    /// Obtain smoothed position (x, y, z) and orientation (x, y, z, w) data
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
        _QuartX = _AVG_QtX.SmoothOrientationData(_OSCReader.Orientation(Values.Qt_X));
        _QuartY = _AVG_QtY.SmoothOrientationData(_OSCReader.Orientation(Values.Qt_Y));
        _QuartZ = _AVG_QtZ.SmoothOrientationData(_OSCReader.Orientation(Values.Qt_Z));
        _QuartW = _AVG_QtW.SmoothOrientationData(_OSCReader.Orientation(Values.Qt_W));

        Debug.Log(string.Format("Position: [x:{0} y:{1} z:{2}], Rotation: [x:{3:f2} y:{4:f2} z:{5:f2} w:{6:f2}]", _XCoord, _YCoord, _ZCoord, _QuartX, _QuartY, _QuartZ, _QuartW));
    }

    /// <summary>
    /// Move the game object to the position and orientation given by the Pozyx device
    /// </summary>
    private void MoveGameObject()
    {
        // convert mm values to m values for Unity
        cube.transform.position = new Vector3(_XCoord / 1000.0f, _YCoord / 1000.0f, _ZCoord / 1000.0f);
        cube.transform.rotation = new Quaternion(_QuartX, _QuartY, _QuartZ, _QuartW);
    }
}
