using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Entities.Hero
{
    [RequireComponent(typeof(Animations))]
    public class WallMovement : MonoBehaviour
    {
        [SerializeField] private Tilemap _wallTilemap;
        [SerializeField] private float _timeToRun;

        public Action RunStarted;

        public Vector2 CurrentVelocity { get; private set; }

        private bool _wasWallRun;
        private float _startRunTime;
        private bool _isRunTimeout;

        private Animations _animations;

        private void Awake()
        {
            _animations = GetComponent<Animations>();
        }

        public bool IsWallBehind()
        {
            return _wallTilemap.GetTile(Vector3Int.FloorToInt(transform.position)) != null;
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
            CurrentVelocity = direction;
            _animations.WallRun(direction.x);

            FinishRunUnderConditions();
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
            if (Time.time - _startRunTime >= _timeToRun)
            {
                FinishRun();
            }
        }

        private void FinishRun()
        {
            _isRunTimeout = true;
            CurrentVelocity = Vector2.zero;
            _animations.StopWallRun();
        }

        private void StartRun()
        {
            _wasWallRun = true;
            _startRunTime = Time.time;
            RunStarted?.Invoke();
        }

        public void ResetWallRun()
        {
            _wasWallRun = false;
            _isRunTimeout = false;
        }
    }
}