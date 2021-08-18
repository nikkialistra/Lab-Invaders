using System;
using UnityEngine;

namespace Entities.Hero
{
    [RequireComponent(typeof(Dashes))]
    [RequireComponent(typeof(Animations))]
    [RequireComponent(typeof(PhysicsSolving))]
    public class Movement : MonoBehaviour
    {
        [SerializeField] private float _speed;

        private Dashes _dashes;
        private Animations _animations;

        private PhysicsSolving _physicsSolving;

        private void Awake()
        {
            _dashes = GetComponent<Dashes>();
            _animations = GetComponent<Animations>();
            _physicsSolving = GetComponent<PhysicsSolving>();
        }

        private void Update()
        {
            if (_physicsSolving.Falling)
            {
                _animations.Fall();
            }
            else
            {
                _animations.Land();
            }
        }

        private void FixedUpdate()
        {
            _physicsSolving.MoveAcrossDash(_dashes.LastChange);
        }

        public void Move(Vector2 moveDirection)
        {
            if (_physicsSolving.Grounded)
            {
                MoveAcrossFloor(moveDirection.x);
            }
            else
            {
                MoveAcrossWall(moveDirection);
            }
        }

        private void MoveAcrossFloor(float move)
        {
            var distance = move * _speed;
            _physicsSolving.MoveAcrossFloor(distance);
            UpdateMoveAnimation(move);
        }

        private void UpdateMoveAnimation(float move)
        {
            if (move != 0)
            {
                _animations.Run(move);
            }
            else
            {
                _animations.Stay();
            }
        }

        private void MoveAcrossWall(Vector2 wallMove)
        {
            Debug.Log("wall move");
        }

        public void Dash()
        {
            _dashes.Dash();
        }
    }
}
