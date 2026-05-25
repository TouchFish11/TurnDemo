using Core.DI;
using Core.Mono;
using Core.UI;
using HotUpdate.Game.Battle.Status;
using HotUpdate.Game.Battle.Status.Enum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.Game.Battle.UI.Status
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
            DIContainer.GetInstance<IMonoAdapter>().AddUpdateListener(OnUpdate);
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
            DIContainer.GetInstance<IMonoAdapter>().RemoveUpdateListener(OnUpdate);
        }

        public int GetStatusId() => status.StatusProperty.StatusInfo.f_id;

        public bool IsValid => status.IsValid;
    }
}
