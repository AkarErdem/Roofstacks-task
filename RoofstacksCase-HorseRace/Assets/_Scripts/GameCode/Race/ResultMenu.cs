using GameCode.Data;
using GameCode.Hud;
using GameCode.Init;
using GameCode.SceneHandler;
using Photon.Pun;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GameCode.Race
{
    public class ResultMenu : Menu
    {
        [SerializeField] private TMP_Text resultText;
        [SerializeField] private Button _returnButton;
        
        private Color _winColor = Color.green;
        private Color _loseColor = Color.red;
        
        public void RegisterObservables(IMenuManager menuManager, CompositeDisposable disposable)
        {
            _returnButton
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    menuManager.OpenMenu(MenuType.Loading);
                    
                    if (PhotonNetwork.CurrentRoom != null)
                    {
                        PhotonNetwork.LeaveRoom();
                    }
                    if (PhotonNetwork.LocalPlayer.IsMasterClient)
                    {
                        PhotonNetwork.DestroyAll();
                    }
                    SceneLoader.LoadScene(GameManager.Instance.GameConfig.LobbySceneName, SceneLoadType.Default);
                })
                .AddTo(disposable);
        }
        public void SetResult(bool isWin)
        {
            var result = !isWin ? "You Lose" : "You Win!";
            var resultColor = isWin ? _winColor: _loseColor;
            
            resultText.SetText(result);
            resultText.color = resultColor;
        }
    }
}
