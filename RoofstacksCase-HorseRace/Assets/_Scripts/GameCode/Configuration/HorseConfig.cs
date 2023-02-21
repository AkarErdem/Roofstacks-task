using GameCode.Data;
using UnityEngine;
using System.IO;

namespace GameCode.Configuration
{
    [CreateAssetMenu(menuName = "Config/Horse Config")]
    public class HorseConfig : ScriptableObject
    {
        [Header("Resources")]
        [SerializeField] private Sprite _icon;
        [SerializeField] private Color _color;
        [SerializeField] private string _prefabName;
        
        [Header("Properties")]
        [SerializeField] private float _speed;
        [SerializeField] private float _boostSpeed;
        [SerializeField] private string _name;
        
        public Sprite Icon => _icon;
        public Color Color => _color;      
        
        public float Speed => _speed;
        public float BoostSpeed => _boostSpeed;
        
        public string Name => _name;
        public string PrefabName => _prefabName;

        public static string GetHorsePrefabPath(string horsePrefabName)
        {
            return Path.Combine(ConstantData.PhotonPrefabsFolderName,
                                ConstantData.HorsePrefabsFolderName,
                                horsePrefabName);
        }
    }
}
