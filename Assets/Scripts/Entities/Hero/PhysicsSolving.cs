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

        public bool Grounded { get; private set; }
        public bool Falling { get; private set; }

        private float _distance;
        
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

        public void MoveAcrossFloor(float distance)
        {
            _distance = distance;
        }

        private void ComputeVelocity()
        {
            _velocity += Physics2D.gravity * (_gravityModifier * Time.deltaTime);
            _velocity.x = _distance;
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