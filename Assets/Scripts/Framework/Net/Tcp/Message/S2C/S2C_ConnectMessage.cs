using System.Collections.Generic;

namespace Net.TCP.Message.S2C
{
    /// <summary>
    /// 服务器发送客户端_连接消息_2001
    /// </summary>
    public class S2C_ConnectMessage : TcpMessage
    {
        /// <summary>
        /// 所有已连接的客户端ID
        /// </summary>
        public List<int> ClientIds {  get; set; }

        /// <summary>
        /// 连接状态
        /// true：ClientID表示连入的客户端，ClientIds表示其它已连接的客户端ID
        /// false：ClientID表示断开的客户端，ClientIds不表示任何内容
        /// </summary>
        public bool ConnectState { get; set; }

        public override int GetMsgID()
        {
            return 2001;
        }

        protected override int GetBytesBodyLength()
        {
            return 4 + 4 * ClientIds.Count + 1;
        }

        protected override void SerializeBody(byte[] bytes, ref int index)
        {
            WriteField<List<int>>(bytes, ClientIds, ref index);
            WriteField(bytes, ConnectState, ref index);
        }

        protected override void DeserializeBody(byte[] bytes, ref int index)
        {
            ClientIds = ReadListInt(bytes, ref index);
            ConnectState = ReadBool(bytes, ref index);
        }

        public override string ToString()
        {
            return $"玩家：{ClientID}，{(ConnectState ? "已连接" : "断开连接")}";
        }
    }
}
