using Core.Pool;
using UnityEngine.Events;

namespace Core.Time
{
    /// <summary>
    /// ����ʱ��
    /// </summary>
    public class DateTime : IPoolData
    {
        // ָ��������
        private int _targetDay;
        // ָ����Сʱ��
        private int _targetHour;
        // ָ���ķ�����
        private int _targetMinute;
        // ָ��������
        private int _targetSecond;
        //������ʱ��
        private System.DateTime realTargetTime;
        //ʣ��ʱ��Ϊ0ʱ�Ļص�
        public event UnityAction OverCallBack;

        /// <summary>
        /// ��ʼ��ʱ�����
        /// </summary>
        /// <param name="currentTime">��ǰʱ��ṹ��</param>
        /// <param name="targetDay">ָ������</param>
        /// <param name="targetHour">ָ��Сʱ</param>
        /// <param name="targetMin">ָ������</param>
        /// <param name="targetSec">ָ������</param>
        /// <returns>ʱ�����</returns>
        public DateTime Init(System.DateTime currentTime, int targetDay, int targetHour, int targetMin, int targetSec)
        {
            _targetDay = targetDay;
            _targetHour = targetHour;
            _targetMinute = targetMin;
            _targetSecond = targetSec;

            //�������
            CheckDays(currentTime);

            return this;
        }

        /// <summary>
        /// ����ʣ��ʱ��
        /// </summary>
        /// <param name="currentTime">��ǰʱ��ṹ��</param>
        /// <returns>ʣ��ʱ�䣨�룩</returns>
        public long CalcRemainTime(System.DateTime currentTime)
        {
            // ��������ָ��ʱ���Ѿ���ȥ����ôĿ��ʱ�����´ε�ָ��ʱ��
            if (currentTime > realTargetTime)
            {
                //�������
                CheckDays(currentTime);

                //ʣ��ʱ��С�ڵ���0��ִ�лص�
                if ((long)(realTargetTime - currentTime).TotalSeconds <= 0)
                {
                    OverCallBack?.Invoke();
                }

                return (long)(realTargetTime - currentTime).TotalSeconds;
            }
            // ����Ŀ��ʱ����ǽ����ָ��ʱ��
            return (long)(realTargetTime - currentTime).TotalSeconds;
        }

        public void ResetData()
        {
            OverCallBack = null;
        }

        /// <summary>
        /// �������
        /// </summary>
        /// <param name="currentTime">��ǰʱ��ṹ��</param>
        private void CheckDays(System.DateTime currentTime)
        {
            if (_targetDay != 0)
            {
                if (currentTime.Day + _targetDay > System.DateTime.DaysInMonth(currentTime.Year, currentTime.Month))
                {
                    int deltaDay = 0;
                    //��ȡ��ǰ�¹��ж�����
                    int daysInMonth = System.DateTime.DaysInMonth(currentTime.Year, currentTime.Month);
                    //������� = ��ǰ���� + Ŀ������ - ��ǰ��������
                    deltaDay = currentTime.Day + _targetDay - daysInMonth;
                    //����������ڵ�ǰ��������
                    while (deltaDay > daysInMonth)
                    {
                        //�õ�ǰʱ���һ����
                        currentTime = currentTime.AddMonths(1);
                        //�����������ȥ��ǰʱ�������
                        deltaDay = deltaDay - currentTime.Day;
                    }

                    realTargetTime = new System.DateTime(currentTime.Year, currentTime.Month, deltaDay, _targetHour, _targetMinute, _targetSecond);
                }
                else
                {
                    realTargetTime = new System.DateTime(currentTime.Year, currentTime.Month, currentTime.Day + _targetDay, _targetHour, _targetMinute, _targetSecond);
                }
            }
            else
            {
                realTargetTime = new System.DateTime(currentTime.Year, currentTime.Month, currentTime.Day + 1, _targetHour, _targetMinute, _targetSecond);
                if (currentTime < realTargetTime.AddDays(-1))
                {
                    realTargetTime = realTargetTime.AddDays(-1);
                }
            }
        }
    }
}
