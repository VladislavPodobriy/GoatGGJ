using System.Collections;
using Pixelplacement;
using UnityEngine;

public class MusicController : Singleton<MusicController>
{
    public AudioClip MainTheme;
    public AudioClip WitchTheme;
    public AudioClip UnfateTheme;
    public AudioClip BrownieTheme;
    public AudioClip DidkoTheme;
    public AudioSource Source;

    public void Start()
    {
        PlayMainTheme();
    }
    
    public void PlayMainTheme()
    {
        Source.clip = MainTheme;
        Source.Play();
    }
    
    public void PlayWitchTheme()
    {
        Source.clip = WitchTheme;
        Source.Play();
    }
    
    public void PlayUnfateTheme()
    {
        Source.clip = UnfateTheme;
        Source.Play();
    }
    
    public void PlayBrownieTheme()
    {
        Source.clip = BrownieTheme;
        Source.Play();
    }
    
    public void PlayDidkoTheme()
    {
        Source.clip = DidkoTheme;
        Source.Play();
    }
}
