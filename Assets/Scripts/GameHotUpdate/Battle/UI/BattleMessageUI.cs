using Core.Mono;
using Core.Pool;
using Core.Service;
using Core.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameHotUpdate.Battle.UI
{
    /// <summary>
    /// ս����ϢUI
    /// </summary>
    public class BattleMessageUI : BaseUIBehaviour
    {
        [Inject] private TextMeshProUGUI txtMsg;
        [Inject] private Image msg;
        [Inject] private Image imgIcon;

        // ͸����
        private float msgAlpha;
        private float imgIconAlpha;

        private float duration = 3;
        // ��ǰ����ʱ��
        private float currentDuration;

        protected override void Awake()
        {
            base.Awake();

            msgAlpha = msg.color.a;
            imgIconAlpha = imgIcon.color.a;
        }

        protected override void OnEnable()
        {
            currentDuration = 0;
            ServiceLocator.Get<IMonoAdapter>().AddUpdateListener(OnUpdate);
        }

        public void InitMessage(Color color, string msg)
        {
            this.msg.color = new Color(color.r, color.g, color.b, msgAlpha);
            imgIcon.color = new Color(color.r, color.g, color.b, imgIconAlpha);
            txtMsg.text = msg;
        }

        private void OnUpdate()
        {
            currentDuration += Time.deltaTime;
            if (currentDuration >= duration)
            {
                ServiceLocator.Get<IPoolManager>().PushObj(gameObject);
            }
        }

        protected override void OnDisable()
        {
            ServiceLocator.Get<IMonoAdapter>().RemoveUpdateListener(OnUpdate);
        }
    }
}
