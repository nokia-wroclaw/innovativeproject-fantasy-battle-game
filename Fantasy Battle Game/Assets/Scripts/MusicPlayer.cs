using System.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MusicPlayer : MonoBehaviour
{


    public List<AudioClip> musicClips = new List<AudioClip>();

    public AutomaticPlayer automaticPlayer;

    public static AudioSource Music;

    private AudioClip currentTrack_;

    private Coroutine musicLoop_;

    private AudioSource musicSource_;



    void Start()
    {
        automaticPlayer = new AutomaticPlayer(musicClips);

        musicSource_ = GetComponent<AudioSource>();
        musicSource_.volume = 0.25f;

        StartAutomaticMusic();
    }


    public void PlayMusicClip(AudioClip music)
    {
        musicSource_.Stop();
        musicSource_.clip = music;
        musicSource_.Play();
    }

    public void StopMusic()
    {
        if (musicLoop_ != null)
            StopCoroutine(musicLoop_);

        Music.Stop();
    }
    /// <summary>
    /// That's will play all of default tracks, which are defined in list.
    /// </summary>
    public void StartAutomaticMusic()
    {
        automaticPlayer = new AutomaticPlayer(musicClips);

        musicLoop_ = StartCoroutine(automaticPlayer.LoopMusic(this, 0, PlayMusicClip));
    }
    /// <summary>
    /// That's will play one selected track.
    /// </summary>
    /// <param name="index"></param>
    public void PlayOneTrack(int index)
    {
        PlayMusicClip(musicClips[index]);
    }

}

public class AutomaticPlayer
{
    private List<AudioClip> clips_;

    public AutomaticPlayer(List<AudioClip> clips)
    {
        this.clips_ = clips;
    }

    public IEnumerator LoopMusic(MonoBehaviour player, float delay, System.Action<AudioClip> playFunction)
    {
        while (true)
        {
            yield return player.StartCoroutine(Run((clips_), delay, playFunction));
        }
    }


    public IEnumerator Run(List<AudioClip> tracks,
        float delay, System.Action<AudioClip> playFunction)
    {
        foreach (AudioClip clip in tracks)
        {
            playFunction(clip);

            yield return new WaitForSeconds(clip.length);
        }
    }
}