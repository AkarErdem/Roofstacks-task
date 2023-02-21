using GameCode.Data;
using GameCode.Hud;
using GameCode.Init;
using Photon.Pun;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GameCode.Lobby
{
    public class CreateRoomMenu : Menu
    {
        [Header("View Components")] 
        [SerializeField] private TMP_InputField _roomNameInputField;
        [SerializeField] private Button _createRoomButton;
        [SerializeField] private Button _returnButton;
        
        public void RegisterObservables(IMenuManager menuManager, CompositeDisposable disposable)
        {
            _roomNameInputField
                .ObserveEveryValueChanged(x => x.text)
                .Subscribe(value => _createRoomButton.interactable = value.Length > 0)
                .AddTo(disposable);
            
            _createRoomButton
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    menuManager.OpenMenu(MenuType.Loading);
                    var roomName = _roomNameInputField.text;
                    PhotonNetwork.CreateRoom(roomName, GameManager.Instance.RoomOptions);
                })
                .AddTo(disposable);
            
            _returnButton
                .OnClickAsObservable()
                .Subscribe(_ => menuManager.OpenMenu(MenuType.Title))
                .AddTo(disposable);
        }
    }
}
