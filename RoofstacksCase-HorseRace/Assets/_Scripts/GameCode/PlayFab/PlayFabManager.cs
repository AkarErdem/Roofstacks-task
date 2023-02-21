using System.Collections.Generic;
using GameCode.Data;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace GameCode.PlayFab
{
    public static class PlayFabManager
    {
        public static bool IsLoggedIn() => PlayFabClientAPI.IsClientLoggedIn();
        
        public static void LoginAsGuest()
        {
            var request = new LoginWithCustomIDRequest
            {
                CustomId = SystemInfo.deviceUniqueIdentifier,
                CreateAccount = true,
            };
            PlayFabClientAPI.LoginWithCustomID(request, 
                result =>
                {
                    Debug.Log("Successfully logged in!");
                    PhotonNetwork.NickName = result.PlayFabId;
                    SendLeaderboardRequest();
                }, 
                OnError
            );
        }

        public static void SendLeaderboardRequest()
        {
            var request = new GetLeaderboardRequest
            {
                StatisticName = ConstantData.PlayFabLeaderboardName,
                StartPosition = 0,
                MaxResultsCount = 10
            };
            PlayFabClientAPI.GetLeaderboard(request, 
                result =>
                {
                    Object.FindObjectOfType<LeaderboardMenu>()?.Create(result);
                }, (error) =>
                {
                    Debug.LogWarning(error.GenerateErrorReport());
                }
            );
        }
        
        public static void UpdatePlayerScore()
        {
            const int increaseAmount = 1;
            var request = new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate>
                {
                    new()
                    {
                        StatisticName = ConstantData.PlayFabLeaderboardName,
                        Value = increaseAmount
                    }
                }
            };
            
            PlayFabClientAPI.UpdatePlayerStatistics(request, 
                _ =>
                {
                    SendLeaderboardRequest();
                    Debug.Log("Leaderboard successfully updated!");
                }, 
                OnError
            );
        }
        
        private static void OnError(PlayFabError error)
        {
            Debug.LogWarning(error.GenerateErrorReport());
        }
    }
}
