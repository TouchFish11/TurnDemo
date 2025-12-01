using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Net.TCP.Message
{
    /// <summary>
    /// TCP消息
    /// </summary>
    public abstract class TcpMessage : BaseMessage
    {
        /// <summary>
        /// 客户端ID
        /// </summary>
        public int ClientID {  get; set; }

        /// <summary>
        /// 获取消息ID
        /// </summary>
        /// <returns></returns>
        public abstract int GetMsgID();

        /// <summary>
        /// 获取字节数组长度
        /// </summary>
        /// <returns></returns>
        public override int GetBytesLength()
        {
            //消息ID + 主体长 + 客户端ID + 主体
            return 4 + 4 + 4 + GetBytesBodyLength();
        }

        /// <summary>
        /// 获取字节数组的主体长度（不包含帧ID和消息ID）
        /// </summary>
        /// <returns></returns>
        protected abstract int GetBytesBodyLength();

        /// <summary>
        /// TCP消息序列化
        /// </summary>
        /// <returns></returns>
        public override byte[] Serialize()
        {
            int index = 0;
            byte[] bytes = new byte[GetBytesLength()];

            //写入消息ID
            WriteField<int>(bytes, GetMsgID(), ref index);
            //写入主体长度
            WriteField<int>(bytes, GetBytesBodyLength(), ref index);
            //写入客户端ID
            WriteField<int>(bytes, ClientID, ref index);
            //写入自定义字段
            SerializeBody(bytes, ref index);
            return bytes;
        }

        /// <summary>
        /// 序列化主体数据
        /// </summary>
        /// <returns></returns>
        protected abstract void SerializeBody(byte[] bytes, ref int index);

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="beginIndex"></param>
        /// <returns></returns>
        public override int Deserialize(byte[] bytes, int beginIndex = 0)
        {
            int index = beginIndex;
            //消息ID、主体长度外部已处理
            //读取客户端ID
            ClientID = ReadInt(bytes, ref index);
            //读取自定义主体
            DeserializeBody(bytes, ref index);
            return index - beginIndex;
        }

        /// <summary>
        /// 反序列化主体数据
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        protected abstract void DeserializeBody(byte[] bytes, ref int index);

    }
}
