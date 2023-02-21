using UnityEngine;

namespace GameCode.Configuration
{
    [CreateAssetMenu(menuName = "Config/Network Config")]
    public class NetworkConfig : ScriptableObject
    {
        [Header("PlayFab Configuration")]
        [SerializeField] private string _titleId;
        [SerializeField] private int _maxPlayersPerRoom;
        [SerializeField] private double _gameStartCountdown;
        public string TitleId => _titleId;
        
        public int MaxPlayersPerRoom => _maxPlayersPerRoom;
        
        public double GameStartCountdown => _gameStartCountdown;
    }
}
