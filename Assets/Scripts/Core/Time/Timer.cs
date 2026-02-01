using Core.Pool;
using UnityEngine.Events;

namespace Core.Time
{
    /// <summary>
    /// ��ʱ��
    /// </summary>
    public class Timer : IPoolData
    {
        //��ʱ��ΨһID
        private int _id;
        //ʣ��ʱ��(ms)
        private int _nowTime;
        //���ʱ��(ms)
        private int _maxTime;
        //���ʣ��ʱ��(ms)
        private int _nowIntervalTime;
        //�����ʱ��(ms)
        private int _maxIntervalTime;
        //��ʱ������ص�
        private  UnityAction _allTimeOverCallBack;
        //���ʱ������ص�
        private  UnityAction _intervalTimeOverCallBack;
        //�Ƿ����ڼ�ʱ
        private bool _isRunning;

        /// <summary>
        /// ��ʼ����ʱ��
        /// </summary>
        /// <param name="id">ΨһID</param>
        /// <param name="maxTime">���ʱ��</param>
        /// <param name="timeOverCallBack">ʱ������ص�</param>
        /// <param name="maxIntervalTime">��ѡ�����ʱ��</param>
        /// <param name="intervalTimeOverCallBack">��ѡ�����ʱ��ص�</param>
        public void InitTimer(int id, int maxTime, UnityAction timeOverCallBack, int maxIntervalTime = 0, UnityAction intervalTimeOverCallBack = null)
        {
            _id = id;
            _maxTime = _nowTime = maxTime;
            _maxIntervalTime = _nowIntervalTime = maxIntervalTime;
            _allTimeOverCallBack = timeOverCallBack;
            _intervalTimeOverCallBack = intervalTimeOverCallBack;
            _isRunning = true;
        }

        /// <summary>
        /// ���ü�ʱ��
        /// </summary>
        public void ResetTimer()
        {
            _nowTime = _maxTime;
            _nowIntervalTime = _maxIntervalTime;
            _isRunning = true;
        }

        /// <summary>
        /// ��ʱ�������ص�
        /// </summary>
        public void OverInvoke()
        {
            _allTimeOverCallBack?.Invoke();
        }

        /// <summary>
        /// ��ʱ�����ʱ��ص�
        /// </summary>
        public void IntervalInvoke()
        {
            _intervalTimeOverCallBack?.Invoke();
            _nowIntervalTime = _maxIntervalTime;
        }

        public void ResetData()
        {
            _id = -1;
            _nowTime = _maxTime = 0;
            _nowIntervalTime = _maxIntervalTime = 0;
            IsRunning = false;
            //���ί��
            _allTimeOverCallBack = null;
            _intervalTimeOverCallBack = null;
        }

        /// <summary>
        /// �Ƿ����ڼ�ʱ
        /// </summary>
        public bool IsRunning { get => _isRunning; set => _isRunning = value; }

        /// <summary>
        /// ʣ�����ʱ��
        /// </summary>
        public int NowTime { get => _nowTime; set => _nowTime = value; }

        /// <summary>
        /// ʣ��ļ��ʱ��
        /// </summary>
        public int NowIntervalTime { get => _nowIntervalTime; set => _nowIntervalTime = value; }

        /// <summary>
        /// ��ʱ��ΨһID
        /// </summary>
        public int Id { get => _id; }
    }
}
