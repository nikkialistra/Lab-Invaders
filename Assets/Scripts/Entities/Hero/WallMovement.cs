using UnityEngine;

namespace Entities.Hero
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class WallMovement : MonoBehaviour
    {
        [SerializeField] private LayerMask _wallMask;

        private Rigidbody2D _rigidBody;

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
        }

        public bool IsWallBehind()
        {
            return Physics2D.OverlapCircle(_rigidBody.position, 1f, _wallMask) != null;
        }
    }
}