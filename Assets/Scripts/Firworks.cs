using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firworks : MonoBehaviour
{
    public AudioClip fireWorkSound; // the firework sfx
    public AudioSource audioSource; // reference to our audio source
    public int numberOfFireworks = 3; // the number of fireworks
    public float timeBetweenFireworks = 0.5f; // half a second between each firework
    public float initialDelay = 2;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PlayFireWorks()); // start our coroutine
    }

    /// <summary>
    /// A coroutine that allows us to dictate when certain parts of code would be played. this allows us to delay certain parts
    /// but also allows us to do more complex actions
    /// </summary>
    /// <returns></returns>
    IEnumerator PlayFireWorks()
    {
        yield return new WaitForSeconds(initialDelay); // wait for a couple of seconds before contitnuing
        for(int i = 0; i<numberOfFireworks; i++)
        {
            audioSource.PlayOneShot(fireWorkSound); // play our fireworks once
            yield return new WaitForSeconds(timeBetweenFireworks); // now wait before we iterate to the next part of the loop
        }

        yield return null;
    }
}
