using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHandler : MonoBehaviour
{
    public LayerMask layersToHit; // the layers are going to be allowed to hit
    public GameManager gameManager; // a reference to our game manager

    // Start is called before the first frame update
    void Start()
    {
        GetMouseInput();
    }

    /// <summary>
    /// Gets input from the player to mouse/tap on screen
    /// </summary>
    void GetMouseInput()
    {
        if(Input.GetMouseButtonDown(0)) // primary mouse button input
        {
            RaycastHit hit; // data stored based on what we hit
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // draws a ray from camera to mouse position

            // do ouy raycast, if hit something it blocks the ray & stores the data
            if(Physics.Raycast(ray, out hit, layersToHit))
            {
                gameManager.SpawnOrMoveSoccerball(hit.point); // the point in the world where the ray has hit, spawn our soccer ball or move it
            }
        }
    }
}
