using UnityEngine;

namespace Entities.Hero
{
    [RequireComponent(typeof(Dashes))]
    [RequireComponent(typeof(Animations))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Movement : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [Space]
        [SerializeField] private float _minGroundNormalY = .65f;
        [SerializeField] private float _gravityModifier = 1f;
        [Space]
        [SerializeField] private float _minMoveDistance = 0.001f;
        [SerializeField] private float _minFallDistance = -0.01f;
        [SerializeField] private float _shellRadius = 0.01f;

        private float _floorMove;
        
        private Vector2 _velocity;
        
        private bool _grounded;
        private Vector2 _groundNormal;
        
        private Dashes _dashes;
        private Animations _animations;
        
        private Rigidbody2D _rigidBody;
        private readonly RaycastHit2D[] _hitBuffer = new RaycastHit2D[16];

        private void Awake()
        {
            _dashes = GetComponent<Dashes>();
            _animations = GetComponent<Animations>();
            _rigidBody = GetComponent<Rigidbody2D>();
        }

        public void Move(Vector2 moveDirection)
        {
            if (_grounded)
            {
                MoveAcrossFloor(moveDirection.x);
            }
            else
            {
                MoveAcrossWall(moveDirection);
            }
        }

        private void MoveAcrossFloor(float floorMove)
        {
            _floorMove = floorMove * _speed;
            
            if (_floorMove != 0)
            {
                _animations.Run(_floorMove);
            }
            else
            {
                _animations.Stay();
            }
        }

        private void MoveAcrossWall(Vector2 wallMove)
        {
            Debug.Log("wall");
        }

        private void FixedUpdate()
        {
            ComputeVelocity();
            Move();
        }

        private void ComputeVelocity()
        {
            _velocity += Physics2D.gravity * (_gravityModifier * Time.deltaTime);
            _velocity.x = _floorMove;
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
            var deltaPosition = move.normalized * distance;
            _rigidBody.position = _rigidBody.position + deltaPosition;
            
            FallIfNeeded(deltaPosition.y);
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

        private void FallIfNeeded(float deltaYPosition)
        {
            if (deltaYPosition < _minFallDistance)
            {
                _animations.Fall();
            }
            else
            {
                _animations.Land();
            }
        }

        public void Dash()
        {
            _dashes.Dash();
        }
    }
}
