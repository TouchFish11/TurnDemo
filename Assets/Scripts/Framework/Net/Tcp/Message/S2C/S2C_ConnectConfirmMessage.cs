using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.TCP.Message.S2C
{
    /// <summary>
    /// 服务器发送客户端_连接确认消息_2008
    /// </summary>
    public class S2C_ConnectConfirmMessage : TcpMessage
    {
        public override int GetMsgID()
        {
            return 2008;
        }

        protected override void DeserializeBody(byte[] bytes, ref int index)
        {

        }

        protected override int GetBytesBodyLength()
        {
            return 0;
        }

        protected override void SerializeBody(byte[] bytes, ref int index)
        {

        }
    }
}
