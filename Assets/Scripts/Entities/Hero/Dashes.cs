using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Entities.Hero
{
    [RequireComponent(typeof(Animations))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Dashes : MonoBehaviour
    {
        [SerializeField] private float _distance;
        [SerializeField] private AnimationCurve _animation;
        [SerializeField] private float _animationTime;

        [Space]
        [SerializeField] private Camera _camera;
        
        private Rigidbody2D _rigidBody;
        private Animations _animations;

        private void Awake()
        {
            _animations = GetComponent<Animations>();
            _rigidBody = GetComponent<Rigidbody2D>();
        }

        public void Dash()
        {
            var direction = GetDirection();
            _animations.Dash(direction.x);
            StartCoroutine(AnimateDash(direction));
        }

        private IEnumerator AnimateDash(Vector2 direction)
        {
            var time = 0f;
            var originalPosition = _rigidBody.position;
            while (time <= _animationTime)
            {
                var progress = time / _animationTime;
                var delta = _distance * _animation.Evaluate(progress);
                _rigidBody.position = originalPosition + direction * delta;
                
                yield return new WaitForFixedUpdate();
                time += Time.fixedDeltaTime;
            }
            _animations.StopDash();
        }

        private Vector2 GetDirection()
        {
            var cursorPosition = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            var cursorPosition2d = new Vector2(cursorPosition.x, cursorPosition.y);
            var heroToCursorDirection = (cursorPosition2d - _rigidBody.position).normalized;
            return heroToCursorDirection;
        }
    }
}