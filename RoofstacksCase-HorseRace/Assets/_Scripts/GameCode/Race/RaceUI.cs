using GameCode.Hud;
using GameCode.Init;
using TMPro;
using UnityEngine;

namespace GameCode.Race
{
    public class RaceUI : MenuManager
    {
        [SerializeField] private ResultMenu _resultMenu;
        [SerializeField] private GameplayMenu _gameplayMenu;
        [SerializeField] private TMP_Text _countdownText;
        
        private void Start()
        {
            _resultMenu.RegisterObservables(this, GameManager.Instance.Disposable);
            _gameplayMenu.RegisterObservables(this, GameManager.Instance.Disposable);
        }
        
        public void SetResult(bool isWin)
        {
            _resultMenu.SetResult(isWin);
        }
        
        public void SetCooldownText(string message)
        {
            _countdownText.SetText(message);
        }
    }
}
