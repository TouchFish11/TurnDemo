using System.Collections.Generic;
using Core.Log;
using Core.Pool;
using Core.Service;
using UnityEngine.Events;

namespace Core.Time
{
    /// <summary>
    /// ʱ������(����)
    /// </summary>
    public class TimeChecker : ITimeChecker
    {
        // �洢ʱ������ֵ� Key��Ψһ����Value��ʱ�����
        private Dictionary<int, DateTime> _dateTimeDic = new();

        /// <summary>
        /// ʱ�����Ψһ��
        /// </summary>
        private static int TIME_KEY = 0;

        /// <summary>
        /// ����Ŀ��ʱ��
        /// </summary>
        /// <param name="currentTime">��ǰʱ��ṹ��</param>
        /// <param name="targetDay">ָ������</param>
        /// <param name="targetHour">ָ��Сʱ</param>
        /// <param name="targetMin">ָ������</param>
        /// <param name="targetSec">ָ������</param>
        /// <returns>ʱ������ӦKey</returns>
        public int CreateTargetTime(System.DateTime currentTime, int targetDay, int targetHour, int targetMin, int targetSec)
        {
            // ��������ָ��ʱ��� DateTime ����
            DateTime tagetTime = ServiceLocator.Get<IPoolManager>().GetData<DateTime>("GameUtility");
            //��ʼ��ʱ�����
            tagetTime = tagetTime.Init(currentTime, targetDay, targetHour, targetMin, targetSec);
            //�洢���ֵ�
            _dateTimeDic.Add(++TIME_KEY, tagetTime);
            //����ֵʱ���Ӧ�ļ�
            return TIME_KEY;
        }

        /// <summary>
        /// ����ʱ���������
        /// </summary>
        /// <param name="key">ʱ������Ӧ�ļ�</param>
        /// <param name="overCallBack">�����ص�</param>
        public void AddListener(int key, UnityAction overCallBack)
        {
            GetDateTime(key).OverCallBack += overCallBack;
        }

        /// <summary>
        /// ����ʣ��ʱ��
        /// </summary>
        /// <param name="current">��ǰʱ��</param>
        /// <param name="key">ʱ�����Key</param>
        /// <returns>��ǰʣ��ʱ�䣨�룩</returns>
        public long CalcRemainTime(System.DateTime current, int key)
        {
            if (_dateTimeDic.ContainsKey(key))
            {
                return _dateTimeDic[key].CalcRemainTime(current);
            }

            LogManager.LogError($"δ�ҵ���ָ����ʱ�����KEY��{key}");
            return default;
        }

        /// <summary>
        /// ��ȡ����Ӧ��ʱ�����
        /// </summary>
        /// <param name="key">ʱ�����Key</param>
        /// <returns>ʱ�����</returns>
        public DateTime GetDateTime(int key)
        {
            if (_dateTimeDic.ContainsKey(key))
                return _dateTimeDic[key];

            LogManager.LogError($"δ�ҵ���ָ����ʱ�����KEY��{key}");
            return default;
        }

        /// <summary>
        /// ���
        /// </summary>
        public void Clear()
        {
            _dateTimeDic.Clear();
            _dateTimeDic = null;
        }
    }
}