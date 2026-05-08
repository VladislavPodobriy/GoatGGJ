using System;
using System.Collections.Generic;
using UnityEngine;

namespace MainScripts.Audio
{
    [Serializable]
    public class AudioLibraryItem
    {
        public string Key;
        public List<AudioClip> Clips;
    }
}
