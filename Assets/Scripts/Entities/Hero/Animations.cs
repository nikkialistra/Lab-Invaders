using UnityEngine;

namespace Entities.Hero
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Animations : MonoBehaviour
    {
        public bool Idle => _animator.GetCurrentAnimatorStateInfo(0).IsName("Idle");
        
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;

        private bool _falling;
        
        private readonly int _run = Animator.StringToHash("run");
        private readonly int _fall = Animator.StringToHash("fall");
        
        private readonly int _floorDash = Animator.StringToHash("floorDash");
        private readonly int _floorDashX = Animator.StringToHash("floorDashX");
        private readonly int _floorDashY = Animator.StringToHash("floorDashY");
        
        private readonly int _wallDash = Animator.StringToHash("wallDash");
        private readonly int _wallDashX = Animator.StringToHash("wallDashX");
        private readonly int _wallDashY = Animator.StringToHash("wallDashY");
        
        private readonly int _wallRun = Animator.StringToHash("wallRun");
        private readonly int _wallRunX = Animator.StringToHash("wallRunX");
        private readonly int _wallRunY = Animator.StringToHash("wallRunY");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Run(float moveDirection)
        {
            _animator.SetBool(_run, true);
            SetOrientation(moveDirection);
        }

        public void WallRun(Vector2 direction)
        {
            StopFall();
            SetOrientation(direction.x);
            _animator.SetBool(_wallRun, true);
            _animator.SetFloat(_wallRunX, direction.x);
            _animator.SetFloat(_wallRunY, direction.y);
        }

        private void SetOrientation(float orientation)
        {
            if (orientation < 0)
            {
                Flip(true);
            }
            if (orientation > 0)
            {
                Flip(false);
            }
        }

        private void Flip(bool value)
        {
            _spriteRenderer.flipX = value;
        }

        public void Stay()
        {
            _animator.SetBool(_run, false);
        }

        public void FloorDash(Vector2 direction)
        {
            SetOrientation(direction.x);
            _animator.SetBool(_floorDash, true);
            _animator.SetFloat(_floorDashX, direction.x);
            _animator.SetFloat(_floorDashY, direction.y);
        }

        public void WallDash(Vector2 direction)
        {
            SetOrientation(direction.x);
            _animator.SetBool(_wallDash, true);
            _animator.SetFloat(_wallDashX, direction.x);
            _animator.SetFloat(_wallDashY, direction.y);
        }

        public void StopDash()
        {
            _animator.SetBool(_floorDash, false);
            _animator.SetBool(_wallDash, false);
        }

        public void StopWallRun()
        {
            _animator.SetBool(_wallRun, false);
        }

        public void Fall()
        {
            _animator.SetBool(_fall, true);
        }

        private void StopFall()
        {
            _animator.SetBool(_fall, false);
        }

        public void Land()
        {
            _animator.SetBool(_fall, false);
        }
    }
}