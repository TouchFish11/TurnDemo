using Core.DI;
using Core.Mono;
using Core.UI;
using HotUpdate.Game.Battle.Statuses;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.UI.Battle.Status
{
    /// <summary>
    /// 角色状态栏的状态格子对象
    /// </summary>
    public class StatusGridUI : UIBehaviourBase
    {
        [InjectUI] private Image imgIcon;
        [InjectUI] private Image imgBuffOrDeBuff;
        [InjectUI] private TextMeshProUGUI txtPine;

        private IMonoAdapter _monoAdapter;
        private IStatus status;
        private int currentPine;

        protected override void OnEnable()
        {
            _monoAdapter?.AddUpdateListener(OnUpdate);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="status"></param>
        /// <param name="monoAdapter"></param>
        public void Init(IStatus status, IMonoAdapter monoAdapter)
        {
            this.status = status;
            currentPine = status.StatusProperty.CurrentPine;

            txtPine.text = status.StatusProperty.CurrentPine.ToString();
            ChangedBuffOrDeBuff();

            _monoAdapter = monoAdapter;
        }

        private void ChangedBuffOrDeBuff()
        {
            if ((EStatusType)status.StatusProperty.StatusInfo.f_statusType == EStatusType.Positive)
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
            _monoAdapter.RemoveUpdateListener(OnUpdate);
        }

        public int GetStatusId() => status.StatusProperty.StatusInfo.f_id;

        public bool IsValid => status.IsValid;
    }
}
