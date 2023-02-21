using System.IO;

namespace GameCode.Data
{
    public static class ConstantData
    {
        public const string PlayFabLeaderboardName = "TotalWin";
        
        public const string PhotonPrefabsFolderName = "PhotonPrefabs";   
        public const string HorsePrefabsFolderName = "HorsePrefabs";

        public static string PlayerManagerPath => Path.Combine(PhotonPrefabsFolderName, "PlayerManager");
    }
}
