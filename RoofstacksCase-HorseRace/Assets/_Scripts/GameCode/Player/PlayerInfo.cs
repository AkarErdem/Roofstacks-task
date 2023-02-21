using GameCode.Configuration;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace GameCode.Player
{
    [System.Serializable]
    public class PlayerInfo
    {
        [SerializeField] private TMP_Text _playerText;
        [SerializeField] private TMP_Text _horseNameText;
        [SerializeField] private Vector3 _spawnedPrefabPosition;
        [SerializeField] private Quaternion _spawnedPrefabRotation;
        
        private GameObject _spawnedHorse;
        private string _prefabName;
        private string _userId;
        
        public string HorseName => _horseNameText.text;
        public string PrefabName => _prefabName;
        public string UserId => _userId;
        
        public void SetUserId(string userId) => _userId = userId;
        public void SetPrefabName(string prefabName) => _prefabName = prefabName;
        public void SetPlayerText(string playerText) => _playerText.SetText(playerText);
        public void SetHorseText(string horseText) => _horseNameText.SetText(horseText);
        public void SpawnPrefab(HorseConfig horseConfig)
        {
            DestroyPrefab();
            SetHorseText(horseConfig.Name);
            _prefabName = horseConfig.PrefabName;
            _spawnedHorse = PhotonNetwork.Instantiate(HorseConfig.GetHorsePrefabPath(horseConfig.PrefabName), 
                                                      _spawnedPrefabPosition,
                                                      _spawnedPrefabRotation);
        }
        public void DisablePlayerInfo()
        {
            _playerText.SetText(string.Empty);
            DestroyPrefab();
        }
        
        private void DestroyPrefab()
        {
            SetHorseText(string.Empty);
            if (_spawnedHorse != null)
            {
                PhotonNetwork.Destroy(_spawnedHorse);
            }
        }
    }
}

