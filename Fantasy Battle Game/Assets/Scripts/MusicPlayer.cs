using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MusicPlayer
{
    public class MusicPlayer : MonoBehaviour
    {
        AudioSource MpPlayer;

        IEnumerator Start()
        {
            AudioSource audio = GetComponent<AudioSource>();
            audio.Play();
            yield return new WaitForSeconds(audio.clip.length);
        }
    }
}