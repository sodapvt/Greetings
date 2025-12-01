using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    internal static AudioHandler instance;
    [SerializeField] private AudioSource sfxAudioSource, voAudioSource,extraAudioSource;
    [SerializeField] private AudioClip click,appear,pop;
    [SerializeField]
    private AudioClip create, wish;
    int shakingIndex = 0;
    void Start()
    {
        instance = this;
    }
    public float PlaySFX(string clipName)
    {

         AudioClip clip = null;
        switch (clipName)
        {
            case "click":
                clip = click;
                break;
            case "appear":
                clip = appear;
                break;
            case "pop":
                clip = pop;
                break;
            // Add more cases for other SFX clips as needed
            default:
                Debug.LogWarning("Unknown SFX clip name: " + clipName);
                return 0f;
        }
        sfxAudioSource.clip = clip;
        sfxAudioSource.Play();
        return clip.length;
    }
    public void PlayOneShotSFX(string clipName)
    {
        AudioClip clip = null;
        switch (clipName)
        {
            default:
                Debug.LogWarning("Unknown SFX clip name: " + clipName);
                return;
        }
        extraAudioSource.PlayOneShot(clip);
    }

    public void StopSFX()
    {
        sfxAudioSource.Stop();
    }
    public float PlayVO(string clipName)
    {
        AudioClip clip = null;
        switch (clipName)
        {
            case "create":
                clip = create;
                break;
            case "wish":
                clip = wish;
                break;
            // Add more cases for other VO clips as needed
            default:
                Debug.LogWarning("Unknown VO clip name: " + clipName);
                return 0f;
        }
        voAudioSource.clip = clip;
        voAudioSource.Play();
        return clip.length;
    }
    public void StopVO()
    {
        voAudioSource.Stop();
    }
    public bool IsVOPlaying()
    {
        return voAudioSource.isPlaying;
    }
    public bool IsSFXPlaying()
    {
        return sfxAudioSource.isPlaying;
    }
}
