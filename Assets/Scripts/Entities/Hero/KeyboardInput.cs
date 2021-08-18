using UnityEngine;
using UnityEngine.InputSystem;

namespace Entities.Hero
{
    [RequireComponent(typeof(Movement))]
    [RequireComponent(typeof(PlayerInput))]
    public class KeyboardInput : MonoBehaviour
    {
        private Movement _movement;

        private PlayerInput _input;
        private InputAction _moveAction;
        private InputAction _dashAction;

        private void Awake()
        {
            _movement = GetComponent<Movement>();
            _input = GetComponent<PlayerInput>();
            _moveAction = _input.actions.FindAction("Move");
            _dashAction = _input.actions.FindAction("Dash");
        }
        
        private void OnEnable()
        {
            _dashAction.started += Dash;
        }

        private void OnDisable()
        {
            _dashAction.started -= Dash;
        }

        private void Update()
        {
            ReadMoveDirection();
        }

        private void ReadMoveDirection()
        {
            var moveDirection = _moveAction.ReadValue<Vector2>();
            _movement.Move(moveDirection);
        }

        private void Dash(InputAction.CallbackContext context)
        {
            _movement.Dash();
        }
    }
}