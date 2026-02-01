using System.Collections.Generic;
using Core.Net.FrameSync.Interface;
using Core.Net.FrameSync.Manager;
using Core.Net.FrameSync.Udp.Command;
using UnityEngine;

namespace Core.Net.FrameSync.Udp
{
    /// <summary>
    /// ֡ͬ�����������
    /// </summary>
    public class FSFrameHandler
    {
        /// <summary>
        /// ָ������ṹ
        /// </summary>
        public struct CommandArg
        {
            /// <summary>
            /// �߼�ʱ��
            /// </summary>
            public float LogicTime { get; set; }
        }
        
        /// <summary>
        /// һָ֡����Ϣ�ṹ
        /// </summary>
        public struct OneFrameCommandInfo
        {
            // ��Ҷ���
            public INetObject netObject;
            // �ͻ���ָ֡��
            public ClientFrameCommand clientFrameCommand;

            public OneFrameCommandInfo(INetObject netObject, ClientFrameCommand clientFrameCommand)
            {
                this.netObject = netObject;
                this.clientFrameCommand = clientFrameCommand;
            }
        }

        /// <summary>
        /// ����֡ID
        /// </summary>
        public int FrameId {  get; set; }

        // ��һ֡��֡����
        private OneFrameCommand _lastFrameCommmand;

        /// <summary>
        /// �߼�֡ʱ��
        /// </summary>
        private const float LogicTime = 1 / 15f;

        // ¼��ط�ָ��
        private readonly SortedDictionary<int, OneFrameCommand> _rePlayCommand = new SortedDictionary<int, OneFrameCommand>();

        /// <summary>
        /// ���շ��������͵����ݰ�
        /// </summary>
        /// <param name="s2C_FrameCommand"></param>
        public void ServerCommandInput(S2C_FrameCommand s2C_FrameCommand)
        {
            // ִֻ�С�֡ID == ������ִ��֡ID+1������Ϣ����˳��ִ�У���������
            if (FrameId > s2C_FrameCommand.FrameId)
            {
                Debug.Log($"�յ�����֡{s2C_FrameCommand.FrameId}����ǰ��ִ�е�{FrameId}������");
                return;
            }

            // ��������������¼��طţ������ٱ���
            foreach (var item in s2C_FrameCommand.Commands)
            {
                _rePlayCommand.TryAdd(item.FrameID, item);
            }

            // �������һ֡���ݣ�ǿ�ƻع�����һ֡��״̬��������Ⱦ���״̬ǿ��ͬ�����߼���
            // ǿ�ƻع�����һ֡�����ݣ�����ִ�У��ع�����һ֡��״̬��
            // ��ʵ�����õ���һ֡������,��ִ��һ��
            // ...

            // ֻ�����߼�λ�ã����ع�
            UpdateLogic(_lastFrameCommmand);

            // ��ʼ׷֡����֡����֡������
            JumpFrame(s2C_FrameCommand);

            if (s2C_FrameCommand.Commands.Count > 0)
            {
                OneFrameCommand finalFrameCommand = s2C_FrameCommand.Commands[^1];
                // ִ�����һ֡
                SyncCommand(finalFrameCommand);

                // ִ����󣬽��߼�����������������һ�ν��յ�֡���ݻع�ʹ��
                // ���浱ǰ֡������,������һ��֡��������ʱ��,�ع�״̬�Ͳ���
                _lastFrameCommmand = finalFrameCommand;
            }

            // ���±���֡ID
            FrameId = s2C_FrameCommand.FrameId;

            // ִ�к�ͬ������
            SendFrameCommand();
        }

        /// <summary>
        /// ���͵�ǰ֡���ݵ�������
        /// </summary>
        private void SendFrameCommand()
        {
            // �ɼ���ǰָ֡����͸�������
            C2S_NextFrameCommand c2S_NextFrameCommand = new C2S_NextFrameCommand()
            {
                FrameId = FrameId + 1,
                ClientFrameCommand = new ClientFrameCommand()
                {
                    ClientID = NetManager.Instance.ClientID
                }
            };

            if (NetGameManager.Instance.TryGetPlayer(NetManager.Instance.ClientID, out INetObject netObject))
            {
                // ��ȡ���ƶ�����������ɼ�ָ��
                netObject.CollectInput(c2S_NextFrameCommand.ClientFrameCommand);
                // ����ָ���������
                NetManager.Instance.GetUdp().EnqueueCommand(c2S_NextFrameCommand);
            }
        }

