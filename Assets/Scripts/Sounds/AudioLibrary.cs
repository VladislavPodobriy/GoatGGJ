using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MainScripts.Audio
{
    [CreateAssetMenu(fileName = "AudioLibrary", menuName = "ScriptableObjects/Audio Library", order = 1)]
    public class AudioLibrary : ScriptableObject
    {
        [SerializeField] private List<AudioLibraryItem> _items;

        public AudioClip Get(string key)
        {
            var item = _items.FirstOrDefault(x => x.Key == key);
            if (item != null)
                return item.Clips.FirstOrDefault();
            return null;
        }
        
        public AudioClip Get(string key, int index)
        {
            var item = _items.FirstOrDefault(x => x.Key == key);
            if (item != null && item.Clips.Count > index)
                return item.Clips[index];
            return null;
        }

        public AudioClip GetRandom(string key)
        {
            var item = _items.FirstOrDefault(x => x.Key == key);
            if (item != null)
            {
                if (item.Clips.Count > 1)
                {
                    var clip = item.Clips[Random.Range(1, item.Clips.Count)];
                    item.Clips.Remove(clip);
                    item.Clips.Insert(0, clip);
                    return clip;
                }
                return item.Clips[0];
            }
            return null;
        }
    }

    public static class SoundManagerExtensions
    {
        public static AudioClip GetRandom(this List<AudioClip> clips)
        {
            return clips[Random.Range(0, clips.Count)];
        }
    }
}