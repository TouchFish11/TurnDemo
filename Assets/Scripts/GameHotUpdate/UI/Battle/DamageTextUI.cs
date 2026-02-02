using Core.Mono;
using Core.Pool;
using Core.Service;
using Core.UI;
using TMPro;
using UnityEngine;

namespace GameHotUpdate.UI.Battle
{
    /// <summary>
    /// �˺��ı�UI
    /// </summary>
    public class DamageTextUI : BaseUIBehaviour
    {
        [Inject] private TextMeshProUGUI txtDamageTip;
        [Inject] private TextMeshProUGUI txtDamageNum;

        [Inject(1)] private RectTransform DamageTextMover { get; set; }

        // �����ٶ�
        private float upMoveSpeed = 2.5f;
        // ����ʱ��
        private float destroyTime = 0.85f;
        // ��ʼ����
        private Vector3 StartScale = Vector3.one * 1.7f;
        // ��������
        private Vector3 endScale = Vector3.one;
        // ��������
        private float scaleFactor = 9f;

        // ��ǰʱ��
        private float currentTime;
        // ԭʼ��ɫ
        private Color originColor;
        // ԭʼ͸����
        private float originAlpha;
        // ��ǰ͸����
        private float currentAlpha;
        
        protected override void OnEnable()
        {
            ServiceLocator.Get<IMonoAdapter>().AddUpdateListener(OnUpdate);
            (DamageTextMover.transform as RectTransform).anchoredPosition = Vector3.zero;
            DamageTextMover.localScale = StartScale;
            currentTime = 0;
            currentAlpha = originAlpha;
            txtDamageTip.color = originColor;
            txtDamageNum.color = originColor;
        }

        /// <summary>
        /// ��ʼ���˺��ı�
        /// </summary>
        /// <param name="textColor"></param>
        /// <param name="damageTypeText"></param>
        /// <param name="damage"></param>
        public void InitDamageText(Color textColor, string damageTypeText, int damage)
        {
            txtDamageTip.color = textColor;
            txtDamageNum.color = textColor;

            txtDamageTip.text = damageTypeText;
            txtDamageNum.text = damage.ToString();

            originColor = txtDamageTip.color;
            originAlpha = currentAlpha = txtDamageTip.color.a;
        }

        private void OnUpdate()
        {
            currentTime += Time.deltaTime;
            if (currentTime >= destroyTime)
            {
                currentTime = 0;
                ServiceLocator.Get<IPoolManager>().PushObj(gameObject);
            }

            // �ı�����
            DamageTextMover.localScale = Vector3.Lerp(DamageTextMover.localScale, endScale, Time.deltaTime * scaleFactor);
            //�ı��˶�
            DamageTextMover.Translate(Time.deltaTime * upMoveSpeed * Vector3.up);
        }

        protected override void OnDisable()
        {
            ServiceLocator.Get<IMonoAdapter>().RemoveUpdateListener(OnUpdate);
        }
    }
}
