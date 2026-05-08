using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

public class Env : Singleton<Env>
{
    public string language = "Ukrainian";

    public string ukrainian = "Ukrainian";
    public string english = "English";

    public bool isDebug = false;
    public bool lowHp = false;
    public bool endlessHeal = false;
    
    public UnityEvent OnLanguageChanged;
    public UnityEvent OnLowHpChanged;
    public UnityEvent OnEndlessHealChanged;

    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _soundSlider;
    [SerializeField] private AudioMixer _mixer;
    
    private void Awake()
    {
        language = PlayerPrefs.GetString("Language", ukrainian);
        endlessHeal = PlayerPrefs.GetInt("EndlessHeal", 0) == 1;
        lowHp = PlayerPrefs.GetInt("LowHp", 0) == 1;
    }

    private void Start()
    {
        var soundVolume = PlayerPrefs.GetFloat("SoundVolume", 0.3f);
        SetVolumeNormalized(soundVolume, "SoundVolume");
        if (_soundSlider != null)
        {
            _soundSlider.value = soundVolume;
            _soundSlider.onValueChanged.AddListener(x =>
            {
                SetVolumeNormalized(x, "SoundVolume");
                PlayerPrefs.SetFloat("SoundVolume", x);
            });
        }

        var musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.4f);
        SetVolumeNormalized(musicVolume, "MusicVolume");
        if (_musicSlider != null)
        {
            _musicSlider.value = musicVolume;
            _musicSlider.onValueChanged.AddListener(x =>
            {
                SetVolumeNormalized(x, "MusicVolume");
                PlayerPrefs.SetFloat("MusicVolume", x);
            });
        }
    }
    
    public void SwitchLanguage()
    {
        language = language == ukrainian ? english : ukrainian;
        PlayerPrefs.SetString("Language", language);
        OnLanguageChanged.Invoke();
    }

    public void ToggleEndlessHeal(bool value)
    {
        endlessHeal = value;
        PlayerPrefs.SetInt("EndlessHeal", endlessHeal ? 1 : 0);
    }

    public void ToggleLowHp(bool value)
    {
        lowHp = value;
        PlayerPrefs.SetInt("LowHp", lowHp ? 1 : 0);
    }

    public void OnMusicSliderChanged(float value)
    {
        
        SetVolumeNormalized(value, "MusicVolume");
    }
    
    public void OnSoundSliderChanged(float value)
    {
        PlayerPrefs.SetFloat("SoundVolume", value);
        SetVolumeNormalized(value, "SoundVolume");
    }
    
    public void SetVolumeNormalized(float value01, string param)
    {
        value01 = Mathf.Clamp(value01, 0.0001f, 1f);
        
        float db = Mathf.Log10(value01) * 20f; 
        db = Mathf.Clamp(db,-80, 0);

        _mixer.SetFloat(param, db);
    }

    private float DbToNormalized(float db)
    {
        db = Mathf.Clamp(db, -80, 0);
        return Mathf.Pow(10f, db / 20f);
    }
}
