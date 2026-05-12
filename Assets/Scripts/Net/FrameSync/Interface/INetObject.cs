using Core.Components;
using Net.FrameSync.Command;
using UnityEngine;
using static Net.FrameSync.Handler.FSFrameHandler;

namespace Net.FrameSync.Interface
{
    /// <summary>
    /// �������ӿ�
    /// </summary>
    public interface INetObject
    {
        /// <summary>
        /// ����λ��
        /// </summary>
        Transform Transform { get; }

        IEntityObject Character { get; }

        /// <summary>
        /// �ռ�����
        /// </summary>
        /// <param name="clientFrameCommand"></param>
        void CollectInput(ClientFrameCommand clientFrameCommand);

        /// <summary>
        /// ͬ���߼�״̬
        /// </summary>
        /// <param name="clientFrameCommand"></param>
        void SyncLogic(ClientFrameCommand clientFrameCommand, CommandArg commandArg);

        /// <summary>
        /// ׷֡
        /// </summary>
        /// <param name="clientFrameCommand"></param>
        /// <param name="commandArg"></param>
        void ChaseFrame(ClientFrameCommand clientFrameCommand, CommandArg commandArg);

        /// <summary>
        /// ͬ��֡
        /// </summary>
        /// <param name="clientFrameCommand"></param>
        void SyncFrame(ClientFrameCommand clientFrameCommand);
    }
}
