using UnityEngine;

namespace GameCode.Configuration
{
    [CreateAssetMenu(menuName = "Config/Scene Config")]
    public class SceneConfig : ScriptableObject
    {
        [Header("Configuration")]
        [SerializeField] private string _lobbySceneName;
        [SerializeField] private string _gameSceneName;
        [SerializeField] private float sceneLoadCountdown;

        public string LobbySceneName => _lobbySceneName;
        public string GameSceneName => _gameSceneName;
        public float SceneLoadCountdown => sceneLoadCountdown;
    }
}
