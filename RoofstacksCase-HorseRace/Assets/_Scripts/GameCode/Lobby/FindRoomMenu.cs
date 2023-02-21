using System;
using System.Collections.Generic;
using System.Linq;
using GameCode.Data;
using GameCode.Hud;
using GameCode.Utilities.Events;
using Photon.Pun;
using Photon.Realtime;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GameCode.Lobby
{
    public class FindRoomMenu : Menu
    {
        [Header("View Components")] 
        [SerializeField] private Button _returnButton;
        [SerializeField] private Transform _roomListContent;
        
        [Header("Prefabs")] 
        [SerializeField] private RoomListItem _roomListPrefab;

        private List<RoomListItem> _roomListItems = new();
        private CompositeDisposable _disposable;
        private IMenuManager _menuManager;

        public void RegisterObservables(IMenuManager menuManager, CompositeDisposable disposable)
        {
            _menuManager = menuManager;
            _disposable = disposable;
            
            _returnButton
                .OnClickAsObservable()
                .Subscribe(_ => menuManager.OpenMenu(MenuType.Title))
                .AddTo(disposable);

            UpdateRoomList(LobbyManager.RoomInfoList);
            GameEvents.RoomListUpdate += UpdateRoomList;
        }

        private void OnEnable()
        {
            UpdateRoomList(LobbyManager.RoomInfoList);
        }

        private void OnDestroy()
        {
            GameEvents.RoomListUpdate -= UpdateRoomList;
        }

        private void UpdateRoomList(List<RoomInfo> roomList)
        {
            foreach (var item in _roomListItems)
            {
                Destroy(item.gameObject);
            }
            
            if (roomList == null) return;

            foreach (var roomInfo in roomList.Where(roomInfo => !roomInfo.RemovedFromList && roomInfo.IsVisible && roomInfo.IsOpen))
            {
                var roomListItem = Instantiate(_roomListPrefab, _roomListContent);
                roomListItem.RegisterObservables(roomInfo, _menuManager, _disposable);
                _roomListItems.Add(roomListItem);
            }
        }
    }
}
