using System;

namespace HotUpdate.Core.Battle.Event
{
    /// <summary>
    /// ս���¼����߽ӿ�
    /// </summary>
    public interface IBattleEventBus
    {
        /// <summary>
        /// �����¼���ģ��ͨ���˷���ע���Լ�Ҫ�������¼���
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="callback"></param>
        void AddListener<TEvent>(Action<TEvent> callback) where TEvent : BattleEvent;

        /// <summary>
        /// �����¼�����������ͨ���˷���֪ͨ���ж����ߣ�
        /// </summary>
        /// <param name="battleEvent"></param>
        void TriggerEvent<TEvent>(TEvent battleEvent) where TEvent : BattleEvent;

        /// <summary>
        /// �Ƴ��¼�
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="callback"></param>
        void RemoveListener<TEvent>(Action<TEvent> callback) where TEvent : BattleEvent;

        /// <summary>
        /// ��������
        /// </summary>
        void Clear();
    }
}
