using UnityEngine;
using UnityEngine.InputSystem;

namespace Entities.Hero
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerInput))]
    public class Movement : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private float _jumpForce;

        private Rigidbody2D _rigidBody;

        private float _moveDirection;

        private PlayerInput _input;
        private InputAction _moveAction;
        private InputAction _jumpAction;

        private void Awake()
        {
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
        }

        private void Move()
        {
            _rigidBody.velocity = new Vector2(_moveDirection, 0) * (_speed * Time.fixedDeltaTime);
        }

        private void Jump(InputAction.CallbackContext context)
        {
            _rigidBody.AddForce(new Vector2(0, _jumpForce));
        }
    }
}
