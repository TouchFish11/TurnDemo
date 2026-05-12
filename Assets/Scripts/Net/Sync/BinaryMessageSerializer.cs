using System;
using Net.Sync.Msg;

namespace Net.Sync
{
    /// <summary>
    /// 二进制消息序列化器
    /// </summary>
    public class BinaryMessageSerializer : IMessageSerializer
    {
        public byte[] Serialize(Message message, EProtocolChannel channel)
        {
            // 不可靠的直接返回消息本身的序列化数据，帧数据消息直接返回本体的序列化数据
            if (channel != EProtocolChannel.Reliable) return message.Serialize();
            
            // 可靠的需要解析消息头，为了区分消息类型
            var index = 0;
            // 添加自定义消息头
            var data = new byte[4 + 4 + message.GetMsgLength()];
            // 添加消息ID
            var msgID = MessageFactory.GetMessageID(message);
            Array.Copy(BitConverter.GetBytes(msgID), 0, data, index, 4);
            index += 4;
            // 添加消息体长度
            var msgLength = message.GetMsgLength();
            Array.Copy(BitConverter.GetBytes(msgLength), 0, data, index, 4);
            index += 4;
            // 写入消息本身
            Array.Copy(message.Serialize(), 0, data, index, msgLength);
            return data;
        } 

        public Message Deserialize(byte[] data, EProtocolChannel channel)
        {
            // 可靠的需要解析消息头，为了区分消息类型
            if (channel == EProtocolChannel.Reliable)
            {
                var index = 0;
                // 解析自定义消息头
                // 解析消息ID
                var msgID = BitConverter.ToInt32(data, 0);
                index += 4;
                // 解析消息长度
                var msgLength = BitConverter.ToInt32(data, index);
                index += 4;
                // 通过工厂创建消息
                return MessageFactory.CreateMessage(msgID, data, index);
            }

            // 不可靠的，直接反序列化数据后返回帧消息本身
            var frameMessage = new OneFrameMessage();
            frameMessage.Deserialize(data);
            return frameMessage;
        }
    }
}