        /// <summary>
        /// �����߼�״̬
        /// </summary>
        /// <param name="oneFrameCommand"></param>
        private void UpdateLogic(OneFrameCommand oneFrameCommand)
        {
            if (oneFrameCommand == null)
            {
                return;
            }

            CommandArg commandArg = new CommandArg();
            foreach (OneFrameCommandInfo oneFrameCommandInfo in ForeachOneFrameCommand(oneFrameCommand))
            {
                // ���ɸ�ָ������Ĳ���
                GenerateCommandArgs(oneFrameCommandInfo.clientFrameCommand.CommandType, ref commandArg);
                // �ع�
                oneFrameCommandInfo.netObject.SyncLogic(oneFrameCommandInfo.clientFrameCommand, commandArg);
            }
        }

        /// <summary>
        /// ׷֡
        /// </summary>
        /// <param name="s2C_FrameCommand"></param>
        private void JumpFrame(S2C_FrameCommand s2C_FrameCommand)
        {
            // ��������֡ID <= ����֡ID �� ˵������֡���Ѵ�����ֱ�ӷ���
            if (s2C_FrameCommand.FrameId <= FrameId)
            {
                Debug.Log($"��Ϣ��ͬ�����ˣ�������֡��{s2C_FrameCommand.FrameId}������֡��{FrameId}");
                // �����������������ݣ��ͻ�����ͬ�����ˣ��Ͳ��ô�����
                return;
            }

            CommandArg commandArg = new CommandArg();
            // ���ϴη��͵Ŀͻ���֡����������������֡
            foreach (OneFrameCommand oneFrameCommand in s2C_FrameCommand.Commands)
            {
                if (_lastFrameCommmand.FrameID == oneFrameCommand.FrameID)
                {
                    continue;
                }

                // ���ڱ���֡�Ҳ����ڷ���������֡ʱ����Ҫִ�У����ڷ����������֡�ţ�����׷֡�����κδ���
                if (oneFrameCommand.FrameID > FrameId && oneFrameCommand.FrameID != s2C_FrameCommand.FrameId)
                {
                    foreach (OneFrameCommandInfo oneFrameCommandInfo in ForeachOneFrameCommand(oneFrameCommand))
                    {
                        // ���ɸ�ָ������Ĳ���
                        GenerateCommandArgs(oneFrameCommandInfo.clientFrameCommand.CommandType, ref commandArg);
                        // ׷֡
                        oneFrameCommandInfo.netObject.ChaseFrame(oneFrameCommandInfo.clientFrameCommand, commandArg);
                    }
                }
            }
        }

        /// <summary>
        /// ͬ��ָ��
        /// </summary>
        /// <param name="oneFrameCommand"></param>
        private void SyncCommand(OneFrameCommand oneFrameCommand)
        {
            if (oneFrameCommand == null)
            {
                return;
            }

            CommandArg commandArg = new CommandArg();
            foreach (OneFrameCommandInfo oneFrameCommandInfo in ForeachOneFrameCommand(oneFrameCommand))
            {
                // ���ɸ�ָ������Ĳ���
                GenerateCommandArgs(oneFrameCommandInfo.clientFrameCommand.CommandType, ref commandArg);
                // ����ָ���ͬ��
                oneFrameCommandInfo.netObject.SyncFrame(oneFrameCommandInfo.clientFrameCommand);
            }
        }

        /// <summary>
        /// ����ָ������Ĳ���
        /// </summary>
        /// <param name="commandType"></param>
        /// <returns></returns>
        private void GenerateCommandArgs(byte commandType, ref CommandArg commandArg)
        {
            switch (commandType)
            {
                case 1:
                    commandArg.LogicTime = LogicTime;
                    break;
                default:
                    break;
            }
        }

        private IEnumerable<OneFrameCommandInfo> ForeachOneFrameCommand(OneFrameCommand oneFrameCommand)
        {
            if (oneFrameCommand.Commands.Count == 0)
            {
                yield break;
            }

            foreach (ClientFrameCommand clientFrameCommand in oneFrameCommand.Commands)
            {
                // ��ѯ�ĸ��ͻ���ID���в���
                if (!NetGameManager.Instance.TryGetPlayer(clientFrameCommand.ClientID, out INetObject netObject))
                {
                    continue;
                }

                yield return new OneFrameCommandInfo(netObject, clientFrameCommand);
            }
        }
    }
}
