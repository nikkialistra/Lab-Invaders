using System;
using UnityEngine;

namespace Entities.Hero
{
    [RequireComponent(typeof(Dashes))]
    [RequireComponent(typeof(WallMovement))]
    [RequireComponent(typeof(Animations))]
    [RequireComponent(typeof(PhysicsSolving))]
    public class Movement : MonoBehaviour
    {
        [SerializeField] private float _floorSpeed;
        [SerializeField] private float _wallSpeed;

        private Dashes _dashes;
        private WallMovement _wallMovement;
        private Animations _animations;
        private PhysicsSolving _physicsSolving;

        private void Awake()
        {
            _dashes = GetComponent<Dashes>();
            _wallMovement = GetComponent<WallMovement>();
            _animations = GetComponent<Animations>();
            _physicsSolving = GetComponent<PhysicsSolving>();
        }

        private void OnEnable()
        {
            _physicsSolving.Ground += OnGround;
            _wallMovement.RunStarted += OnRunStarted;
        }

        private void OnDisable()
        {
            _physicsSolving.Ground -= OnGround;
            _wallMovement.RunStarted -= OnRunStarted;
        }

        private void Update()
        {
            if (_physicsSolving.Falling)
            {
                _animations.Fall();
            }
            else
            {
                _animations.Land();
            }
        }

        private void FixedUpdate()
        {
            _physicsSolving.MoveAcrossDash(_dashes.CurrentVelocity);
            _physicsSolving.MoveAcrossWall(_wallMovement.CurrentVelocity);
        }

        private void OnGround()
        {
            _wallMovement.ResetWallRun();
        }

        private void OnRunStarted()
        {
            _dashes.CancelDash();
        }

        public void Run(Vector2 moveDirection)
        {
            if (_physicsSolving.Grounded)
            {
                RunAcrossFloor(moveDirection.x);
            }
            else
            {
                RunAcrossWall(moveDirection);
            }
        }

        private void RunAcrossFloor(float direction)
        {
            var velocity = direction * _floorSpeed;
            _physicsSolving.MoveAcrossFloor(velocity);
            UpdateMoveAnimation(direction);
        }

        private void UpdateMoveAnimation(float direction)
        {
            if (direction != 0)
            {
                _animations.Run(direction);
            }
            else
            {
                _animations.Stay();
            }
        }

        private void RunAcrossWall(Vector2 direction)
        {
            if (direction != Vector2.zero)
            {
                var velocity = direction * _wallSpeed;
                _wallMovement.TryRun(velocity);
            }
            else
            {
                _wallMovement.TryStayAtRun();
            }
        }

        public void Dash()
        {
            if (_physicsSolving.Grounded)
            {
                _dashes.Dash();
            }
        }
    }
}
