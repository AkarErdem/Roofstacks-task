using System;
using System.Collections.Generic;
using System.Linq;
using GameCode.Configuration;
using GameCode.Data;
using GameCode.Hud;
using GameCode.Init;
using GameCode.Player;
using GameCode.SceneHandler;
using GameCode.Utilities.Events;
using Photon.Pun;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

namespace GameCode.Lobby
{
    public class RoomMenu : Menu
    {
        [Header("View Components")] 
        [SerializeField] private TMP_Text _roomNameText;
        [SerializeField] private TMP_Text _readyText;
        
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _returnButton;

        [SerializeField] private Transform _contentsParent;
        [SerializeField] private HorseListItem _prefabListItem;
        
        [SerializeField] private List<PlayerInfo> _playerInfos;

        private List<HorseConfig> _horseConfigs = new();
        private List<HorseListItem> _listItems = new();
        private List<Photon.Realtime.Player> _players = new();
        private IMenuManager _menuManager;
        
        public void RegisterObservables(IMenuManager menuManager, CompositeDisposable disposable)
        {
            _menuManager = menuManager;
            _horseConfigs = GameManager.Instance.GameConfig.HorseConfigs;
            
            Observable
                .EveryUpdate()
                .Where(_ => _startButton != null && _startButton.gameObject.activeSelf != PhotonNetwork.IsMasterClient)
                .Subscribe(_ => _startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient))
                .AddTo(disposable);
            
            Observable
                .EveryUpdate()
                .Where(_ => PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.Name != _roomNameText.text)
                .Subscribe(_ => _roomNameText.SetText($"Room\n{PhotonNetwork.CurrentRoom.Name}"))
                .AddTo(disposable);
            
            Observable
                .EveryUpdate()
                .Where(_ => _players != null && _players.Count != PhotonNetwork.PlayerList.Length)
                .Subscribe(_ =>
                {
                    SetPlayers();
                })
                .AddTo(disposable);
            
            Observable
                .EveryUpdate()
                .Where(_ => _startButton != null)
                .Subscribe(_ =>
                {
                    _startButton.interactable = _players.Count == GameManager.Instance.GameConfig.MaxPlayersPerRoom &&
                                                !string.IsNullOrWhiteSpace(_playerInfos[0].PrefabName) &&
                                                !string.IsNullOrWhiteSpace(_playerInfos[1].PrefabName);
                })
                .AddTo(disposable);
            
