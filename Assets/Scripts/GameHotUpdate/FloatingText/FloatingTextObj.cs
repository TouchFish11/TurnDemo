using Core.Mono;
using Core.Service;
using TMPro;
using UnityEngine;

namespace GameHotUpdate.FloatingText
{
    /// <summary>
    /// �����ı�
    /// </summary>
    public class FloatingTextObj : MonoBehaviour
    {
        private TextMeshPro txtName;
        private TextMeshPro txtTip;

        // �����NPCTransform
        private Transform followNpcTarget;
        // ͷ��ƫ����
         private readonly Vector3 offset = new(0, 2, 0);
        // ��С����
         private readonly Vector3 minScale = Vector3.one * 0.2f;
        // �������
        private readonly Vector3 maxScale = Vector3.one * 1.35f;
        // �����ٶ�
        private const float scaleSpeed = 1.1f;
        // �����
        private Camera mainCamera;
        // �����
        private Transform mainPlayer;
        // �ϴξ���
        private float lastDis;

        private void Awake()
        {
            txtName = this.transform.Find($"{nameof(txtName)}")?.GetComponent<TextMeshPro>();
            txtTip = this.transform.Find($"{nameof(txtTip)}")?.GetComponent<TextMeshPro>();
            
            mainCamera = Camera.main;
        }

        private void OnEnable()
        {
            ServiceLocator.Get<IMonoManager>().AddUpdateListener(OnUpdate);
        }

        /// <summary>
        /// ��ʼ��
        /// </summary>
        /// <param name="player"></param>
        /// <param name="name"></param>
        /// <param name="tip"></param>
        /// <param name="npcTarget"></param>
        public void Init(Transform npcTarget, Transform player, string name, string tip)
        {
            followNpcTarget = npcTarget;
            mainPlayer = player;
            txtName.text = name;
            txtTip.text = tip;
        }

        private void OnUpdate()
        {
            if (followNpcTarget == null || mainCamera == null)
            {
                return;
            }

            // �������
            transform.forward = mainCamera.transform.forward;
            // ����Ŀ��
            transform.position = followNpcTarget.position + offset;
            // ��Ŀ��Խ�����ı�ԽС����֮Խ��
            UpdateScale();
        }

        private void UpdateScale()
        {
            float currentDis = Vector3.Distance(transform.position, mainPlayer.position);
            if (currentDis < lastDis)
            {
                // ��С
                transform.localScale = Vector3.Lerp(transform.localScale, minScale, Time.deltaTime * scaleSpeed);
                lastDis = currentDis;
            }
            else if(currentDis > lastDis)
            {
                // �Ŵ�
                transform.localScale = Vector3.Lerp(transform.localScale, maxScale, Time.deltaTime * scaleSpeed);
                lastDis = currentDis;
            }
        }
        

        private void OnDisable()
        {
            ServiceLocator.Get<IMonoManager>().RemoveUpdateListener(OnUpdate);
        }
    }
}
