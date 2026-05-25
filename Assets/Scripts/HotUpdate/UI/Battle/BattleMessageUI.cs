using Core.DI;
using Core.Mono;
using Core.Pool;
using Core.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.Game.Battle.UI
{
    /// <summary>
    /// ս����ϢUI
    /// </summary>
    public class BattleMessageUI : UIBehaviourBase
    {
        [InjectUI] private TextMeshProUGUI txtMsg;
        [InjectUI] private Image msg;
        [InjectUI] private Image imgIcon;

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
            DIContainer.GetInstance<IMonoAdapter>().AddUpdateListener(OnUpdate);
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
                DIContainer.GetInstance<IPoolManager>().PushObj(gameObject);
            }
        }

        protected override void OnDisable()
        {
            DIContainer.GetInstance<IMonoAdapter>().RemoveUpdateListener(OnUpdate);
        }
    }
}
