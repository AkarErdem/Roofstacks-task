using GameCode.Configuration;
using Photon.Pun;
using UniRx;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
    
namespace GameCode.Player
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] private PhotonView _pv;
        private PlayerController _controller;
        private bool _raceEndEventCalled;
        
        /// <summary>
        /// Instantiate and register Player Controller
        /// </summary>
        public void CreateController(IMovementInput movementInput, HorseConfig horseConfig, Vector3 spawnPos, Vector3 raceEndPos, CompositeDisposable disposable)
        {
            if (!_pv.IsMine) 
                return;

            _controller = PhotonNetwork.Instantiate(HorseConfig.GetHorsePrefabPath(horseConfig.PrefabName), 
                    spawnPos, 
                    Quaternion.identity)
                .GetComponent<PlayerController>();

            _controller.RegisterObservables(movementInput, horseConfig);

            _controller
                .ObserveEveryValueChanged(c => c.HorsePosition)
                .Where(_ => !_raceEndEventCalled)
                .Subscribe(controllerPos =>
                {
                    if (!(controllerPos.z >= raceEndPos.z))
                    {
                        return;
                    }
                    _raceEndEventCalled = true;
                    
                    var hash = new Hashtable
                    {
                        { "RaceEnd", PhotonNetwork.LocalPlayer.UserId },
                    };
                    PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
                })
                .AddTo(disposable);
        }
    }
}

