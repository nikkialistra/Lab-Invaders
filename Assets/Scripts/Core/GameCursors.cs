using UnityEngine;
using UnityEngine.InputSystem;

namespace Core
{
    public class GameCursors : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _mainCursor;
        [SerializeField] private SpriteRenderer _auxiliaryCursor;
        [Space]
        [SerializeField] private float _auxiliaryCursorOffsetMultiplier;
        [Space]
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _hero;

        private Vector3 _cursorPosition;

        private void Awake()
        {
            Cursor.visible = false;
        }

        private void Update()
        {
            GetCursorPosition();
            PlaceMainCursor();
            PlaceAuxiliaryCursor();
        }

        private void GetCursorPosition()
        {
            _cursorPosition = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            _cursorPosition.z = 0;
        }

        private void PlaceMainCursor()
        {
            _mainCursor.transform.position = _cursorPosition;
        }

        private void PlaceAuxiliaryCursor()
        {
            var heroToCursor = _cursorPosition - _hero.position;
            if (heroToCursor.magnitude <= 1)
            {
                _auxiliaryCursor.transform.position = _hero.position;
            }
            else
            {
                var offset = heroToCursor.normalized * _auxiliaryCursorOffsetMultiplier;
                _auxiliaryCursor.transform.position = _cursorPosition - offset;
            }
        }
    }
}