using System;
using Core.Components;
using Core.Mono;
using Core.Service;
using Core.Singleton;
using GameHotUpdate.Dialogue;
using GameHotUpdate.Input;
using UnityEngine;

namespace GameHotUpdate.Camera
{
    /// <summary>
    /// ����ʽ�����˳������
    /// </summary>
    public class OrbitCameraController : SingletonMono<OrbitCameraController>, IOrbitCameraController
    {
        public Transform Transform { get; private set; }

        [Header("��������")]
        // ����ҵĹ̶��뾶�����룩
        public float radius = 4f;
        // ������ҵ�ƫ�ƣ���ͷ����
        public Vector3 lookOffset = new Vector3(0, 1.5f, 0);

        [Header("������������")]
        // ���������
        public float mouseSensitivity = 0.2f;
        // ��ֱ�ӽ���С�Ƕȣ������ӽǷ�ת��
        public float minVerticalAngle = 50f;
        // ��ֱ�ӽ����Ƕȣ����������͸���棩
        public float maxVerticalAngle = 90f;
        // �Ƿ�ƽ����ת
        public bool smoothRotate = false;
        // ƽ���ٶ�
        public float smoothSpeed = 15f;

        // ���Ŀ�꣨�Ƹö�����ת��
        private Transform player;
        // ˮƽ��ת�Ƕȣ���Y�ᣩ
        private float _horizontalAngle;
        // ��ֱ��ת�Ƕȣ���X�ᣩ
        private float _verticalAngle = 30f;
        // �����Ŀ��λ��
        private Vector3 _targetCameraPos;
        // �������
        private Vector2 mouseInput;
        
        private GameObject _gameObject;
        private EntityProperty _entityProperty;

        public IEntityObject EntityObject { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            ServiceLocator.Get<IMonoAdapter>().AddUpdateListener(OnUpdate);
            ServiceLocator.Get<IDialogueManager>().OnDialogueStart += OnDialogueStart;
            ServiceLocator.Get<IDialogueManager>().OnDialogueEnd += OnDialogueEnd;

            Transform = transform;
            Init();
        }

        /// <summary>
        /// ��ʼ��
        /// </summary>
        private void Init()
        {
            // ��ʼ����������굽��Ļ����
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // ��ʼ�Ƕȣ���ѡ����ȡ��ǰ������Ƕȣ�
            if (player)
            {
                var dir = transform.position - player.position;
                _horizontalAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                _verticalAngle = Mathf.Asin(dir.y / radius) * Mathf.Rad2Deg;
            }
        }

        private void OnDialogueStart()
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }

        private void OnDialogueEnd()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        /// <summary>
        /// ����Ŀ��
        /// </summary>
        /// <param name="target"></param>
        public void SetTarget(Transform target)
        {
            player = target;
            target.GetComponent<IEntityObject>().GetComponent<InputComponent>().OnMouseSlideChanged += OnUpdateMouse;
        }

        /// <summary>
        /// ֡����
        /// </summary>
        private void OnUpdate()
        {
            // ������룺�����������ʱ��Ӧ����Esc/Atl�ɽ�������ѡ��
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                // ������
                OnMouseWheel();
                // Ӧ���������
                ApplyMouseInput();
            }
        }

        private void LateUpdate()
        {
            // ȷ����Ҳ�Ϊ��
            if (!player)
            {
                return;
            }

            // ���������Ŀ��λ�ã���������ת�������꣩
            CalculateTargetPosition();

            // ƽ���ƶ��������Ŀ��λ��
            if (smoothRotate)
            {
                transform.position = Vector3.Lerp(transform.position, _targetCameraPos, Time.deltaTime * smoothSpeed);
            }
            else
            {
                transform.position = _targetCameraPos;
            }

            // ǿ�������������ң���ƫ�ƣ�
            transform.LookAt(player.position + lookOffset);
        }

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="mouseInput"></param>
        private void OnUpdateMouse(Vector2 mouseInput)
        {
            this.mouseInput = mouseInput;
        }

        /// <summary>
        /// Ӧ��������룬������ת�Ƕ�
        /// </summary>
        private void ApplyMouseInput()
        {
            // ��ȡ�������ƶ���
            float mouseX = mouseInput.x * mouseSensitivity;
            float mouseY = mouseInput.y * mouseSensitivity;

            // ˮƽ�Ƕȣ���Y����ת�������ƶ���꣩
            _horizontalAngle += mouseX;
            // ��ֱ�Ƕȣ���X����ת�������ƶ���꣩�������Ʒ�Χ
            _verticalAngle -= mouseY;
            _verticalAngle = Mathf.Clamp(_verticalAngle, minVerticalAngle, maxVerticalAngle);
        }

        /// <summary>
        /// ���������Ŀ��λ�ã����ģ���������ת�ѿ������꣩
        /// </summary>
        private void CalculateTargetPosition()
        {
            // ���Ƕ�תΪ����
            float horizontalRad = _horizontalAngle * Mathf.Deg2Rad;
            float verticalRad = _verticalAngle * Mathf.Deg2Rad;

            // �������깫ʽ��
            // x = Բ��x + �뾶 * sin(��ֱ�Ƕ�) * sin(ˮƽ�Ƕ�)
            // y = Բ��y + �뾶 * cos(��ֱ�Ƕ�)
            // z = Բ��z + �뾶 * sin(��ֱ�Ƕ�) * cos(ˮƽ�Ƕ�)
            float x = player.position.x + radius * Mathf.Sin(verticalRad) * Mathf.Sin(horizontalRad);
            float y = player.position.y + radius * Mathf.Cos(verticalRad);
            float z = player.position.z + radius * Mathf.Sin(verticalRad) * Mathf.Cos(horizontalRad);

            _targetCameraPos = new Vector3(x, y, z);
        }

        /// <summary>
        /// ��̬�����뾶���������ţ�
        /// </summary>
        private void OnMouseWheel()
        {
            float scroll = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
            radius = Mathf.Clamp(radius - scroll * 2f, 2f, 10f); // ���ư뾶��Χ2-10��
        }

        private void OnDestroy()
        {
            ServiceLocator.Get<IMonoAdapter>().RemoveUpdateListener(OnUpdate);
        }
        
        #region 无用
        GameObject IEntityObject.GameObject => _gameObject;

        EntityProperty IEntityObject.EntityProperty => _entityProperty;

        void IEntityObject.BaseInit(int id)
        {
            throw new NotImplementedException();
        }

        T IEntityObject.GetComponent<T>()
        {
            throw new NotImplementedException();
        }

        TComponent IEntityObject.GetComponentInChildren<TComponent>()
        {
            throw new NotImplementedException();
        }

        TComponent IEntityObject.AddComponent<TComponent>()
        {
            throw new NotImplementedException();
        }

        bool IEntityObject.AddComponents(params string[] componentNames)
        {
            throw new NotImplementedException();
        }

        void IEntityObject.Destroy()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
