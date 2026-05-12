using System;
using System.Collections.Generic;

namespace Net.Sync
{
    /// <summary>
    /// 消息序列化器获取器
    /// </summary>
    public static class MessageSerializerGetter
    {
        private static readonly Dictionary<Type, IMessageSerializer> serializers = new()
        {
            {typeof(BinaryMessageSerializer),  new BinaryMessageSerializer()},
        };

        public static Func<IMessageSerializer> BinaryMessageSerializer => () => serializers[typeof(BinaryMessageSerializer)];
    }
}
