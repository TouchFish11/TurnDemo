using Core.Mono;
using Core.Service;
using Core.UI;
using HotUpdate.Battle.Status;
using HotUpdate.Battle.Status.Enum;
using HotUpdate.Core.Battle.Status;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.Battle.UI.Status
{
    /// <summary>
    /// ״̬����UI
    /// </summary>
    public class StatusGridUI : UIBehaviourBase
    {
        [Inject] private Image imgIcon;
        [Inject] private Image imgBuffOrDeBuff;
        [Inject] private TextMeshProUGUI txtPine;

        private IStatus status;
        private int currentPine;

        protected override void OnEnable()
        {
            ServiceLocator.Get<IMonoAdapter>().AddUpdateListener(OnUpdate);
        }

        /// <summary>
        /// ��ʼ��
        /// </summary>
        /// <param name="status"></param>
        public void Init(IStatus status)
        {
            this.status = status;
            currentPine = status.StatusProperty.CurrentPine;

            txtPine.text = status.StatusProperty.CurrentPine.ToString();
            ChangedBuffOrDeBuff();
        }

        private void ChangedBuffOrDeBuff()
        {
            if ((E_StatusType)status.StatusProperty.StatusInfo.f_statusType == E_StatusType.Positive)
            {
                imgBuffOrDeBuff.color = Color.blue;
            }
            else
            {
                imgBuffOrDeBuff.color = Color.red;
                imgBuffOrDeBuff.transform.rotation = Quaternion.Euler(0, 0, 180);
            }
        }

        private void OnUpdate()
        {
            if (currentPine == status.StatusProperty.CurrentPine)
            {
                return;
            }

            txtPine.text = status.StatusProperty.CurrentPine.ToString();
            currentPine = status.StatusProperty.CurrentPine;
        }

        protected override void OnDisable()
        {
            ServiceLocator.Get<IMonoAdapter>().RemoveUpdateListener(OnUpdate);
        }

        public int GetStatusId() => status.StatusProperty.StatusInfo.f_id;

        public bool IsValid => status.IsValid;
    }
}
