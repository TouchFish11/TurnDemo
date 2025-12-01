using Framework;
using Net.FrameSync.Command;
using Net.TCP;
using System.Collections.Generic;
using UnityEngine;

namespace Net.FrameSync
{
    /// <summary>
    /// 帧同步管理器组件
    /// </summary>
    public class FSFrameHandler
    {
        /// <summary>
        /// 指令参数结构
        /// </summary>
        public struct CommandArg
        {
            /// <summary>
            /// 逻辑时间
            /// </summary>
            public float LogicTime { get; set; }
        }
        
        /// <summary>
        /// 一帧指令信息结构
        /// </summary>
        public struct OneFrameCommandInfo
        {
            // 玩家对象
            public INetObject netObject;
            // 客户端帧指令
            public ClientFrameCommand clientFrameCommand;

            public OneFrameCommandInfo(INetObject netObject, ClientFrameCommand clientFrameCommand)
            {
                this.netObject = netObject;
                this.clientFrameCommand = clientFrameCommand;
            }
        }

        /// <summary>
        /// 本地帧ID
        /// </summary>
        public int FrameId {  get; set; }

        // 上一帧的帧数据
        private OneFrameCommand _lastFrameCommmand;

        /// <summary>
        /// 逻辑帧时间
        /// </summary>
        private const float LogicTime = 1 / 15f;

        // 录像回放指令
        private readonly SortedDictionary<int, OneFrameCommand> _rePlayCommand = new SortedDictionary<int, OneFrameCommand>();

        /// <summary>
        /// 接收服务器发送的数据包
        /// </summary>
        /// <param name="s2C_FrameCommand"></param>
        public void ServerCommandInput(S2C_FrameCommand s2C_FrameCommand)
        {
            // 只执行“帧ID == 本地已执行帧ID+1”的消息（按顺序执行，避免乱序）
            if (FrameId > s2C_FrameCommand.FrameId)
            {
                Debug.Log($"收到乱序帧{s2C_FrameCommand.FrameId}，当前已执行到{FrameId}，丢弃");
                return;
            }

            // 保存起来，用于录像回放，保存再本地
            foreach (var item in s2C_FrameCommand.Commands)
            {
                _rePlayCommand.TryAdd(item.FrameID, item);
            }

            // 如果有上一帧数据，强制回滚到上一帧的状态。即将渲染层的状态强制同步到逻辑层
            // 强制回滚到上一帧的数据，进行执行（回滚到上一帧的状态）
            // 其实就是拿到上一帧的命令,再执行一次
            // ...

            // 只更新逻辑位置，不回滚
            UpdateLogic(_lastFrameCommmand);

            // 开始追帧（跳帧、补帧）操作
            JumpFrame(s2C_FrameCommand);

            if (s2C_FrameCommand.Commands.Count > 0)
            {
                OneFrameCommand finalFrameCommand = s2C_FrameCommand.Commands[^1];
                // 执行最后一帧
                SyncCommand(finalFrameCommand);

                // 执行完后，将逻辑保存起来，用于下一次接收到帧数据回滚使用
                // 保存当前帧的数据,用于下一次帧数据来的时候,回滚状态和操作
                _lastFrameCommmand = finalFrameCommand;
            }

            // 更新本地帧ID
            FrameId = s2C_FrameCommand.FrameId;

            // 执行后同步发送
            SendFrameCommand();
        }

        /// <summary>
        /// 发送当前帧数据到服务器
        /// </summary>
        private void SendFrameCommand()
        {
            // 采集当前帧指令，发送给服务器
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
                // 获取到移动输入组件，采集指令
                netObject.CollectInput(c2S_NextFrameCommand.ClientFrameCommand);
                // 发送指令给服务器
                NetManager.Instance.GetUdp().EnqueueCommand(c2S_NextFrameCommand);
            }
        }

        /// <summary>
        /// 更新逻辑状态
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
                // 生成该指令所需的参数
                GenerateCommandArgs(oneFrameCommandInfo.clientFrameCommand.CommandType, ref commandArg);
                // 回滚
                oneFrameCommandInfo.netObject.SyncLogic(oneFrameCommandInfo.clientFrameCommand, commandArg);
            }
        }

        /// <summary>
        /// 追帧
        /// </summary>
        /// <param name="s2C_FrameCommand"></param>
        private void JumpFrame(S2C_FrameCommand s2C_FrameCommand)
        {
            // 若服务器帧ID <= 本地帧ID → 说明所有帧都已处理，直接返回
            if (s2C_FrameCommand.FrameId <= FrameId)
            {
                Debug.Log($"消息已同步过了：服务器帧：{s2C_FrameCommand.FrameId}，本地帧：{FrameId}");
                // 服务器发送来的数据，客户端若同步过了，就不用处理了
                return;
            }

            CommandArg commandArg = new CommandArg();
            // 从上次发送的客户端帧，到服务器的最新帧
            foreach (OneFrameCommand oneFrameCommand in s2C_FrameCommand.Commands)
            {
                if (_lastFrameCommmand.FrameID == oneFrameCommand.FrameID)
                {
                    continue;
                }

                // 大于本地帧且不等于服务器最新帧时，都要执行；等于服务器的最大帧号，不在追帧中做任何处理
                if (oneFrameCommand.FrameID > FrameId && oneFrameCommand.FrameID != s2C_FrameCommand.FrameId)
                {
                    foreach (OneFrameCommandInfo oneFrameCommandInfo in ForeachOneFrameCommand(oneFrameCommand))
                    {
                        // 生成该指令所需的参数
                        GenerateCommandArgs(oneFrameCommandInfo.clientFrameCommand.CommandType, ref commandArg);
                        // 追帧
                        oneFrameCommandInfo.netObject.ChaseFrame(oneFrameCommandInfo.clientFrameCommand, commandArg);
                    }
                }
            }
        }

        /// <summary>
        /// 同步指令
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
                // 生成该指令所需的参数
                GenerateCommandArgs(oneFrameCommandInfo.clientFrameCommand.CommandType, ref commandArg);
                // 处理指令——同步
                oneFrameCommandInfo.netObject.SyncFrame(oneFrameCommandInfo.clientFrameCommand);
            }
        }

        /// <summary>
        /// 生成指令所需的参数
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
                // 查询哪个客户端ID进行操作
                if (!NetGameManager.Instance.TryGetPlayer(clientFrameCommand.ClientID, out INetObject netObject))
                {
                    continue;
                }

                yield return new OneFrameCommandInfo(netObject, clientFrameCommand);
            }
        }
    }
}
