using Core.DI;
using Core.Mono;
using Core.Pool;
using Core.UI;
using HotUpdate.Game.Battle.Statuses;
using UnityEngine;

namespace HotUpdate.UI.Battle.Status
{
    public class StatusGridLogic : IUILogic<StatusGridUI, StatusGridLogic>, IPoolData
    {
        [Inject] private IPoolManager _poolManager;
        [Inject] private IMonoAdapter _monoAdapter;
        
        private IStatus status;
        private int currentPine;

        public bool IsValid => status.IsValid;

        public StatusGridUI View { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="statusGridUI"></param>
        /// <param name="status"></param>
        public void Init(StatusGridUI statusGridUI, IStatus status)
        {
            View = statusGridUI;
            this.status = status;
            currentPine = status.StatusProperty.CurrentPine;

            View.txtPine.text = status.StatusProperty.CurrentPine.ToString();
            ChangedBuffOrDeBuff();
            _monoAdapter.AddUpdateListener(OnUpdate);
        }

        public int GetStatusId()
        {
            return status.StatusProperty.StatusInfo.f_id;
        }
        
        private void ChangedBuffOrDeBuff()
        {
            if ((EStatusType)status.StatusProperty.StatusInfo.f_statusType == EStatusType.Positive)
            {
                View.imgBuffOrDeBuff.color = Color.blue;
            }
            else
            {
                View.imgBuffOrDeBuff.color = Color.red;
                View.imgBuffOrDeBuff.transform.rotation = Quaternion.Euler(0, 0, 180);
            }
        }
        
        private void OnUpdate()
        {
            if (currentPine == status.StatusProperty.CurrentPine)
            {
                return;
            }

            View.txtPine.text = status.StatusProperty.CurrentPine.ToString();
            currentPine = status.StatusProperty.CurrentPine;
        }
        
        public void ResetData()
        {
            _monoAdapter.RemoveUpdateListener(OnUpdate);
        }
        
        public void Dispose()
        {
            _poolManager.PushData(this);
        }
    }
}
