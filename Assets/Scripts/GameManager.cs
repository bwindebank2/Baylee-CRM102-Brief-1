using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform soccerField; // a reference to our soccer field
    public Vector3 moveArea; // the size of our play area
    public Transform arCamera; // a reference to an AR Camera

    public GameObject soccerballPrefab; // a reference to the soccer ball in our scene
    public GameObject currentSoccerBallInstance; // the current soccerball that has been spawned in
    public Transform aRContentParent;

    public int playerOneScore;
    public int playerTwoScore;

    public UIManager uiManager; // a reference to our ui manager
    public AudioManager audioManager;

    private bool areCharactersRunningAway = false; // are there any characters currently running away
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("new random position of" + ReturnRandomPositionOnField());
        playerOneScore = 0;
        playerTwoScore = 0;
        uiManager.DisplayScores(false); // hide our canvases to start with
        uiManager.UpdateScores(playerOneScore, playerOneScore); // update players scores
    }

    /// <summary>
    /// Increase the passed in player score by 1
    /// </summary>
    /// <param name="playerNumber"></param>
    public void IncreasePlayerScore(int playerNumber)
    {
        if(playerNumber == 1)
        {
            playerOneScore++;
        }
        else if(playerNumber == 2)
        {
            playerTwoScore++;
        }
        ResetSoccerBall();
        uiManager.UpdateScores(playerOneScore, playerTwoScore); // updates the ui scores to display our current values

    }

    /// <summary>
    /// Resets the balls position and velocities
    /// </summary>
    private void ResetSoccerBall()
    {
        currentSoccerBallInstance.GetComponent<Rigidbody>().velocity = Vector3.zero; // reset the ball velocity
        currentSoccerBallInstance.GetComponent<Rigidbody>().angularVelocity = Vector3.zero; // reset the ball velocity
        currentSoccerBallInstance.transform.position = ReturnRandomPositionOnField(); // reset ball position
    }

    /// <summary>
    /// Returns a random position within our move area
    /// </summary>
    /// <returns></returns>
    public Vector3 ReturnRandomPositionOnField()
    {
        float xPosition = Random.Range(-moveArea.x/2, moveArea.x/2); // gives us a random number between negative moveArea x and positive moveArea x
        float yPosition = soccerField.position.y; // our soccer fields y transform positions
        float zPosition = Random.Range(-moveArea.z / 2, moveArea.z / 2);

        return new Vector3(xPosition, yPosition, zPosition);
    }

    /// <summary>
    /// Debug function that lets us draw objects in our scene but it's not viewable in play mode
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if(soccerField == null)
        {
            return;
        }
        Gizmos.color = Color.red;
        Gizmos.DrawCube(soccerField.position + new Vector3(0,.01f,0), moveArea); // draws a cube at the soccer fields position at the size of the move area
    }

    /// <summary>
    /// returns true or false if we are too close, or not close enough to AR camera
    /// </summary>
    /// <param name="character"></param>
    /// <param name="distanceThreshold"></param>
    /// <returns></returns>
    public bool IsPlayerTooCloseToCharacter(Transform character, float distanceThreshold)
    {
        if(Vector3.Distance(arCamera.position,character.position)<= distanceThreshold)
        {
            // returns true if we are too close
            return true;
        }
        else
        {
            return false;
            // returns false if we are too far away
        }
    }

    /// <summary>
    /// spawns in a new soccer ball based on the position provided
    /// if a soccer ball already exists in the world, we just want to move it
    /// </summary>
    /// <param name="positionToSpawn"></param>
    public void SpawnOrMoveSoccerball(Vector3 positionToSpawn)
    {
        if(soccerballPrefab == null)
        {
            Debug.Log("No Soccer Ball Prefab Assigned");
            return;
        }

        // if soccer ball not spawned in yet
        if (currentSoccerBallInstance == null)
        {
            // spawn in and store reference to our soccer ball, parent it to our AR content parent
            currentSoccerBallInstance = Instantiate(soccerballPrefab, positionToSpawn, soccerballPrefab.transform.rotation, aRContentParent);
            currentSoccerBallInstance.GetComponent<Rigidbody>().velocity = Vector3.zero; // sets the velocity of the ball to zero
            currentSoccerBallInstance.GetComponent<Rigidbody>().angularVelocity = Vector3.zero; // sets the angular velocity of the ball to zero
            AlertCharacters(); // tell everyone ball is spawned
        }
        else
        {
            // if the soccer ball already exists, let's move it
            currentSoccerBallInstance.transform.position = positionToSpawn; // move soccer ball back to position
            currentSoccerBallInstance.GetComponent<Rigidbody>().velocity = Vector3.zero; // sets the velocity of the ball to zero
            currentSoccerBallInstance.GetComponent<Rigidbody>().angularVelocity = Vector3.zero; // sets the angular velocity of the ball to zero
        }
    }

    /// <summary>
    /// finds all of the characters in the scene and loops through them
    /// tells them that there is a soccer ball
    /// </summary>
    private void AlertCharacters()
    {
        CharacterController[] mice = FindObjectsOfType<CharacterController>(); // find all instances of character controller type in our scene
        for(int i=0; i<mice.Length; i++)
        {
            mice[i].SoccerBallSpawned(currentSoccerBallInstance.transform);
        }
        uiManager.DisplayScores(true); // display our scores
        if(audioManager != null) // if we have a reference  to the audio manager
        {
            audioManager.PlayPlayingMusic(); // start playing the second track soccer music
        }
    }

    /// <summary>
    /// a function to handle the characters telling us that the player is too close, so play the fleeing music
    /// </summary>
    /// <param name="isRunningAway"></param>
    public void RunningAwayPlayer(bool isRunningAway)
    {
        if(isRunningAway == areCharactersRunningAway) // don't do anything if the value is the same
        {
            return;
        }
        else
        {
            areCharactersRunningAway = isRunningAway; // set our private bool to this value
        }

        // if characters are runnning away in fear
        if(areCharactersRunningAway)
        {
            audioManager.PlayFleeingMusic(); // start playing the fleeing music
        }
        else
        {
            audioManager.PlayPreviousTrack(); // otherwise start playing last track 
        }
    }
}
