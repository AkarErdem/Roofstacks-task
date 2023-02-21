using GameCode.Data;
using GameCode.Hud;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GameCode.Lobby
{
    public class RoomListItem : Menu
    {
        [SerializeField] private TMP_Text _roomName;
        [SerializeField] private TMP_Text _playerCount;
        [SerializeField] private Button _enterRoomButton;
        
        private IReactiveProperty<RoomInfo> _roomInfo = new ReactiveProperty<RoomInfo>();
        private IMenuManager _menuManager;

        public void RegisterObservables(RoomInfo roomInfo, IMenuManager menuManager, CompositeDisposable disposable)
        {
            this._menuManager = menuManager;
            this._roomInfo.Value = roomInfo;
            
            _roomInfo.Value
                .ObserveEveryValueChanged(info => info.PlayerCount)
                .Subscribe(_ =>
                {
                    _roomName.SetText($"{_roomInfo.Value.Name}");
                    _playerCount.SetText($"{_roomInfo.Value.PlayerCount}/{_roomInfo.Value.MaxPlayers}");
                    _enterRoomButton.interactable = _roomInfo.Value.PlayerCount < _roomInfo.Value.MaxPlayers && roomInfo.IsOpen && roomInfo.IsVisible;
                })
                .AddTo(disposable);
            
            _enterRoomButton
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _menuManager.OpenMenu(MenuType.Loading);
                    PhotonNetwork.JoinRoom(roomInfo.Name);
                })
                .AddTo(disposable);
        }
    }
}

