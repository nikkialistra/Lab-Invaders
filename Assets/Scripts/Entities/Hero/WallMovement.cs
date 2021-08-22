using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Entities.Hero
{
    [RequireComponent(typeof(Animations))]
    public class WallMovement : MonoBehaviour
    {
        [SerializeField] private Tilemap _wallTilemap;
        [Space] 
        [SerializeField] private float _upSpeedMultiplier;
        [SerializeField] private float _downSpeedMultiplier;
        [Space]
        [SerializeField] private float _timeToRun;
        [SerializeField] private float _timeToStay;
        [SerializeField] private Vector2 _stayFallSpeed;
        [Space]
        [SerializeField] private float _speedForStayMultiplier;
        [SerializeField] private float _inertiaMultiplier;
        [Space]
        [SerializeField] private float _directionChangePenalty;

        public Action RunStarted;
        public Action<float> RunFinished;

        public Vector2 CurrentVelocity { get; private set; }
        public bool Running => CurrentVelocity != Vector2.zero;

        private bool _wasWallRun;
        private float _runStartTime;
        private float _stayStartTime = float.MaxValue;
        private bool _isRunTimeout;

        private Vector2 _lastDirection;

        private Animations _animations;

        private void Awake()
        {
            _animations = GetComponent<Animations>();
        }

        public void TryRun(Vector2 direction)
        {
            if (ShouldCancel())
            {
                return;
            }

            if (!_wasWallRun)
            {
                StartRun();
            }

            _animations.WallRun(direction);
            FineForDirectionChange(direction);
            ComputeVelocity(direction);
            
            _stayStartTime = float.MaxValue;
            _lastDirection = direction;
            FinishRunUnderConditions();
        }

        private void FineForDirectionChange(Vector2 direction)
        {
            if (_lastDirection != direction)
            {
                _runStartTime -= _directionChangePenalty;
            }
        }

        private void ComputeVelocity(Vector2 direction)
        {
            if (direction.y > 0)
            {
                direction.y *= _upSpeedMultiplier;
            }
            if (direction.y < 0)
            {
                direction.y *= _downSpeedMultiplier;
            }
            CurrentVelocity = direction;
        }

        public void TryStayAtRun()
        {
            if (!_wasWallRun || ShouldCancel())
            {
                return;
            }

            if (StayTimeNotStarted())
            {
                _stayStartTime = Time.time;
                CurrentVelocity *= _speedForStayMultiplier;
            }
            
            FinishRunUnderConditions();
        }

        private bool StayTimeNotStarted()
        {
            return Math.Abs(_stayStartTime - float.MaxValue) < 1f;
        }

        private bool IsWallBehind()
        {
            return _wallTilemap.GetTile(Vector3Int.FloorToInt(transform.position)) != null;
        }

        private bool ShouldCancel()
        {
            if (_isRunTimeout)
            {
                return true;
            }
            if (!IsWallBehind())
            {
                FinishRun();
                return true;
            }

            return false;
        }

        private void FinishRunUnderConditions()
        {
            FinishOnRunTimeout();
            FinishOnStayTimeout();
        }

        private void FinishOnRunTimeout()
        {
            if (Time.time - _runStartTime >= _timeToRun)
            {
                FinishRun();
            }
        }

        private void FinishOnStayTimeout()
        {
            if (Time.time - _stayStartTime >= _timeToStay)
            {
                FinishRun();
            }
        }

        private void FinishRun()
        {
            _isRunTimeout = true;
            RunFinished?.Invoke(CurrentVelocity.x * _inertiaMultiplier);
            CurrentVelocity = Vector2.zero;
            _animations.StopWallRun();
        }

        private void StartRun()
        {
            _wasWallRun = true;
            _runStartTime = Time.time;
            _stayStartTime = float.MaxValue;
            RunStarted?.Invoke();
        }

        public void TryFinishRun()
        {
            if (CurrentVelocity != Vector2.zero)
            {
                FinishRun();
            }
        }

        public void ResetWallRun()
        {
            _wasWallRun = false;
            _isRunTimeout = false;
        }
    }
}