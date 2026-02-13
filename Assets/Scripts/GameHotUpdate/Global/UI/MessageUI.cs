using Core.Mono;
using Core.Pool;
using Core.Service;
using Core.UI;
using TMPro;
using UnityEngine;

namespace GameHotUpdate.Global.UI
{
    /// <summary>
    /// ��ϢUI
    /// </summary>
    public class MessageUI : UIBehaviourBase
    {
        [Inject] private TextMeshProUGUI txtMsg;

        // ����ʱ��
        [SerializeField] private float duration = 2.5f;

        // ��ǰʱ��
        private float currentDuration;

        protected override void OnEnable()
        {
            currentDuration = 0;
            ServiceLocator.Get<IMonoAdapter>().AddUpdateListener(OnUpdate);
        }

        /// <summary>
        /// ��ʼ����Ϣ
        /// </summary>
        /// <param name="msg"></param>
        public void InitMessage(string msg)
        {
            txtMsg.text = msg;
        }

        private void OnUpdate()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

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
