using GameCode.Data;
using GameCode.Hud;
using GameCode.Utilities.Events;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GameCode.Lobby
{
    public class ErrorMenu : Menu
    {
        [Header("View Components")] 
        [SerializeField] private TMP_Text _errorText;
        [SerializeField] private Button _returnButton;
        
        private IMenuManager _menuManager;
        
        public void RegisterObservables(IMenuManager menuManager, CompositeDisposable disposable)
        {
            _menuManager = menuManager;
            
            _returnButton
                .OnClickAsObservable()
                .Subscribe(_ => menuManager.OpenMenu(MenuType.Title))
                .AddTo(disposable);
            
            GameEvents.CreateRoomFailed += OnCreateRoomFailed;
        }

        private void OnCreateRoomFailed(string message)
        {
            _errorText.SetText($"Room creation failed. {message}");
            _menuManager.OpenMenu(MenuType.Error);
        }
    }
}
