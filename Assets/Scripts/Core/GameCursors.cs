using UnityEngine;
using UnityEngine.InputSystem;

namespace Core
{
    public class GameCursors : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _mainCursor;
        [SerializeField] private SpriteRenderer _auxiliaryCursor;
        [Range(0, 1)]
        [SerializeField] private float _auxialiaryCursorOffset;
        [Space] 
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _heroTransform;

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
            var delta = _cursorPosition - _heroTransform.position;
            var offset = delta * _auxialiaryCursorOffset;
            _auxiliaryCursor.transform.position = _cursorPosition - offset;
        }
    }
}