using System;

namespace Game.Battle.Core
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
        void AddListener<TEvent>(Action<TEvent> callback) where TEvent : Game.Battle.BattleEvent;

        /// <summary>
        /// �����¼�����������ͨ���˷���֪ͨ���ж����ߣ�
        /// </summary>
        /// <param name="battleEvent"></param>
        void TriggerEvent<TEvent>(TEvent battleEvent) where TEvent : Game.Battle.BattleEvent;

        /// <summary>
        /// �Ƴ��¼�
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="callback"></param>
        void RemoveListener<TEvent>(Action<TEvent> callback) where TEvent : Game.Battle.BattleEvent;

        /// <summary>
        /// ��������
        /// </summary>
        void Clear();
    }
}
