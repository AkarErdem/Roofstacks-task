using UnityEngine;
using UnityEngine.Audio;

namespace GameCode.Configuration
{
    [CreateAssetMenu(menuName = "Config/Sound Config")]
    public class SoundConfig : ScriptableObject
    {
        [Header("Configuration")]
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private AudioSource _gameMusic;
        [SerializeField] private AudioSource[] _sfx;
        
        public AudioMixer AudioMixer => _audioMixer;
        public AudioSource GameMusic => _gameMusic;
        public AudioSource[] SFX => _sfx;
    }
}
