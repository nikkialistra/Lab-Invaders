using System;
using UnityEngine;

namespace Entities.Hero
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PhysicsSolving : MonoBehaviour
    {
        [SerializeField] private float _minGroundNormalY = .65f;
        [SerializeField] private float _gravityModifier = 1f;
        [Space]
        [SerializeField] private float _minMoveDistance = 0.001f;
        [SerializeField] private float _minFallDistance = -0.03f;
        [SerializeField] private float _shellRadius = 0.01f;

        public Action Ground;

        public bool Grounded { get; private set; }
        public bool Falling { get; private set; }

        private float _floorVelocity;
        private Vector2 _dashVelocity;
        private Vector2 _wallVelocity;

        private Vector2 _velocity;

        private Vector2 _groundNormal;

        private Rigidbody2D _rigidBody;
        private readonly RaycastHit2D[] _hitBuffer = new RaycastHit2D[16];

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            ComputeVelocity();
            Move();
        }

        public void MoveAcrossFloor(float velocity)
        {
            _floorVelocity = velocity;
        }

        public void MoveAcrossDash(Vector2 velocity)
        {
            _dashVelocity = velocity;
        }

        public void MoveAcrossWall(Vector2 velocity)
        {
            _wallVelocity = velocity;
        }

        private void ComputeVelocity()
        {
            if (_dashVelocity != Vector2.zero)
            {
                AddDashVelocity();
            } else if (_wallVelocity != Vector2.zero)
            {
                AddWallVelocity();
            }
            else
            {
                AddFloorVelocity();
                AddGravity();
            }
        }

        private void AddDashVelocity()
        {
            _velocity = _dashVelocity;
        }

        private void AddWallVelocity()
        {
            // Debug.Log(_velocity);
            _velocity = _wallVelocity;
        }

        private void AddFloorVelocity()
        {
            _velocity.x = _floorVelocity;
        }

        private void AddGravity()
        {
            if (_velocity.y > 0)
            {
                _velocity.y = 0;
            }
            _velocity += Physics2D.gravity * (_gravityModifier * Time.deltaTime);
        }

        private void Move()
        {
            Grounded = false;
            
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
            var deltaPosition = move.normalized * distance;
            _rigidBody.position += deltaPosition;
            
            UpdateFallingFlag(deltaPosition.y);
        }

        private void UpdateFallingFlag(float deltaPositionY)
        {
            Falling = deltaPositionY < _minFallDistance;
        }

        private float CalculateCollisions(float distance, Vector2 move, bool vertical)
        {
            var count = _rigidBody.Cast(move, _hitBuffer, distance + _shellRadius);

            for (var i = 0; i < count; i++)
            {
                var currentNormal = _hitBuffer[i].normal;
                if (currentNormal.y > _minGroundNormalY)
                {
                    Grounded = true;
                    Ground?.Invoke();
                    if (vertical)
                    {
                        _groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                var projection = Vector2.Dot(_velocity, currentNormal);
                if (projection < 0)
                {
                    _velocity -= projection * currentNormal;
                }

                var modifiedDistance = _hitBuffer[i].distance - _shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }

            return distance;
        }
    }
}