using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip roamingMusic; // the music we will use for roaming state
    public AudioClip playingMusic; // music that will play during play state
    public AudioClip fleeingMusic; // the mice scream as they flee

    private AudioClip currentTrack; // the current track being played
    private AudioClip previousTrack; // the previous track that was played
    public AudioSource audioSource; // a reference to our audiosource, where the music will be playing from

    /// <summary>
    /// So this gets called every time the object/script is turned on
    /// </summary>
    private void OnEnable()
    {
        if (currentTrack == null)
        {
            currentTrack = roamingMusic; // just music to default
        }
        ChangeTrack(currentTrack); // start playing our music
    }

    /// <summary>
    /// Plays the music that is played during the start of the game and whilst roaming
    /// </summary>
    public void PlayRoamingMusic()
    {
        currentTrack = roamingMusic;
        ChangeTrack(currentTrack);
    }

    /// <summary>
    /// play the music played during soccer game
    /// </summary>
    public void PlayPlayingMusic()
    {
        currentTrack = playingMusic;
        ChangeTrack(currentTrack);
    }


    /// <summary>
    /// Plays the fleeing music
    /// </summary>
    public void PlayFleeingMusic()
    {
        currentTrack = fleeingMusic;
        ChangeTrack(currentTrack);
    }

    /// <summary>
    /// Play the previous track that was being played
    /// </summary>
    public void PlayPreviousTrack()
    {
        // if there is no previous track
        if(previousTrack == null)
        {
            return;
        }
        currentTrack = previousTrack; // set the current track to the previous track
        ChangeTrack(currentTrack); // play previous track

    }

    /// <summary>
    /// this function changes the clip being played atm
    /// </summary>
    private void ChangeTrack(AudioClip clip)
    {
        audioSource.Stop(); // stop playing the current clip
        if(audioSource.clip != clip) // iof the current clip in the source is not equal to the track we want to play
        {
            previousTrack = audioSource.clip; // store the previous track
            audioSource.clip = clip; // set the new track
        }
        audioSource.loop = true; // set the music to be looping
        audioSource.Play(); // start playing our music
    }
}
