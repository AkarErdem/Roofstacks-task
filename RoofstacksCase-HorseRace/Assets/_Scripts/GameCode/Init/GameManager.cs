using System.Collections.Generic;
using GameCode.Utilities.Singleton;
using GameCode.Configuration;
using GameCode.Player;
using GameCode.PlayFab;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using UniRx;
using UnityEngine;

namespace GameCode.Init
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField] private GameConfig _gameConfig;
        
        private RoomOptions _roomOptions;
        private CompositeDisposable _disposable;
        private List<AudioSource> _sfx = new();
        private List<PlayerInfo> _playerInfosCache = new();
        
        public GameConfig GameConfig => _gameConfig;
        public RoomOptions RoomOptions => _roomOptions;
        public CompositeDisposable Disposable => _disposable;
        public List<PlayerInfo> PlayerInfosCache => _playerInfosCache;
        
        public List<AudioSource> SFX => _sfx;

        protected override void Awake()
        {
            base.Awake();
            
            if (Instance != this) return;
            
            // Disposable
            _disposable = new CompositeDisposable().AddTo(this);
            
            // FPS
            Application.targetFrameRate = _gameConfig.TargetFPS;
            
            // Photon
            _roomOptions = new RoomOptions
            {
                MaxPlayers = (byte)_gameConfig.MaxPlayerPerRoom,
                CleanupCacheOnLeave = false,
            };
            PhotonNetwork.ConnectUsingSettings();
            
            // PlayFab
            PlayFabSettings.TitleId = _gameConfig.TitleId;
            PlayFabManager.LoginAsGuest();
            
            // Music
            Instantiate(_gameConfig.GameMusic, transform).Play();
            
            // SFX
            foreach (var sfx in _gameConfig.SFX)
            {
                SFX.Add(Instantiate(sfx, transform));
            }
        }
    }
}
