using GameCode.Race;
using UnityEngine;

namespace GameCode.Player
{
    public class MovementInput : IMovementInput
    {
        private readonly RaceManager _raceManager;
        private Vector3 _movementInput = new(0, 0, 1);
        
        public bool CanMove => _raceManager.IsRaceStarted;

        public Vector3 GetMovement()
        {
            var movement = CanMove ? _movementInput : Vector3.zero;
            return movement;
        }

        public MovementInput(RaceManager raceManager)
        {
            _raceManager = raceManager;
        }
    }
}
