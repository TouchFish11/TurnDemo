using Game.Test.Animation;
using Game.Test.Inputs;
using UnityEngine;

namespace Game.Test.Controller
{
    /// <summary>
    /// 第一人称控制器
    /// </summary>
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField] private Camera fpsCamera;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 130f;
        [SerializeField] private float dashSpeed = 10f;
        [SerializeField] private float jumpSpeed = 2f;
        [SerializeField] private float jumpHeight = 1.3f;
        [SerializeField] private float gravityValue = -9.81f;
        [SerializeField] private float minYAngle = -60f;
        [SerializeField] private float maxYAngle = 60f;
        
        private Vector3 _moveDirection;
        private float _currentXAngle;
        private float _currentYAngle;
        private float _currentX;
        private float _currentY;
        private Vector3 playerVelocity;
        private bool groundedPlayer;
        private Quaternion cameraOriginRotation;
        
        private CharacterController _characterController;
        private AnimatorController _animatorController;
        private UpperBodyLookAtIK _lookAtIK;
        
        private void Awake()
        {
            _characterController = this.GetComponent<CharacterController>();
            _animatorController = this.GetComponentInChildren<AnimatorController>();
            _lookAtIK = this.GetComponentInChildren<UpperBodyLookAtIK>();
            
            cameraOriginRotation = Quaternion.Euler(3, 0, 0);
            fpsCamera.transform.localRotation = cameraOriginRotation;
            
            // test
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Start()
        {
            InputManager.Instance.OnMove += OnMove;
            InputManager.Instance.OnJump += OnJump;
            InputManager.Instance.OnMouseX += OnMouseX;
            InputManager.Instance.OnMouseY += OnMouseY;
        }

        private void OnMouseY(float y)
        {
            _currentY = y;
        }

        private void OnMouseX(float x)
        {
            _currentX = x;
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
                _characterController.Move(this.transform.forward * (dashSpeed * Time.deltaTime)); 
            }
            else if(CanMove())
            {
                Move();
            }
            
            // 旋转
            _currentXAngle += _currentX * rotationSpeed * Time.deltaTime;   // rotationSpeed应该是灵敏度
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                Quaternion.Euler(0, _currentXAngle, 0), rotationSpeed * Time.deltaTime);
            
            var currentRealY = -_currentY * rotationSpeed * Time.deltaTime;
            _currentYAngle += currentRealY;
            _currentYAngle = Mathf.Clamp(_currentYAngle, 
                 minYAngle + cameraOriginRotation.eulerAngles.x, 
                maxYAngle + cameraOriginRotation.eulerAngles.x);
            // 移动IK
            _lookAtIK.MoveTarget(currentRealY);
            // 旋转相机
            fpsCamera.transform.localRotation = Quaternion.Slerp(fpsCamera.transform.localRotation,
                Quaternion.Euler(_currentYAngle, 0, 0), rotationSpeed * Time.deltaTime);

            // 跳跃
            playerVelocity.y += gravityValue * Time.deltaTime;
            _characterController.Move(playerVelocity * Time.deltaTime);
        }

        private void Move()
        {
            // 面朝向移动
            if (_moveDirection.z > 0 &&  _moveDirection.x == 0)
            {
                _characterController.Move(this.transform.forward * (Time.deltaTime * moveSpeed)); 
            }
            // 后退
            else if (_moveDirection.z < 0 && _moveDirection.x == 0)
            {
                _characterController.Move(-this.transform.forward * (Time.deltaTime * moveSpeed));
            }
            // 左
            else if (_moveDirection.x < 0 && _moveDirection.z == 0)
            {
                _characterController.Move(-this.transform.right * (Time.deltaTime * moveSpeed));
            }
            // 右
            else if (_moveDirection.x > 0 && _moveDirection.z == 0)
            {
                _characterController.Move(this.transform.right * (Time.deltaTime * moveSpeed));
            }
            // 右上
            else if (_moveDirection.z > 0 && _moveDirection.x > 0)
            {
                var rightTop = this.transform.right + this.transform.forward;
                _characterController.Move(rightTop.normalized * (Time.deltaTime * moveSpeed));
            }
            // 左上
            else if (_moveDirection.z > 0 && _moveDirection.x < 0)
            {
                var leftTop = -this.transform.right + this.transform.forward;
                _characterController.Move(leftTop.normalized * (Time.deltaTime * moveSpeed));
            }
            // 右下
            else if (_moveDirection.x > 0 && _moveDirection.z < 0)
            {
                var rightBot = this.transform.right  + -this.transform.forward;
                _characterController.Move(rightBot.normalized * (Time.deltaTime * moveSpeed));
            }
            // 左下
            else if (_moveDirection.x < 0 && _moveDirection.z < 0)
            {
                var leftBot = -this.transform.right + -this.transform.forward;
                _characterController.Move(leftBot.normalized * (Time.deltaTime * moveSpeed));
            }
        }
        
        private bool CanMove()
        {
            return !_animatorController.IsDashing;
        }
    }
}
