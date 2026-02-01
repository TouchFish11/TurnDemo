using UnityEngine.Events;

namespace Core.Time
{
    /// <summary>
    /// ʱ�������ӿ�
    /// </summary>
    public interface ITimeChecker
    {
        void AddListener(int key, UnityAction overCallBack);
        long CalcRemainTime(System.DateTime current, int key);
        void Clear();
        int CreateTargetTime(System.DateTime currentTime, int targetDay, int targetHour, int targetMin, int targetSec);
        DateTime GetDateTime(int key);
    }
}
