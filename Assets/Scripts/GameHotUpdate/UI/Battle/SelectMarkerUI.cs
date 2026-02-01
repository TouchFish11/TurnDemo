using System.Collections.Generic;
using Core.Mono;
using Core.Service;
using Core.UI;
using Core.Utility;
using Game.Battle;
using Game.Battle.Objects;
using Game.Battle.Skill.Enum;
using UnityEngine;
using UnityEngine.UI;

namespace GameHotUpdate.UI.Battle
{
    /// <summary>
    /// ѡ����UI
    /// </summary>
    public class SelectMarkerUI : BaseUIBehaviour
    {
        //���ƫ��
        private Vector2 markerOffset = new Vector2(0, 100);
        //�����ת�ٶ�
        private float markerRotationSpeed = 50f;
        //��������ٶ�
        private float markerPulseSpeed = 1.2f;
        //�����������
        private float markerPulseScale = 1.2f;

        // ͼ��UI�б�
        private readonly List<Image> images = new List<Image>();
        // ��ʼ��ת
        private Quaternion originQuaterion;
        // ��ʼ����
        private Vector3 originScale;
        // �����ɫ���
        private Color enermyRed = Color.red;
        private Color friendBlue = Color.blue;

        // ���Ŀ��
        private IBattleEntityObject battleEntity;
        // ��Ǹ�����
        private Transform selectMarkerArea;

        protected override void Awake()
        {
            base.Awake();
            
            for (int i = 0; i < 5; i++)
            {
                images.Add(binder.GetControl<Image>($"m{i + 1}"));
            }

            originQuaterion = transform.rotation;
            originScale = transform.localScale;
        }

        protected override void OnEnable()
        {
            transform.rotation = originQuaterion;
            transform.localScale = originScale;
            MonoManager.Instance.AddUpdateListener(OnUpdate);
        }

        /// <summary>
        /// ��ʼ��ѡ����
        /// </summary>
        /// <param name="battleEntity"></param>
        /// <param name="skillTargetType"></param>
        /// <param name="selectMarkerArea"></param>
        public void InitSelectMarker(IBattleEntityObject battleEntity, E_SkillTargetType skillTargetType, Transform selectMarkerArea)
        {
            this.battleEntity = battleEntity;
            this.selectMarkerArea = selectMarkerArea;
            Color color = skillTargetType == E_SkillTargetType.Enemy ? enermyRed : friendBlue;
            foreach (Image image in images)
            {
                image.color = color;
            }
        }

        private void OnUpdate()
        {
            FollowTarget();
            //���±�Ƕ���
            UpdateMarkerAnimation();
        }

        /// <summary>
        /// ����Ŀ��
        /// </summary>
        private void FollowTarget()
        {
            if (battleEntity == null)
            {
                return;
            }

            UIUtility.WorldToLocalPointInRectangle(ServiceLocator.Get<IBattlePoint>().CurrentActiveCamera, ServiceLocator.Get<IUIManager>().UICamera, selectMarkerArea, gameObject, battleEntity.GameObject.transform.position, Vector2.up * 50);
        }

        /// <summary>
        /// ���±�Ƕ���
        /// </summary>
        private void UpdateMarkerAnimation()
        {
            // ��ת����
            transform.Rotate(Vector3.forward, markerRotationSpeed * Time.deltaTime);
            // �������Ŷ���
            float scale = 1f + Mathf.Sin(Time.time * markerPulseSpeed) * (markerPulseScale - 1f) * 0.5f;
            transform.localScale = Vector3.one * scale;
        }

        protected override void OnDisable()
        {
            ServiceLocator.Get<IMonoManager>().RemoveUpdateListener(OnUpdate);
        }
    }
}
