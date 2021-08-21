using UnityEngine;
using UnityEngine.Tilemaps;

namespace Entities.Hero
{
    [RequireComponent(typeof(Animations))]
    public class WallMovement : MonoBehaviour
    {
        [SerializeField] private Tilemap _wallTilemap;
        
        public Vector2 CurrentVelocity { get; private set; }

        private Animations _animations;

        private void Awake()
        {
            _animations = GetComponent<Animations>();
        }

        public bool IsWallBehind()
        {
            return _wallTilemap.GetTile(Vector3Int.FloorToInt(transform.position)) != null;
        }

        public void Move(Vector2 direction)
        {
            CurrentVelocity = direction;
            _animations.WallRun(direction.x);
        }
    }
}