using UnityEngine;

namespace GameCode.Configuration
{
    [CreateAssetMenu(menuName = "Config/App Config")]
    public class AppConfig : ScriptableObject
    {
        [Header("Configuration")]
        [SerializeField] private int _targetFPS;
        
        public int TargetFPS => _targetFPS;
    }
}
