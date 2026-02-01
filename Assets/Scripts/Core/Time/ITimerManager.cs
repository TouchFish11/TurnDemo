using UnityEngine.Events;

namespace Core.Time
{
    /// <summary>
    /// ʱ��������ӿ�
    /// </summary>
    public interface ITimerManager
    {
        void Close();
        void ContinueTimer(int id);
        int CreateTimer(bool isRealTime, int maxTime, UnityAction timeOverCallBack, int intervalTime = 0, UnityAction intervalTimeOverCallBack = null);
        Timer GetTimer(int id);
        void PauseTimer(int id);
        void RemoveTimer(int id);
        void ResetTimer(int id);
        void SetTimeRate(E_TimeRate timeRate);
    }
}
