using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Entities.Hero
{
    [RequireComponent(typeof(Animations))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Dashes : MonoBehaviour
    {
        public Vector2 CurrentVelocity { get; private set; }
        
        [SerializeField] private float _floorDashDistance;
        [SerializeField] private AnimationCurve _floorDashTrajectory;
        [SerializeField] private float _floorDashTime;
        [SerializeField] private float _alignThreshold;
        [Space]
        [SerializeField] private float _wallDashDistance;
        [SerializeField] private AnimationCurve _wallDashTrajectory;
        [SerializeField] private float _wallDashTime;
        [SerializeField] private int _wallDashMaxNumber;
        [Space]
        [SerializeField] private Camera _camera;

        private bool _dashing;
        private int _wallDashNumber;

        private Rigidbody2D _rigidBody;
        private Animations _animations;
        private Coroutine _startDash;

        private void Awake()
        {
            _animations = GetComponent<Animations>();
            _rigidBody = GetComponent<Rigidbody2D>();
        }

        public void FloorDash()
        {
            if (_dashing)
            {
                return;
            }

            ResetWallDashes();
            var direction = GetFloorDashDirection();
            _animations.FloorDash(direction);
            _startDash = StartCoroutine(StartFloorDash(direction));
        }

        private void ResetWallDashes()
        {
            _wallDashNumber = 0;
        }

        public void WallDash()
        {
            if (_dashing || _wallDashNumber >= _wallDashMaxNumber)
            {
                return;
            }

            _wallDashNumber++;

            var direction = GetWallDashDirection();
            _animations.WallDash(direction);
            _startDash = StartCoroutine(StartWallDash(direction));
        }

        public void CancelDash()
        {
            if (_dashing)
            {
                StopCoroutine(_startDash);
                StopFloorDash();
            }
        }

        private Vector2 GetFloorDashDirection()
        {
            var direction = GetHeroToCursor();
            direction = PruneDirectionsIntoFloor(direction);
            direction = Normalize(direction);
            direction = Align(direction);
            direction = Normalize(direction);
            direction = СhangeExceptionalDirectionToRight(direction);
            return direction;
        }

        private Vector2 GetWallDashDirection()
        {
            var direction = GetHeroToCursor();
            direction = Normalize(direction);
            direction = СhangeExceptionalDirectionToRight(direction);
            return direction;
        }

        private static Vector2 СhangeExceptionalDirectionToRight(Vector2 heroToCursor)
        {
            if (heroToCursor == Vector2.zero)
            {
                heroToCursor = Vector2.right;
            }
            return heroToCursor;
        }

        private Vector2 GetHeroToCursor()
        {
            var cursorPosition = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            var cursorPosition2d = new Vector2(cursorPosition.x, cursorPosition.y);
            var heroToCursor = cursorPosition2d - _rigidBody.position;
            return heroToCursor;
        }

        private static Vector2 PruneDirectionsIntoFloor(Vector2 heroToCursor)
        {
            if (heroToCursor.y < 0)
            {
                heroToCursor.y = 0;
            }
            return heroToCursor;
        }

        private static Vector2 Normalize(Vector2 heroToCursor)
        {
            heroToCursor = heroToCursor.normalized;
            return heroToCursor;
        }

        private Vector2 Align(Vector2 heroToCursor)
        {
            if (heroToCursor.y <= _alignThreshold)
            {
                heroToCursor.y = 0;
            }
            return heroToCursor;
        }

        private IEnumerator StartFloorDash(Vector2 direction)
        {
            _dashing = true;
            
            var time = 0f;
            var lastPosition = Vector2.zero;
            while (time <= _floorDashTime)
            {
                var position = ComputeFloorDashPosition(direction, time);
                UpdateVelocity(position, ref lastPosition);

                yield return new WaitForFixedUpdate();
                time += Time.fixedDeltaTime;
            }
            StopFloorDash();
        }

        private Vector2 ComputeFloorDashPosition(Vector2 direction, float time)
        {
            var progress = time / _floorDashTime;
            var delta = _floorDashDistance * _floorDashTrajectory.Evaluate(progress);
            var position = direction * delta;
            return position;
        }

        private void StopFloorDash()
        {
            CurrentVelocity = Vector2.zero;
            _animations.StopDash();
            _dashing = false;
        }
        
        private IEnumerator StartWallDash(Vector2 direction)
        {
            _dashing = true;
            
            var time = 0f;
            var lastPosition = Vector2.zero;
            while (time <= _wallDashTime)
            {
                var position = ComputeWallDashPosition(direction, time);
                UpdateVelocity(position, ref lastPosition);

                yield return new WaitForFixedUpdate();
                time += Time.fixedDeltaTime;
            }
            StopWallDash();
        }

        private Vector2 ComputeWallDashPosition(Vector2 direction, float time)
        {
            var progress = time / _wallDashTime;
            var delta = _wallDashDistance * _wallDashTrajectory.Evaluate(progress);
            var position = direction * delta;
            return position;
        }

        private void StopWallDash()
        {
            CurrentVelocity = Vector2.zero;
            _animations.StopDash();
            _dashing = false;
        }

        private void UpdateVelocity(Vector2 position, ref Vector2 lastPosition)
        {
            CurrentVelocity = (position - lastPosition) / Time.fixedDeltaTime;
            lastPosition = position;
        }
    }
}