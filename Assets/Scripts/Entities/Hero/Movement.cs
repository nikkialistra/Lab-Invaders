using UnityEngine;
using UnityEngine.InputSystem;

namespace Entities.Hero
{
    [RequireComponent(typeof(Animations))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerInput))]
    public class Movement : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private float _jumpForce;

        private Animations _animations;
        
        private Rigidbody2D _rigidBody;

        private float _moveDirection;

        private PlayerInput _input;
        private InputAction _moveAction;
        private InputAction _jumpAction;

        private void Awake()
        {
            _animations = GetComponent<Animations>();
            _rigidBody = GetComponent<Rigidbody2D>();
            _input = GetComponent<PlayerInput>();
            
            _moveAction = _input.actions.FindAction("Move");
            _jumpAction = _input.actions.FindAction("Jump");
        }

        private void OnEnable()
        {
            _jumpAction.started += Jump;
        }

        private void OnDisable()
        {
            _jumpAction.started -= Jump;
        }

        private void Update()
        {
            _moveDirection = _moveAction.ReadValue<float>();
        }

        private void FixedUpdate()
        {
            if (_moveDirection != 0)
            {
                Move();
            }
            else
            {
                Stay();
            }
        }

        private void Move()
        {
            _animations.Run(_moveDirection);
            _rigidBody.MovePosition(_rigidBody.position + new Vector2(_moveDirection * _speed * Time.fixedDeltaTime, 0));
        }

        private void Stay()
        {
            _animations.Stay();
        }

        private void Jump(InputAction.CallbackContext context)
        {
            _rigidBody.AddForce(new Vector2(0, _jumpForce));
        }
    }
}