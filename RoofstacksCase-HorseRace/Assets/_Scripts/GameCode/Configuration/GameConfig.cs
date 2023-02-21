using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace GameCode.Configuration
{
    [CreateAssetMenu(menuName = "Config/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Configuration")]
        [SerializeField] private AppConfig _appConfig;
        [SerializeField] private SceneConfig _sceneConfig;
        [SerializeField] private NetworkConfig _networkConfig;
        [SerializeField] private SoundConfig _soundConfig;
        [SerializeField] private List<HorseConfig> _horseConfigs;
        
        public int TargetFPS => _appConfig.TargetFPS;
        public int MaxPlayerPerRoom => _networkConfig.MaxPlayersPerRoom;
        
        public string GameSceneName => _sceneConfig.GameSceneName;
        public string LobbySceneName => _sceneConfig.LobbySceneName;
        
        public string TitleId => _networkConfig.TitleId;
        public int MaxPlayersPerRoom => _networkConfig.MaxPlayersPerRoom;
        public float SceneLoadCountdown => _sceneConfig.SceneLoadCountdown;
        public double GameStartCountdown => _networkConfig.GameStartCountdown;
        
        public AudioSource GameMusic => _soundConfig.GameMusic;
        public AudioSource[] SFX => _soundConfig.SFX;
        
        public AudioMixer AudioMixer => _soundConfig.AudioMixer;
        
        public List<HorseConfig> HorseConfigs => _horseConfigs;
    }
}
