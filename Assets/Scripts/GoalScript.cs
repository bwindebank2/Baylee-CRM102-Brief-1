using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalScript : MonoBehaviour
{

    public int playerNumber = 1; // this is the number of our player
    public GameManager gameManager; // a reference to our game manager

    public GameObject fireWorksPrefab; // reference to a prefab
    public Transform leftFireWorksPosition; // an empty transform left of our goal
    public Transform rightFireWorksPosition; // an empty transform left of our goal

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "SoccerBall")
        {
            Debug.Log("Goal Scored");
            gameManager.IncreasePlayerScore(playerNumber);
            // increase player score

            //spawn in fireworks in left and right positions, then parent them to our AR parent
            GameObject clone = Instantiate(fireWorksPrefab, leftFireWorksPosition.position, fireWorksPrefab.transform.rotation, gameManager.aRContentParent);
            Destroy(clone, 5);

            clone = Instantiate(fireWorksPrefab, rightFireWorksPosition.position, fireWorksPrefab.transform.rotation, gameManager.aRContentParent);
            Destroy(clone, 5);

        }
    }
}
