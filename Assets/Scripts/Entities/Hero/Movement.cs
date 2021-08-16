using System.Collections.Generic;
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
        [SerializeField] private float _jumpVelocity;
        [Space]
        [SerializeField] private float _minGroundNormalY = .65f;
        [SerializeField] private float _gravityModifier = 1f;

        private float _horizontalMove;
        private Vector2 _velocity;
        
        private bool _grounded;
        private Vector2 _groundNormal;
        
        private Rigidbody2D _rigidBody;
        
        private RaycastHit2D[] _hitBuffer = new RaycastHit2D[16];

        private const float _minMoveDistance = 0.001f;
        private const float _shellRadius = 0.01f;

        private Animations _animations;

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
            _horizontalMove = _moveAction.ReadValue<float>() * _speed;
            UpdateAnimation();
        }

        private void UpdateAnimation()
        {
            if (_horizontalMove != 0)
            {
                _animations.Run(_horizontalMove);
            }
            else
            {
                _animations.Stay();
            }
        }

        private void FixedUpdate()
        {
            ComputeVelocity();
            Move();
        }

        private void ComputeVelocity()
        {
            _velocity += Physics2D.gravity * (_gravityModifier * Time.deltaTime);
            _velocity.x = _horizontalMove;
        }

        private void Move()
        {
            _grounded = false;
            
            var deltaPosition = _velocity * Time.deltaTime;
            var moveAlongGround = new Vector2(_groundNormal.y, -_groundNormal.x);

            var horizontalMove = moveAlongGround * deltaPosition.x;
            MoveByAxis(horizontalMove, vertical: false);

            var verticalMove = Vector2.up * deltaPosition.y;
            MoveByAxis(verticalMove, vertical: true);
        }

        private void MoveByAxis(Vector2 move, bool vertical)
        {
            var distance = move.magnitude;

            if (distance < _minMoveDistance)
            {
                return;
            }
            
            distance = CalculateCollisions(distance, move, vertical);

            _rigidBody.position = _rigidBody.position + move.normalized * distance;
        }

        private float CalculateCollisions(float distance, Vector2 move, bool vertical)
        {
            var count = _rigidBody.Cast(move, _hitBuffer, distance + _shellRadius);

            for (var i = 0; i < count; i++)
            {
                var currentNormal = _hitBuffer[i].normal;
                if (currentNormal.y > _minGroundNormalY)
                {
                    _grounded = true;
                    if (vertical)
                    {
                        _groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                var projection = Vector2.Dot(_velocity, currentNormal);
                if (projection < 0)
                {
                    _velocity = _velocity - projection * currentNormal;
                }

                var modifiedDistance = _hitBuffer[i].distance - _shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }

            return distance;
        }

        private void Jump(InputAction.CallbackContext context)
        {
            if (_grounded)
            {
                _velocity.y = _jumpVelocity;
            }
        }
    }
}
