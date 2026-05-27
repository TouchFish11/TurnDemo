using Game.Animation;
using Game.Inputs;
using UnityEngine;

namespace Game.Move
{
    public class Transformer : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private float dashSpeed = 10f;
        [SerializeField] private float jumpSpeed = 2f;
        [SerializeField] private float jumpHeight = 1.3f;
        [SerializeField] private float gravityValue = -9.81f;
        
        private Vector3 _moveDirection;
        private Vector3 playerVelocity;
        private bool groundedPlayer;
        private CharacterController _characterController;
        private AnimatorController _animatorController;
        
        private void Awake()
        {
            _characterController = this.GetComponent<CharacterController>();
            _animatorController = this.GetComponentInChildren<AnimatorController>();
        }

        private void Start()
        {
            InputManager.Instance.OnMove += OnMove;
            InputManager.Instance.OnJump += OnJump;
        }

        private void OnJump()
        {
            if (groundedPlayer)
            {
                playerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
            }
        }

        private void OnMove(Vector2 dir)
        {
            _moveDirection = new Vector3(dir.x, 0, dir.y).normalized;
        }

        private void Update()
        {
            groundedPlayer = _characterController.isGrounded;
            if (groundedPlayer && playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
            }
            
            if (_animatorController.IsDashing)
            {
                _characterController.Move(_moveDirection * (dashSpeed * Time.deltaTime)); 
            }
            else if(CanMove())
            {
                _characterController.Move(_moveDirection * (Time.deltaTime * moveSpeed));
            }
            
            // 旋转
            if (_moveDirection != Vector3.zero)
            {
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(_moveDirection), rotationSpeed * Time.deltaTime);
            }

            // 跳跃
            playerVelocity.y += gravityValue * Time.deltaTime;
            _characterController.Move(playerVelocity * Time.deltaTime);
        }

        private bool CanMove()
        {
            return !(_animatorController.IsAttacking || _animatorController.IsDashing);
        }
    }
}
