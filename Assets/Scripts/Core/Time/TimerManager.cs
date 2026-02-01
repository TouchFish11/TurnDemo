using System.Collections;
using System.Collections.Generic;
using Core.Mono;
using Core.Pool;
using Core.Singleton;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Time
{
    /// <summary>
    /// ��ʱ��������
    /// </summary>
    public class TimerManager : SingletonBase<TimerManager>, ITimerManager
    {
        // �洢��ʱ������Ӱ��ļ�ʱ�����ֵ�
        private readonly Dictionary<int, Timer> _timerDic = new Dictionary<int, Timer>();
        // �洢����ʱ������Ӱ��ļ�ʱ�����ֵ�
        private readonly Dictionary<int, Timer> _realTimerDic = new Dictionary<int, Timer>();
        // �洢��ʱ������Ӱ��ļ�ʱ���Ĵ��Ƴ��б�
        private readonly List<int> _delTimerIDList = new List<int>();
        // �洢����ʱ������Ӱ��ļ�ʱ���Ĵ��Ƴ��б�
        private readonly List<int> _realDelTimerIDList = new List<int>();
        // ��ʱ��ΨһID
        private static int _TimerKey;
        // ��ʱ������Ӱ��ļ�ʱ��Э��
        private Coroutine _coroutine;
        // ����ʱ������Ӱ��ļ�ʱ��Э��
        private Coroutine _realCoroutine;
        // ��ʱ���¼��
        private const float IntervalTime = 0.1f;
        // ��ʱ������Ӱ���Э�̷��ض���
        private readonly WaitForSeconds _WaitForSecondsTime = new WaitForSeconds(IntervalTime);
        // ����ʱ������Ӱ���Э�̷��ض���
        private readonly WaitForSecondsRealtime _WaitForSecondsRealTime = new WaitForSecondsRealtime(IntervalTime);
        // ��ǰ���õ�ʱ���ٶ�
        private E_TimeRate _timeRate;

        private TimerManager()
        {
            //��ʼ��ʱ���ٶ�
            _timeRate = E_TimeRate.Normal;
            Start();
        }

        /// <summary>
        /// ������ʱ��������
        /// </summary>
        private void Start()
        {
            //������ʱ������Ӱ��ļ�ʱ��
            _coroutine = MonoManager.Instance.StartCoroutine(StartTiming(false, _timerDic));
            //��������ʱ������Ӱ��ļ�ʱ��
            _realCoroutine = MonoManager.Instance.StartCoroutine(StartTiming(true, _realTimerDic));
        }

        /// <summary>
        /// �رռ�ʱ��������
        /// </summary>
        public void Close()
        {
            //�ر���ʱ������Ӱ��ļ�ʱ��
            MonoManager.Instance.StopCoroutine(_coroutine);
            //�رղ���ʱ������Ӱ��ļ�ʱ��
            MonoManager.Instance.StopCoroutine(_realCoroutine);
        }

        /// <summary>
        /// ��ʼ��ʱЭ��
        /// </summary>
        /// <param name="isRealTime">�Ƿ���ʱ������Ӱ��</param>
        /// <param name="timerDic">��ʱ���ֵ�</param>
        /// <returns></returns>
        private IEnumerator StartTiming(bool isRealTime, Dictionary<int, Timer> timerDic)
        {
            while (true)
            {
                foreach (Timer timer in timerDic.Values)
                {
                    //��ʶΪ���ڼ�ʱ�ļ�ʱ���������ʱ
                    if (!timer.IsRunning)
                        continue;

                    //���ʱ�����
                    timer.NowIntervalTime -= (int)(IntervalTime * 1000);
                    //ʣ����ʱ��С�ڵ���0��ִ�м��ʱ������ص�
                    if (timer.NowIntervalTime <= 0)
                        timer.IntervalInvoke();

                    //��ʱ�����
                    timer.NowTime -= (int)(IntervalTime * 1000);
                    //ʣ����ʱ��С�ڵ���0��ִ����ʱ������ص�
                    if (timer.NowTime <= 0)
                    {
                        timer.OverInvoke();
                        //��ʱ��ϣ�������Ƴ��б�
                        _delTimerIDList.Add(timer.Id);
                    }
                }

                if (isRealTime)
                {
                    //�Ƴ���ʱ�����ļ�ʱ��
                    for (int i = 0; i < _realDelTimerIDList.Count; i++)
                    {
                        //�ҵ��и�ID�ļ�ʱ��
                        if (timerDic.ContainsKey(_realDelTimerIDList[i]))
                        {
                            //���뻺���
                            PoolManager.Instance.PushData(timerDic[_realDelTimerIDList[i]]);
                            //���ֵ����Ƴ�
                            timerDic.Remove(_realDelTimerIDList[i]);
                        }
                    }
                    //��մ�ɾ���б�
                    _realDelTimerIDList.Clear();
                }
                else
                {
                    //�Ƴ���ʱ�����ļ�ʱ��
                    for (int i = 0; i < _delTimerIDList.Count; i++)
                    {
                        //�ҵ��и�ID�ļ�ʱ��
                        if (timerDic.ContainsKey(_delTimerIDList[i]))
                        {
                            //���뻺���
                            PoolManager.Instance.PushData(timerDic[_delTimerIDList[i]]);
                            //���ֵ����Ƴ�
                            timerDic.Remove(_delTimerIDList[i]);
                        }
                    }
                    //��մ�ɾ���б�
                    _delTimerIDList.Clear();
                }

                //100ms����һ��
                if (isRealTime)
                    yield return _WaitForSecondsRealTime;
                else
                    yield return _WaitForSecondsTime;
            }
        }

        /// <summary>
        /// ������ʱ��
        /// </summary>
        /// <param name="isRealTime">�Ƿ���ʱ������Ӱ��</param>
        /// <param name="maxTime">���ʱ��</param>
        /// <param name="timeOverCallBack">�����ص�</param>
        /// <param name="intervalTime">���ʱ��</param>
        /// <param name="intervalTimeOverCallBack">���ʱ��ص�</param>
        /// <returns>��ʱ��ΨһID</returns>
        public int CreateTimer(bool isRealTime, int maxTime, UnityAction timeOverCallBack, int intervalTime = 0, UnityAction intervalTimeOverCallBack = null)
        {
            //�ӻ�����л�ȡ��ʱ������
            Timer timer = PoolManager.Instance.GetData<Timer>();
            //��ʼ����ʱ��
            timer.InitTimer(++_TimerKey, maxTime, timeOverCallBack, intervalTime, intervalTimeOverCallBack);
            //�����Ƿ���ʱ������Ӱ����벻ͬ�ֵ���
            if(isRealTime)
                _realTimerDic.Add(_TimerKey, timer);
            else
                _timerDic.Add(_TimerKey, timer);
            //���ؼ�ʱ��ΨһID
            return _TimerKey;
        }

        /// <summary>
        /// ���ü�ʱ��
        /// </summary>
        /// <param name="id">��ʱ��ΨһID</param>
        public void ResetTimer(int id)
        {
            if(_timerDic.ContainsKey(id))
            {
                _timerDic[id].ResetTimer();
            }
            else if(_realTimerDic.ContainsKey(id))
            {
                _realTimerDic[id].ResetTimer();
            }
        }

        /// <summary>
        /// �Ƴ�ָ����ʱ��
        /// </summary>
        /// <param name="id">��ʱ��ΨһID</param>
        public void RemoveTimer(int id)
        {
            if (_timerDic.ContainsKey(id))
            {
                _delTimerIDList.Add(id);
            }
            else if (_realTimerDic.ContainsKey(id))
            {
                _realDelTimerIDList.Add(id);
            }
        }

        /// <summary>
        /// ������ʱ
        /// </summary>
        /// <param name="id">��ʱ��ΨһID</param>
        public void ContinueTimer(int id)
        {
            if (_timerDic.ContainsKey(id))
                _timerDic[id].IsRunning = true;
            else if (_realTimerDic.ContainsKey(id))
                _realTimerDic[id].IsRunning = true;
        }

        /// <summary>
        /// ��ͣ��ʱ
        /// </summary>
        /// <param name="id">��ʱ��ΨһID</param>
        public void PauseTimer(int id)
        {
            if (_timerDic.ContainsKey(id))
                _timerDic[id].IsRunning = false;
            else if (_realTimerDic.ContainsKey(id))
                _realTimerDic[id].IsRunning = false;
        }

        /// <summary>
        /// ��ȡָ����ʱ��
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Timer GetTimer(int id)
        {
            if (_timerDic.ContainsKey(id))
                return _timerDic[id];
            else if (_realTimerDic.ContainsKey(id))
                return _realTimerDic[id];
            return null;
        }

        /// <summary>
        /// ����ʱ������
        /// </summary>
        /// <param name="timeRate"></param>
        public void SetTimeRate(E_TimeRate timeRate)
        {
            //�����ڻָ���0ʱ����ȥ���ú͸���ʱ��
            if (timeRate != E_TimeRate.Recovery && timeRate != E_TimeRate.Zero)
            {
                _timeRate = timeRate;
                UnityEngine.Time.timeScale = (int)_timeRate;
            }
            //���ڻָ�ʱ��ֱ������Ϊ�ϴ�ʱ���ٶȼ���
            else if(timeRate == E_TimeRate.Recovery)
            {
                UnityEngine.Time.timeScale = (int)_timeRate;
            }
            else
            {
                UnityEngine.Time.timeScale = (int)timeRate;
            }
        }
    }
}
