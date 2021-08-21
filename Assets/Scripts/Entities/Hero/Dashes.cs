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
        
        [SerializeField] private float _distance;
        [SerializeField] private AnimationCurve _trajectory;
        [SerializeField] private float _time;
        [Space]
        [SerializeField] private float _alignThreshold;

        [Space]
        [SerializeField] private Camera _camera;

        private bool _dashing;
        
        private Rigidbody2D _rigidBody;
        private Animations _animations;
        private Coroutine _startDash;

        private void Awake()
        {
            _animations = GetComponent<Animations>();
            _rigidBody = GetComponent<Rigidbody2D>();
        }

        public void Dash()
        {
            if (_dashing)
            {
                return;
            }
            
            var direction = GetDirection();
            _animations.Dash(direction);
            _startDash = StartCoroutine(StartDash(direction));
        }

        public void CancelDash()
        {
            if (_dashing)
            {
                StopCoroutine(_startDash);
                StopDash();
            }
        }

        private Vector2 GetDirection()
        {
            var direction = GetHeroToCursor();
            direction = PruneDirectionsIntoFloor(direction);
            direction = Normalize(direction);
            direction = Align(direction);
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

        private IEnumerator StartDash(Vector2 direction)
        {
            _dashing = true;
            
            var time = 0f;
            var lastPosition = Vector2.zero;
            while (time <= _time)
            {
                var position = ComputePosition(direction, time);
                UpdateVelocity(position, ref lastPosition);

                yield return new WaitForFixedUpdate();
                time += Time.fixedDeltaTime;
            }
            StopDash();
        }

        private Vector2 ComputePosition(Vector2 direction, float time)
        {
            var progress = time / _time;
            var delta = _distance * _trajectory.Evaluate(progress);
            var position = direction * delta;
            return position;
        }

        private void UpdateVelocity(Vector2 position, ref Vector2 lastPosition)
        {
            CurrentVelocity = (position - lastPosition) / Time.fixedDeltaTime;
            lastPosition = position;
        }

        private void StopDash()
        {
            CurrentVelocity = Vector2.zero;
            _animations.StopDash();
            _dashing = false;
        }
    }
}