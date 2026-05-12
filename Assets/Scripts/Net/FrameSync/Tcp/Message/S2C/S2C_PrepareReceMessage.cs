using System.Collections.Generic;

namespace Net.FrameSync.Tcp.Message.S2C
{
    /// <summary>
    /// 服务器发送客户端_准备比赛消息_2004
    /// </summary>
    public class S2C_PrepareReceMessage : TcpMessage
    {
        // 当前同一比赛的所有客户端ID
        public List<int> clientIds;

        public override int GetMsgID()
        {
            return 2004;
        }

        protected override int GetBytesBodyLength()
        {
            // clientIds长度 + clientIds主体
            return 4 + 4 * clientIds.Count;
        }

        protected override void SerializeBody(byte[] bytes, ref int index)
        {
            WriteField(bytes, clientIds, ref index);
        }

        protected override void DeserializeBody(byte[] bytes, ref int index)
        {
            clientIds = ReadListInt(bytes, ref index);
        }
    }
}
