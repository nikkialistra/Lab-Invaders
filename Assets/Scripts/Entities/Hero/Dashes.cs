using UnityEngine;
using UnityEngine.InputSystem;

namespace Entities.Hero
{
    [RequireComponent(typeof(Animations))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Dashes : MonoBehaviour
    {
        [SerializeField] private float _velocity;
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
            _rigidBody.velocity = direction * _velocity;
            _animations.Dash(direction.x);
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