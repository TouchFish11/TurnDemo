using System;

namespace Net.TCP.Message.C2S
{
    /// <summary>
    /// 客户端发送服务器_心跳消息_1000
    /// </summary>
    public class C2S_HeartMessage : TcpMessage
    {
        public override int GetMsgID()
        {
            return 1000;
        }

        protected override int GetBytesBodyLength()
        {
            return 0;
        }

        protected override void SerializeBody(byte[] bytes, ref int index) { }

        protected override void DeserializeBody(byte[] bytes, ref int index) { }

        public override string ToString()
        {
            return $"玩家：{ClientID}的心跳消息——{DateTime.Now}";
        }
    }
}
