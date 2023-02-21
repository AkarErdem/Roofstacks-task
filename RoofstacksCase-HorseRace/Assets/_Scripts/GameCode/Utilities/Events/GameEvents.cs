using System;
using System.Collections.Generic;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace GameCode.Utilities.Events
{
    public static class GameEvents
    {
        public static event Action<Photon.Realtime.Player, Hashtable> PlayerPropertiesUpdate;
        
        public static event Action<string> CreateRoomFailed;
        
        public static event Action<List<RoomInfo>> RoomListUpdate;
        
        public static event Action Boost;

        public static void OnRoomListUpdate(List<RoomInfo> roomInfos)
        {
            RoomListUpdate?.Invoke(roomInfos);
        }
        
        public static void OnCreateRoomFailed(string message)
        {
            CreateRoomFailed?.Invoke(message);
        }

        public static void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
        {
            PlayerPropertiesUpdate?.Invoke(targetPlayer, changedProps);
        }

        public static void OnBoost()
        {
            Boost?.Invoke();
        }
    }
}
