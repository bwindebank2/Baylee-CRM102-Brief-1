using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    public Animator animator; // a reference to our animator component
    public enum AnimationState { Idle, Walking, Running, Passing, Waving } // the different states of our animation.
    public AnimationState currentAnimationState; // the current state our animation is in.

    /// <summary>
    /// Handles updating the animation state of our character
    /// </summary>
    public AnimationState CurrentState
    {
        get
        {
            return currentAnimationState;
        }
        set
        {
            currentAnimationState = value; // set our animation state to the value
            if(animator != null)
            {
                UpdateAnimator();
            }
            else
            {
                Debug.LogError("No animator has been assigned");
            }
        }
    }

    /// <summary>
    /// Updates the current state to current state of our character.
    /// </summary>
    private void UpdateAnimator()
    {
       switch(currentAnimationState)
        {
            case AnimationState.Idle:
                {
                    // reset our animator back to idle.
                    ResetToIdle();
                    break;
                }
            case AnimationState.Passing:
                {
                    ResetToIdle();
                    // set our animator to passing
                    animator.SetBool("Passing", true);
                    break;
                }
            case AnimationState.Running:
                {
                    ResetToIdle();
                    // set our animator to the running animation
                    animator.SetBool("Running", true);
                    break;
                }
            case AnimationState.Walking:
                {
                    ResetToIdle();
                    // set our animator to the
                    animator.SetBool("Walking", true);
                    break;
                }
            case AnimationState.Waving:
                {
                    ResetToIdle();
                    // set our animtor to the waving animation
                    animator.SetBool("Wave", true);
                    break;
                }
        }
    }

    /// <summary>
    /// reset our animator to the idle state
    /// </summary>
    private void ResetToIdle()
    {
        animator.SetBool("Passing", false);
        animator.SetBool("Running", false);
        animator.SetBool("Wave", false);
        animator.SetBool("Walking", false);
    }
}
