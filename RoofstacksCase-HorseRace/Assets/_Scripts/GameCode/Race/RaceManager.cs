using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using GameCode.Data;
using GameCode.Init;
using GameCode.Lobby;
using GameCode.Player;
using GameCode.PlayFab;
using GameCode.Utilities.ObjectExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace GameCode.Race
{
    public class RaceManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private RaceUI _raceUI;
        [SerializeField] private CinemachineVirtualCameraBase[] _cinemachineVirtualCameras;
        [SerializeField] private Transform[] _raceStartPoints;
        [SerializeField] private Transform _raceEndPoint;
        
        private List<PlayerInfo> _playerInfosCache = new();
        private CinemachineVirtualCameraBase _activeCinemachineVirtualCamera;
        private string _userId;

        public bool IsRaceStarted { get; private set; }

        private void Start()
        {
            _raceUI.OpenMenu(MenuType.Loading);
            LobbyManager.RoomInfoList.Clear();
            
            _playerInfosCache = GameManager.Instance.PlayerInfosCache;
            _userId = PhotonNetwork.LocalPlayer.UserId;
            var disposable = GameManager.Instance.Disposable;
            
            foreach (var cache in _playerInfosCache.Where(cache => cache.UserId == _userId))
            {
                Debug.Log($"Horse created for the user: {_userId}");

                int startingIndex = 0;
                if (!PhotonNetwork.IsMasterClient)
                {
                    startingIndex = 1;
                }
                
                var playerManager = 
                    PhotonNetwork.Instantiate(ConstantData.PlayerManagerPath, 
                            Vector3.zero, 
                            Quaternion.identity)
                        .GetComponent<PlayerManager>();

                foreach (var horseConfig in GameManager.Instance.GameConfig.HorseConfigs
                             .Where(horseConfig => horseConfig.PrefabName == cache.PrefabName))
                {
                    playerManager.CreateController(new MovementInput(this), horseConfig, 
                        _raceStartPoints[startingIndex].position, _raceEndPoint.position, disposable);

                    _activeCinemachineVirtualCamera = _cinemachineVirtualCameras[startingIndex];
                    _activeCinemachineVirtualCamera.Priority = 10;
                    break;
                }
                break;
            }

            if (PhotonNetwork.IsMasterClient)
            {
                var hash = new Hashtable
                {
                    { "RaceStart", GameManager.Instance.GameConfig.GameStartCountdown },
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            }
        }

        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
        {
            if (IsRaceStarted && changedProps.ContainsKey("RaceEnd"))
            {
                IsRaceStarted = false;
                var winnerId = (string)changedProps["RaceEnd"];

                if (PhotonNetwork.LocalPlayer.UserId == winnerId)
                {
                    PlayFabManager.UpdatePlayerScore();
                }
                
                _raceUI.SetResult(PhotonNetwork.LocalPlayer.UserId == winnerId);
                _raceUI.OpenMenu(MenuType.RaceResult);
                
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.LeaveLobby();
                return;
            }
            if (changedProps.ContainsKey("RaceStart"))
            {
                StartCoroutine(StartRace((double)changedProps["RaceStart"]));
                
                _activeCinemachineVirtualCamera.Follow = null;
                FindCameraTargets();
                return;
            }
        }
        
        private IEnumerator StartRace(double cooldown)
        {
            yield return new WaitForSeconds(1f);

            _raceUI.OpenMenu(MenuType.Countdown);

            var musicPlayTimes = 4;
            var raceStartTime = PhotonNetwork.Time + cooldown;
            
            while (raceStartTime - PhotonNetwork.Time > 0)
            {
                string cooldownText = (raceStartTime - PhotonNetwork.Time).ToString("F0");
                if (raceStartTime - PhotonNetwork.Time < 1f)
                {
                    cooldownText = "GO!";
                }
                if (musicPlayTimes > 0 && musicPlayTimes > raceStartTime - PhotonNetwork.Time)
                {
                    var playIndex = musicPlayTimes == 1 ? 1 : 0;

                    GameManager.Instance.SFX[playIndex].Play();
                    
                    musicPlayTimes--;
                }
                _raceUI.SetCooldownText(cooldownText);
                yield return null;
            }
            
            FindCameraTargets();

            // Race started!
            _raceUI.OpenMenu(MenuType.Game);
            IsRaceStarted = true;
        }

        private void FindCameraTargets()
        {
            if (_activeCinemachineVirtualCamera.Follow != null)
                return;
            
            var controllers = HierarchyExtensions.FindObjectsOfType<PlayerController>();

            foreach (var controller in controllers.Where(controller => controller.PhotonView.IsMine))
            {
                _activeCinemachineVirtualCamera.Follow = controller.transform;
                break;
            }
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            LobbyManager.RoomInfoList = roomList;
        }

        public override void OnLeftRoom()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.DestroyAll();
            }
        }
    }
}



