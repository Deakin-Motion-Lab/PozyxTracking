using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TagCollision : MonoBehaviour
{
    public GameObject pozyxTag;
    private Color tagColour;
    private HueBridgeConnection hueBridgeConnection;

    private void Start()
    {
        tagColour = new Color32(5, 75, 20, 255);
        hueBridgeConnection = new HueBridgeConnection();
        //hueBridgeConnection.LightsOn();
        hueBridgeConnection.LightsNormal();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Pozyx Tags")
        {
            pozyxTag.GetComponent<Renderer>().material.color = Color.red;
            hueBridgeConnection.LightsRed();
        }
        else if (collision.collider.tag == "Walls")
        {
            pozyxTag.GetComponent<Renderer>().material.color = Color.blue;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag == "Pozyx Tags"  || collision.collider.tag == "Walls")
        {
            pozyxTag.GetComponent<Renderer>().material.color = tagColour;
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            //do
            //{

            //} while (sw.ElapsedMilliseconds < 3000);
            //sw.Stop();

            hueBridgeConnection.LightsNormal();
        }
}
}
