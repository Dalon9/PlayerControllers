using System.Collections.Generic;
using S8l.Guardian.Runtime.Interfaces;
using UnityEngine;

namespace S8l.Guardian.Runtime.Effects
{
    public class GuardianRandomSoundEffect : GuardianEffect, IGuardianEffectProvider
    {
        [SerializeField] private List<AudioClip> clips;
        [SerializeField] private AudioSource source;
    
        private bool _played;
    
        public void PlayEffect(float strength)
        {
            if (strength >= trigger && !_played)
            {
                _played = true;
                source.PlayOneShot(clips[Random.Range(0, clips.Count - 1)]);
            }
            else if (strength < trigger && _played)
            {
                _played = false;
            }
        }
    }
}