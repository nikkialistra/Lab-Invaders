using UnityEngine;

namespace Entities.Hero
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Animations : MonoBehaviour
    {
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;

        private bool _falling;
        
        private readonly int _run = Animator.StringToHash("run");
        private readonly int _dash = Animator.StringToHash("dash");
        private readonly int _fall = Animator.StringToHash("fall");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Run(float moveDirection)
        {
            _animator.SetBool(_run, true);
            SetDirection(moveDirection);
        }
        
        private void SetDirection(float moveDirection)
        {
            if (moveDirection < 0)
            {
                Flip(true);
            }
            if (moveDirection > 0)
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

        public void Dash(float direction)
        {
            SetDirection(direction);
            _animator.SetBool(_dash, true);
        }

        public void StopDash()
        {
            _animator.SetBool(_dash, false);
        }

        public void Fall()
        {
            _animator.SetBool(_fall, true);
        }

        public void Land()
        {
            _animator.SetBool(_fall, false);
        }
    }
}