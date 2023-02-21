using GameCode.Configuration;
using GameCode.Init;
using GameCode.Utilities.Events;
using Photon.Pun;
using UniRx;
using UnityEngine;

namespace GameCode.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PhotonView _photonView;
        [SerializeField] private Animator _horseAnim;

        private HorseConfig _horseConfig;
        private Vector3 _movement;
        private CompositeDisposable _disposable;
        private int _moveFrame;
        private int _boostClickedCount;
        private bool _isRunning;
        private static readonly int IsRunning = Animator.StringToHash("isRunning");
        
        public PhotonView PhotonView => _photonView;
        public Vector3 HorsePosition => transform.position;

        public void RegisterObservables(IMovementInput movementInput, HorseConfig horseConfig)
        {
            _horseConfig = horseConfig;
            _disposable = GameManager.Instance.Disposable;
            
            Observable.EveryUpdate()
                .Where(_ => _photonView != null)// && _photonView.IsMine)
                .Subscribe(_ =>
                {
                    if (_photonView.IsMine)
                    {
                        _movement = movementInput.GetMovement();
                        Behave();
                    }
                })
                .AddTo(_disposable);

            GameEvents.Boost += IncreaseBoost;
        }

        private void Behave()
        {
            Animate();
            Move();
        }
        
        private void Animate()
        {
            bool isRunningThisFrame = _movement != Vector3.zero;
            
            if (isRunningThisFrame == _isRunning) return;
            
            _isRunning = isRunningThisFrame;
            _horseAnim.SetBool(IsRunning, _isRunning);
        }
        
        private void Move()
        {
            var speed = _horseConfig.Speed + _horseConfig.BoostSpeed * _boostClickedCount;
            transform.position += _movement * (Time.deltaTime * speed);
            
            _moveFrame++;
            if (_moveFrame > 15)
            {
                _moveFrame = 0;
                _boostClickedCount = 0;
            }
        }

        private void IncreaseBoost() => _boostClickedCount++;
    }
}
