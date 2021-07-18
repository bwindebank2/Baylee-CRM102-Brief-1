using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    /// <summary>
    /// the different states that our character can be in
    /// </summary>
    public enum CharacterStates {Idle, Roaming, Waving, Playing, Fleeing}

    public CharacterStates currentCharacterState; // the current state our character is in

    public GameManager gameManager;
    public Rigidbody rigidBody;

    // idle state variables
    public float idleTime = 2;
    private float currentIdleWaitTime; // the time we are waiting till we can move again

    // roaming state variables
    public float moveSpeed = 3;
    public float minDistanceToTarget = .01f;
    private Vector3 currentTargetPosition; // the target we are heading towards
    private Vector3 previousTargetPosition; // the previous target we were heading towards

    // waving state variables
    public float waveTime = 2; // the time spent waving
    private float currentWaveTime; // the current time to wave till
    public float distanceToStartWaving = 4f; // the distance we will be checking to see if we are in range to wave at another character
    private CharacterController[] allCharactersInScene; // a collection of references to all characters in the scene
    public float timeBetweenWaves = 5; // the time between when we are allowed to wave again
    private float currentTimeBetweenWaves; // the current time for our next wave to be initiated

    // fleeing state variables
    public float distanceThresholdOfPlayer = .5f; // the ditsance that is "too" close
    public float fleeSpeed = .5f;

    // playing state variables
    private Transform currentSoccerBall = null; // a reference to the soccer ball
    public GameObject selfIdentifier; // a reference to our idenfication colour
    public GameObject myGoal; // a reference to the character's goal
    public float soccerBallKickForce = 10; // the amount of force the character can use to kick the ball
    public float soccerBallInteractDistance = .1f; // if the soccer ball is close enough, we can kick it.

    public float passingAnimationDelay = 0.5f; // a delay of the soccer animation before they kick
    private float currentTimeTillPassingAnimation; // the time at which this animation will play and we should kick

    public AnimationHandler animationHandler; // a reference to our animation handler scriper

    /// <summary>
    /// returns current target position, and sets new current position
    /// </summary>
    private Vector3 CurrentTargetPosition
    {
        get
        {
            return currentTargetPosition; // gets current value
        }
        set
        {
            previousTargetPosition = currentTargetPosition; // assign our current target position to our previous target position
            currentTargetPosition = value; // assign a new value to current target position
        }
    }

    /// <summary>
    ///  call each time the script or the object is disabled
    /// </summary>
    private void OnDisable()
    {
        if(gameManager != null) // if game is not null
        {
            gameManager.RunningAwayPlayer(false); // then telll is there are no characters in range
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        CurrentTargetPosition = gameManager.ReturnRandomPositionOnField(); // get random starting position
        allCharactersInScene = FindObjectsOfType<CharacterController>();  // fiond the references to all of our characters in the scene
        currentCharacterState = CharacterStates.Roaming; // set the character by default to start roaming
        selfIdentifier.SetActive(false);
        animationHandler.CurrentState = AnimationHandler.AnimationState.Idle; //  set our animation to idle

    }

    // Update is called once per frame
    void Update()
    {
        LookAtTargetPosition(); // always look towards the position we are moving towards
        // if still too far away, get closer
        HandleRoamingState(); // call our roaming state function
        HandleIdleState(); // call our idle state function
        HandleWavingState(); // call our waving state function
        HandleFleeingState(); // call our fleeing state function
        HandlePlayingState(); // call our playing state function
    }

    /// <summary>
    /// Handles the movement state of our character
    /// </summary>
    private void HandleRoamingState()
    {
        float distanceToTarget = 0;

        if(currentSoccerBall != null)
        {
            distanceToTarget = soccerBallInteractDistance;
        }
        else
        {
            distanceToTarget = minDistanceToTarget;
        }

        // if we are still too far away, move closer
        if (currentCharacterState == CharacterStates.Roaming && Vector3.Distance(transform.position, CurrentTargetPosition) > distanceToTarget)
        {
            if(currentSoccerBall != null)
            {
                // here would should be running
                if(animationHandler.CurrentState != AnimationHandler.AnimationState.Running)
                {
                    animationHandler.CurrentState = AnimationHandler.AnimationState.Running; // set our animation to running animation
                }

                currentTargetPosition = currentSoccerBall.position;
                Vector3 targetPosition = new Vector3(CurrentTargetPosition.x, transform.position.y, CurrentTargetPosition.z); // position we want to move towards
                Vector3 nextMovePosition = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime * fleeSpeed); // the amount we want to move to that position
                rigidBody.MovePosition(nextMovePosition);
                currentIdleWaitTime = Time.time + idleTime;
            }
            else
            {
                if(animationHandler.CurrentState != AnimationHandler.AnimationState.Walking)
                {
                    animationHandler.CurrentState = AnimationHandler.AnimationState.Walking; // set our animaiton to running animation
                }
                Vector3 targetPosition = new Vector3(CurrentTargetPosition.x, transform.position.y, CurrentTargetPosition.z); // position we want to move towards
                Vector3 nextMovePosition = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime); // the amount we want to move to that position
                rigidBody.MovePosition(nextMovePosition);
                currentIdleWaitTime = Time.time + idleTime;
            }
        }
        else if (currentCharacterState == CharacterStates.Roaming) // check to see if we are still roaming
        {
            if(currentSoccerBall != null)
            {
                currentCharacterState = CharacterStates.Playing; // start playing with the ball
                currentTimeTillPassingAnimation = Time.time + passingAnimationDelay; // sets the time to wait till until we play the animation
                // we want to kick the ball
            }
            else
            {
                currentCharacterState = CharacterStates.Idle; // change to idle state
            }
        }
    }

    /// <summary>
    /// Handles the idle state of our character
    /// </summary>
    private void HandleIdleState()
    {
        if(currentCharacterState == CharacterStates.Idle)
        {
            // we are close enough to our target position.
            // we want to wait a couple of seconds.
            // find a new position.
            if (Time.time > currentIdleWaitTime)
            {
                // lets find a new target
                CurrentTargetPosition = gameManager.ReturnRandomPositionOnField();
                currentCharacterState = CharacterStates.Roaming; // start roaming again
            }
            if (animationHandler.CurrentState != AnimationHandler.AnimationState.Idle)
            {
                animationHandler.CurrentState = AnimationHandler.AnimationState.Idle; // set our animation to running animation
            }
        }
    }

    /// <summary>
    /// Handles the fleeing of our character
    /// </summary>
    private void HandleFleeingState()
    {
        if(currentCharacterState != CharacterStates.Fleeing && gameManager.IsPlayerTooCloseToCharacter(transform, distanceThresholdOfPlayer))
        {
            // we should be fleeing
            currentCharacterState = CharacterStates.Fleeing;
            gameManager.RunningAwayPlayer(true); // we are fleeing from the player, play flee music

            if (animationHandler.CurrentState != AnimationHandler.AnimationState.Running)
            {
                animationHandler.CurrentState = AnimationHandler.AnimationState.Running; // set our animation to running animation
            }

        }
        else if(currentCharacterState == CharacterStates.Fleeing && gameManager.IsPlayerTooCloseToCharacter(transform, distanceThresholdOfPlayer))
        {
            // we are fleeing now
            if (currentCharacterState == CharacterStates.Fleeing && Vector3.Distance(transform.position, CurrentTargetPosition) > minDistanceToTarget)
            {
                Vector3 targetPosition = new Vector3(CurrentTargetPosition.x, transform.position.y, CurrentTargetPosition.z); // position we want to move towards
                Vector3 nextMovePosition = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime * fleeSpeed); // the amount we want to move to that position
                rigidBody.MovePosition(nextMovePosition);
            }
            else
            {
                CurrentTargetPosition = gameManager.ReturnRandomPositionOnField();
            }

        }
        else if (currentCharacterState == CharacterStates.Fleeing && !gameManager.IsPlayerTooCloseToCharacter(transform, distanceThresholdOfPlayer))
        {
            // if we are still fleeing, then we want to transition back into roaming state
            currentCharacterState = CharacterStates.Roaming;
            CurrentTargetPosition = gameManager.ReturnRandomPositionOnField();
            gameManager.RunningAwayPlayer(false); // stop fleeing from the player, go back to original music

        }
    }

    /// <summary>
    /// Handles the playing of our character
    /// </summary>
    private void HandlePlayingState()
    {
        if (currentCharacterState == CharacterStates.Playing)
        {
            if (animationHandler.CurrentState != AnimationHandler.AnimationState.Passing)
            {
                animationHandler.CurrentState = AnimationHandler.AnimationState.Passing; // set our animation to running animation
            }

            if(Time.time > currentTimeTillPassingAnimation)
            {
                KickBall();
                // set our target to the soccer ball again
                // start moving towards the ball again
                CurrentTargetPosition = currentSoccerBall.position;
                currentCharacterState = CharacterStates.Roaming;
            }
        }
    }

    /// <summary>
    /// Handles the waving of our character
    /// </summary>
    private void HandleWavingState()
    {
        if(ReturnCharacterTransformToWaveAt() != null && currentCharacterState != CharacterStates.Waving && Time.time >currentTimeBetweenWaves && currentCharacterState != CharacterStates.Fleeing && currentSoccerBall == null)
        {
            // we should start waving
            currentCharacterState = CharacterStates.Waving;
            currentWaveTime = Time.time + waveTime; // set up the time we should be waving till
            currentTargetPosition = ReturnCharacterTransformToWaveAt().position; // set the current target position to the closest transform, so that way we also face towards it

            if (animationHandler.CurrentState != AnimationHandler.AnimationState.Waving)
            {
                animationHandler.CurrentState = AnimationHandler.AnimationState.Waving; // set our animation to running animation
            }
        }
        if(currentCharacterState == CharacterStates.Waving && Time.time > currentWaveTime)
        {
            // stop waving
            currentTargetPosition = previousTargetPosition; // resume moving towards random target position
            currentTimeBetweenWaves = Time.time + timeBetweenWaves; // set the next time for when we can wave again
            currentCharacterState = CharacterStates.Roaming;
        }

    }

    /// <summary>
    /// return the transform if they are in range to be waved at
    /// </summary>
    /// <returns></returns>
    private Transform ReturnCharacterTransformToWaveAt()
    {
        // looping through all characters in our scene
        for(int i =0; i<allCharactersInScene.Length; i++)
        {
            // if the current element we are up to is not equal tho this instance our character
            if(allCharactersInScene[i]  != this)
            {
                // check the distance between them, if they are close enough, return that other character
                if(Vector3.Distance(transform.position, allCharactersInScene[i].transform.position) < distanceToStartWaving)
                {
                    return allCharactersInScene[i].transform;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Rotates character to face target position
    /// </summary>
    private void LookAtTargetPosition()
    {
        Vector3 directionToLookAt = CurrentTargetPosition - transform.position; // get the direction we want to look at
        directionToLookAt.y = transform.position.y; // don't change y rotation
        Quaternion rotationOfDirection = Quaternion.LookRotation(directionToLookAt); // get a rotation that we can use to look towards
        transform.rotation = rotationOfDirection; // set our current rotation to our rotation to face towards 
    }

    private void OnDrawGizmosSelected()
    {
        // draw a blue sphere at target pos
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(currentTargetPosition, 0.01f);
    }

    /// <summary>
    /// Is called when the soccer ball is spawned
    /// </summary>
    /// <param name="SoccerBall"></param>
    public void SoccerBallSpawned(Transform SoccerBall)
    {
        currentSoccerBall = SoccerBall; // assign soccer ball to reference
        CurrentTargetPosition = currentSoccerBall.position; // set our target position to the soccer ball
        currentCharacterState = CharacterStates.Roaming; // start moving towards ball
        selfIdentifier.SetActive(true);
    }

    /// <summary>
    /// handles kicking the ball
    /// </summary>
    public void KickBall()
    {
        Vector3 direction = myGoal.transform.position - transform.forward; // get a directional vector that moves towards the goalpost
        currentSoccerBall.GetComponent<Rigidbody>().AddForce(direction * soccerBallKickForce * Random.Range(0.5f, 5f)); // kick ball towards our goal post, add a little random force
    }
}
