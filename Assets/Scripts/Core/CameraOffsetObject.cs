using Entities.Hero;
using UnityEngine;

namespace Core
{
    public class CameraOffsetObject : MonoBehaviour
    {
        [SerializeField] private Transform _hero;
        [SerializeField] private Animations _heroState;
        [SerializeField] private Transform _cursor;
        [Space] 
        [SerializeField] private float _offsetMultiplier;
        [Space] 
        [SerializeField] private float _transitionSpeed;

        private Vector3 _oldDifference;
        private Vector3 _oldHeroPosition;

        private void Update()
        {
            ChangePosition();
        }

        private void ChangePosition()
        {
            if (_heroState.Idle)
            {
                MoveWithCursor();
            }
            else
            {
                MoveWithHero();
            }
            SaveHeroPositionDifference();
        }

        private void MoveWithHero()
        {
            transform.position = _hero.position + _oldDifference;
        }

        private void MoveWithCursor()
        {
            var difference = _cursor.position - _hero.position;
            var offset = difference.normalized * _offsetMultiplier;
            var newPosition = _hero.position + offset;
            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * _transitionSpeed);
        }

        private void SaveHeroPositionDifference()
        {
            _oldDifference = transform.position - _hero.position;
        }
    }
}