using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagCollision : MonoBehaviour
{
    public GameObject pozyxTag;
    public GameObject Cylinder;
    private Color32 tagColour;

    // Initialise Default Tag Colour (Dark Green)
    private void Start()
    {
        tagColour = new Color32(5, 75, 20, 255);
    }
    
    // Change tag colour when collision with another object occurs
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Obstacles")
        {
            pozyxTag.GetComponent<Renderer>().material.color = Color.red;
            //Cylinder.GetComponent<Renderer>().material.color = Color.yellow;
        }
        else if (collision.collider.tag == "PlaneArea")
        {
            pozyxTag.GetComponent<Renderer>().material.color = Color.blue;
        }
    }

    // Reset tag colour when collision stops
    private void OnCollisionExit(Collision collision)
    {
        pozyxTag.GetComponent<Renderer>().material.color = tagColour;
        
    }
}
