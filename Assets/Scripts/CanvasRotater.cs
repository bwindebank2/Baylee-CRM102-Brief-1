using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasRotater : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Vector3 relativePosition = transform.position - Camera.main.transform.position; // get relative direciton of the canvas to player camera
        relativePosition.y = 0; // zero out the y axis as we don't want it to rotate on that axis.
        Quaternion rotation = Quaternion.LookRotation(relativePosition); // create rotation to look at the direction we want.
        transform.rotation = rotation; // set our new rotation to our current rotation/
    }
}
