using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Net.TCP.Message.C2S
{
    /// <summary>
    /// 客户端发送服务器_连接确认消息_1008
    /// </summary>
    public class C2S_ConnectConfirmMessage : TcpMessage
    {
        public override int GetMsgID()
        {
            return 1008;
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