            _startButton
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    Debug.Log("Preparing the game");
                    if (PhotonNetwork.LocalPlayer.IsMasterClient)
                    {
                        PhotonNetwork.DestroyAll();
                    }
                    PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "OpenMenu", MenuType.Loading } });
                    SceneLoader.LoadScene(GameManager.Instance.GameConfig.GameSceneName, SceneLoadType.Photon);
                })
                .AddTo(disposable);

            _returnButton
                .OnClickAsObservable()
                .Subscribe(_ =>
                {
                    menuManager.OpenMenu(MenuType.Loading);
                    PhotonNetwork.LeaveRoom();
                })
                .AddTo(disposable);

            CreatePrefabContents(disposable);
            
            GameEvents.PlayerPropertiesUpdate += OnPlayerPropertiesUpdate;
        }

        private void OnEnable()
        {
            Invoke(nameof(OnListItemClick), .1f);
        }

        private void OnDestroy()
        {
            GameEvents.PlayerPropertiesUpdate -= OnPlayerPropertiesUpdate;
        }

        private void SetPlayers()
        {
            _players = PhotonNetwork.PlayerList.ToList();
            
            bool player1Active = false;
            bool player2Active = false;

            for (var i = 0; i < _players.Count; i++)
            {
                var player = _players[i];
                if (player.IsMasterClient && !player1Active)
                {
                    _playerInfos[0].SetPlayerText($"Player\n{player.NickName}");
                    player1Active = true;
                }
                else if (!player2Active)
                {
                    _playerInfos[1].SetPlayerText($"Player\n{player.NickName}");
                    player2Active = true;
                }

                if (i < _playerInfos.Count)
                {
                    _playerInfos[i].SetUserId(player.UserId);
                }
                
                if (player1Active && player2Active)
                {
                    break;
                }
            }

            if (!player1Active)
                _playerInfos[0].DisablePlayerInfo();
            
            if (!player2Active)
                _playerInfos[1].DisablePlayerInfo();
            
            if (PhotonNetwork.IsMasterClient && _readyText.gameObject.activeSelf)
                _readyText.text = string.Empty;

            SendDataToNewPlayer();
        }

        private void CreatePrefabContents(CompositeDisposable disposable)
        {
            foreach (var horseConfig in _horseConfigs)
            {
                var listItem = Instantiate(_prefabListItem, _contentsParent);
                listItem.HorseName.SetText(horseConfig.Name);
                listItem.HorseBackground.color = horseConfig.Color;
                listItem.HorseIcon.sprite = horseConfig.Icon;
                
                var config = horseConfig;
                listItem.SelectButton
                    .OnClickAsObservable()
                    .Where(_ => listItem.SelectButton != null && 
                                gameObject.activeInHierarchy)
                    .Subscribe(_ =>
                    {
                        OnListItemClick(config, listItem);
                    })
                    .AddTo(disposable);
                
                _listItems.Add(listItem);
            }
        }

        public void OnListItemClick()
        {
            var startingIndex = Random.Range(0, _horseConfigs.Count);
            OnListItemClick(_horseConfigs[startingIndex], _listItems[startingIndex]);
        }
        private void OnListItemClick(HorseConfig config, HorseListItem listItem)
        {
            OnHorseListItemSelected(listItem);
            var userId = PhotonNetwork.LocalPlayer.UserId;
            for (var i = 0; i < _playerInfos.Count; i++)
            {
                var _playerInfo = _playerInfos[i];
                if (_playerInfo.UserId != userId) continue;
                
                var hash = new Hashtable
                {
                    { "Name", config.Name },
                    { "FromPlayerId", userId },
                    { "PlayerInfoIndex", i },
                    { "PrefabName", _playerInfo.PrefabName }
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
                _playerInfo.SpawnPrefab(config);
                break;
            }

            if (!PhotonNetwork.IsMasterClient)
            {
                chosen++;
                Debug.Log(chosen);
                if (chosen >= 3)
                {
                    _readyText.SetText("Ready");
                }
            }
        }

        private int chosen = 0;
        private void SendDataToNewPlayer()
        {
            var userId = PhotonNetwork.LocalPlayer.UserId;
            for (var i = 0; i < _playerInfos.Count; i++)
            {
                var _playerInfo = _playerInfos[i];
                if (_playerInfo.UserId != userId) continue;

                var hash = new Hashtable
                {
                    { "Name", _playerInfo.HorseName },
                    { "FromPlayerId", userId },
                    { "PlayerInfoIndex", i },
                    { "PrefabName", _playerInfo.PrefabName }
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
                
                break;
            }
        }

        private void OnHorseListItemSelected(HorseListItem selectedListItem)
        {
            foreach (var listItem in _listItems)
            {
                listItem.SelectButton.interactable = listItem.SelectButton != selectedListItem.SelectButton;
            }
        }

        private void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
        {
            if (!gameObject.activeInHierarchy || changedProps == null) return;
            
            if (changedProps.ContainsKey("OpenMenu"))
            {
                _menuManager.OpenMenu((MenuType)changedProps["OpenMenu"]);
                return;
            }

            if (!changedProps.ContainsKey("Name") ||
                !changedProps.ContainsKey("FromPlayerId") ||
                !changedProps.ContainsKey("PlayerInfoIndex") ||
                !changedProps.ContainsKey("PrefabName"))
            {
                Debug.LogWarning($"Missing keys on the changedProps {changedProps}");
                return;
            }

            if (targetPlayer.UserId != PhotonNetwork.LocalPlayer.UserId)
            {
                var horseName = (string)changedProps["Name"];
                var fromPlayerId = (string)changedProps["FromPlayerId"];
                var playerInfoIndex = (int)changedProps["PlayerInfoIndex"];
                var prefabName = (string)changedProps["PrefabName"];
            
                if (!string.IsNullOrWhiteSpace(horseName))
                    _playerInfos[playerInfoIndex].SetHorseText(horseName);
                if (!string.IsNullOrWhiteSpace(fromPlayerId))
                    _playerInfos[playerInfoIndex].SetUserId(fromPlayerId);
                if (!string.IsNullOrWhiteSpace(prefabName))
                    _playerInfos[playerInfoIndex].SetPrefabName(prefabName);
                
                var playerInfosCache = GameManager.Instance.PlayerInfosCache;
                playerInfosCache.Clear();
                foreach (var playerInfo in _playerInfos)
                {
                    playerInfosCache.Add(playerInfo);
                }
            }
        }
    }
}
