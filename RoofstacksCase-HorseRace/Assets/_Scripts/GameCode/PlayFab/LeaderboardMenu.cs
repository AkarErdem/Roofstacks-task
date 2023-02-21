using System.Collections.Generic;
using GameCode.Data;
using GameCode.Hud;
using UnityEngine;
using PlayFab.ClientModels;
using UniRx;
using UnityEngine.UI;

namespace GameCode.PlayFab
{
    public class LeaderboardMenu : Menu
    {
        [SerializeField] private LeaderboardItem _leaderboardItemPrefab;
        [SerializeField] private Transform _leaderboardItemsParent;
        [SerializeField] private Button _returnButton;

        private List<LeaderboardItem> _leaderboardItems = new();
        
        public void RegisterObservables(IMenuManager menuManager, CompositeDisposable disposable)
        {
            _returnButton
                .OnClickAsObservable()
                .Subscribe(_ => menuManager.OpenMenu(MenuType.Title))
                .AddTo(disposable);

            var go = gameObject;
            go.ObserveEveryValueChanged(go => go.activeSelf)
                .Subscribe(_ =>
                {
                    if (gameObject.activeSelf)
                    {
                        PlayFabManager.SendLeaderboardRequest();
                    }
                })
                .AddTo(disposable);
        }

        public void Create(GetLeaderboardResult result)
        {
            foreach (var leaderboardItem in _leaderboardItems)
            {
                Destroy(leaderboardItem.gameObject);
            }

            foreach (var playerLeaderboardEntry in result.Leaderboard)
            {
                var leaderboardItem = Instantiate(_leaderboardItemPrefab, _leaderboardItemsParent);

                var rank = $"{playerLeaderboardEntry.Position + 1}.";
                var nickname = $"Player{playerLeaderboardEntry.PlayFabId}";
                var score = $"{playerLeaderboardEntry.StatValue}";
                
                leaderboardItem.SetTexts(rank, nickname, score);
                _leaderboardItems.Add(leaderboardItem);
            }
        }
    }
}
