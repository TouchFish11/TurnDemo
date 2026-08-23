namespace Net.Protocols
{
    /// <summary>
    /// 消息解析器接口
    /// </summary>
    public interface IMessageResolver
    {
        byte[] Serialize(Message message, EProtocolChannel channel);

        Message Deserialize(byte[] data, EProtocolChannel channel);
    }
}
