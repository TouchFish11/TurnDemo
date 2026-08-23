using System.Collections.Generic;
using Net.Protocols.Configs;

namespace Net.Protocols.Tcp.Messages.Battle.S2C
{
    /// <summary>
    /// 准备比赛消息，客户端加载、初始化比赛相关资源
    /// </summary>
    public class S2C_PrepareRaceMessage : TcpMessage
    {
        // 当前同一比赛的所有客户端ID
        public List<int> clientIds;

        public override int GetMsgID()
        {
            return MessageIDConfig.S2C_PrepareRace_ID;
        }

        protected override int OnGetBodyLength()
        {
            // clientIds长度 + clientIds主体
            return 4 + 4 * clientIds.Count;
        }

        protected override void SerializeBody(byte[] bytes, ref int index)
        {
            MessageUtil.WriteField(bytes, clientIds, ref index);
        }

        protected override void DeserializeBody(byte[] bytes, ref int index)
        {
            var length = MessageUtil.ReadInt(bytes, ref index);
            clientIds = new List<int>(length);
            for (var i = 0; i < length; i++)
            {
                clientIds.Add(MessageUtil.ReadInt(bytes, ref index));
            }
        }
    }
}
