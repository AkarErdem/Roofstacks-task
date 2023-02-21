using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using GameCode.Data;
using GameCode.Hud;
using GameCode.Init;
using GameCode.PlayFab;
using GameCode.Utilities.Events;
using GameCode.Utilities.ObjectExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UniRx;

namespace GameCode.Lobby
{
    public class LobbyManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TitleMenu _titleMenu;
        [SerializeField] private CreateRoomMenu _createRoomMenu;
        [SerializeField] private RoomMenu _roomMenu;
        [SerializeField] private ErrorMenu _errorMenu;
        [SerializeField] private FindRoomMenu _findRoomMenu;
        [SerializeField] private LeaderboardMenu _leaderboardMenu;
        [SerializeField] private SettingsMenu _settingsMenu;
        
        private IMenuManager _menuManager;
        private CompositeDisposable _disposable;

        public static List<RoomInfo> RoomInfoList { get; set; }
        
        private void Start()
        {
            _disposable = GameManager.Instance.Disposable;
            _menuManager = HierarchyExtensions.FindObjectsOfType<IMenuManager>().FirstOrDefault();
            _menuManager.OpenMenu(MenuType.Loading);

            _titleMenu.RegisterObservables(_menuManager, _disposable);
            _createRoomMenu.RegisterObservables(_menuManager, _disposable);
            _roomMenu.RegisterObservables(_menuManager, _disposable);
            _errorMenu.RegisterObservables(_menuManager, _disposable);
            _findRoomMenu.RegisterObservables(_menuManager, _disposable);
            _leaderboardMenu.RegisterObservables(_menuManager, _disposable);
            _settingsMenu.RegisterObservables(_menuManager, _disposable);
            
            if (PhotonNetwork.IsConnectedAndReady)
                _menuManager.OpenMenu(MenuType.Title);
            
            if (PhotonNetwork.IsConnectedAndReady)
                JoinLobby();
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Joined master");
            JoinLobby();
        }

        private void JoinLobby()
        {
            PhotonNetwork.JoinLobby();
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        public override async void OnJoinedLobby()
        {
            Debug.Log("Joined lobby");
            while (!PlayFabManager.IsLoggedIn())
            {
                await System.Threading.Tasks.Task.Delay(100);
            }
            _menuManager.OpenMenu(MenuType.Title);
        }

        public override  void OnJoinedRoom()
        {
            Debug.Log("Joined room");
            _menuManager.OpenMenu(MenuType.Room);
            _roomMenu.OnListItemClick();
        }
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log("Room creation failed");
            GameEvents.OnCreateRoomFailed(message);
        }

        public override void OnLeftRoom()
        {
            Debug.Log("Left room");
            //_menuManager.OpenMenu(MenuType.Title);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            RoomInfoList = roomList;
            GameEvents.OnRoomListUpdate(roomList);
        }

        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
        {
            GameEvents.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        }
    }
}

