using GameCode.Data;
using GameCode.Hud;
using GameCode.Init;
using GameCode.SceneHandler;
using GameCode.Utilities.Events;
using Photon.Pun;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GameCode.Race
{
    public class GameplayMenu : Menu
    {
        [SerializeField] private Button _boostButton;
        
        public void RegisterObservables(IMenuManager menuManager, CompositeDisposable disposable)
        {
            _boostButton
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    GameEvents.OnBoost();
                })
                .AddTo(disposable);
        }
    }
}