using Core.Mono;
using Core.Pool;
using Core.Service;
using Core.UI;
using TMPro;
using UnityEngine;

namespace GameHotUpdate.UI.Global
{
    /// <summary>
    /// ��ϢUI
    /// </summary>
    public class MessageUI : BaseUIBehaviour
    {
        [Inject] private TextMeshProUGUI txtMsg;

        // ����ʱ��
        [SerializeField] private float duration;

        // ��ǰʱ��
        private float currentDuration;

        protected override void OnEnable()
        {
            currentDuration = 0;
            ServiceLocator.Get<IMonoManager>().AddUpdateListener(OnUpdate);
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
                PoolManager.Instance.PushObj(gameObject);
            }
        }

        protected override void OnDisable()
        {
            ServiceLocator.Get<IMonoManager>().RemoveUpdateListener(OnUpdate);
        }
    }
}
