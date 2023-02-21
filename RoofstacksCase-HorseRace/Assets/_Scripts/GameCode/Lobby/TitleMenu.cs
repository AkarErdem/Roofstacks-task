using GameCode.Data;
using GameCode.Hud;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GameCode.Lobby
{
    public class TitleMenu : Menu
    {
        [Header("View Components")] 
        [SerializeField] private Button _findRoomButton;
        [SerializeField] private Button _createRoomButton;
        [SerializeField] private Button _leaderboardButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _quitGameButton;
        
        public void RegisterObservables(IMenuManager menuManager, CompositeDisposable disposable)
        {
            _findRoomButton
                .OnClickAsObservable()
                .Subscribe(_ => menuManager.OpenMenu(MenuType.FindRoom))
                .AddTo(disposable);
            
            _createRoomButton
                .OnClickAsObservable()
                .Subscribe(_ => menuManager.OpenMenu(MenuType.CreateRoom))
                .AddTo(disposable);
            
            _leaderboardButton
                .OnClickAsObservable()
                .Subscribe(_ => menuManager.OpenMenu(MenuType.Leaderboard))
                .AddTo(disposable);
            
            _settingsButton
                .OnClickAsObservable()
                .Subscribe(_ => menuManager.OpenMenu(MenuType.Settings))
                .AddTo(disposable);
            
            _quitGameButton
                .OnClickAsObservable()
                .Subscribe(_ => QuitApplication())
                .AddTo(disposable);
        }

        private void QuitApplication()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}

