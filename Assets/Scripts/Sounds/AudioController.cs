using System.Collections.Generic;
using System.Linq;
using MainScripts.VFX;
using Pixelplacement;
using UnityEngine;
using UnityEngine.Audio;


namespace MainScripts.Controllers
{
    public class AudioController : Singleton<AudioController>
    {
        //[SerializeField] private AudioMixer _mixer;
        [SerializeField] private SoundWithLifetime _soundPrefab;
        [SerializeField] private float _bufferSize;
        [SerializeField] private List<SoundWithLifetime> _soundsPool;
        //private static float _globalVolume;
    
        //public static float GlobalVolumeNormalized = 1f;
        //public static bool SoundsEnabled = true;

        private void Start()
        {
            _soundsPool = new List<SoundWithLifetime>();
            for (int i = 0; i < _bufferSize; i++)
            {
                //var instance = SpawnEffect();
                //_soundsPool.Add(instance);
            }
            //ChangeGlobalVolume(GlobalVolumeNormalized);
            //ToggleSounds(SoundsEnabled);
        }

        public SoundWithLifetime SpawnEffect()
        {
            var instance = Instantiate(_soundPrefab, transform.position, Quaternion.identity);
            instance.transform.parent = transform;
            instance.OnEnd.AddListener(() =>
            {
                _soundsPool.Add(instance);
            });
            return instance;
        }
        
        /*public void ChangeGlobalVolume(float value)
        {
            GlobalVolumeNormalized = value;
            _globalVolume = (GlobalVolumeNormalized * 80) - 80;
            _mixer.SetFloat ("GlobalVolume", _globalVolume);
        }

        public void ToggleSounds(bool value)
        {
            SoundsEnabled = value;

            if (SoundsEnabled) 
                _mixer.SetFloat ("SoundVolume", 0);
            else
                _mixer.SetFloat ("SoundVolume", -80);
        }

        public void MuteGameSounds()
        {
            _mixer.SetFloat ("GameSoundsVolume", -80);
        }
    
        public void UnMuteGameSounds()
        {
            if (SoundsEnabled) 
                _mixer.SetFloat ("GameSoundsVolume", 0);
            else
                _mixer.SetFloat ("GameSoundsVolume", -80);
        }*/
        
        public static void PlayAtWorldPosition(AudioClip clip, Vector3 position, float volume = 1)
        {
            SoundWithLifetime effect;
            /*if (Instance._soundsPool.Count > 0)
            {
                effect = Instance._soundsPool[0];
                Instance._soundsPool.Remove(effect);
            }
            else*/
                effect = Instance.SpawnEffect();
            effect.transform.position = position;
            effect.PlayOneShot(clip, volume);
        }
    }
}
