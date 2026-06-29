using System;
using Core.Mono;
using Core.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.UI.Battle
{
    /// <summary>
    /// 战斗消息UI
    /// </summary>
    public class BattleMessageUI : UIBehaviourBase
    {
        [InjectUI] private TextMeshProUGUI txtMsg;
        [InjectUI] private Image msg;
        [InjectUI] private Image imgIcon;
        
        private IMonoAdapter _monoAdapter;
        private float msgAlpha;
        private float imgIconAlpha;

        private float duration = 3;
        private float currentDuration;

        public event Action<BattleMessageUI> OnDurationOver;

        protected override void Awake()
        {
            base.Awake();

            msgAlpha = msg.color.a;
            imgIconAlpha = imgIcon.color.a;
        }

        protected override void OnEnable()
        {
            currentDuration = 0;
        }

        public void InitMessage(Color color, string msg, IMonoAdapter monoAdapter)
        {
            this.msg.color = new Color(color.r, color.g, color.b, msgAlpha);
            imgIcon.color = new Color(color.r, color.g, color.b, imgIconAlpha);
            txtMsg.text = msg;
            monoAdapter.AddUpdateListener(OnUpdate);
            _monoAdapter = monoAdapter;
        }

        private void OnUpdate()
        {
            currentDuration += Time.deltaTime;
            if (currentDuration >= duration)
            {
                OnDurationOver?.Invoke(this);
            }
        }

        protected override void OnDisable()
        {
            _monoAdapter.RemoveUpdateListener(OnUpdate);
        }
    }
}
