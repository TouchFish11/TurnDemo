using System;
using System.Collections.Generic;
using Net.Protocols;

namespace Core.Net.Protocols
{
    /// <summary>
    /// 消息解析器源
    /// </summary>
    public static class MessageSerializerSource
    {
        private static readonly Dictionary<Type, IMessageResolver> serializers = new()
        {
            {typeof(DefaultMessageResolver),  new DefaultMessageResolver()},
        };

        public static IMessageResolver DefaultMessageResolver => serializers[typeof(DefaultMessageResolver)];
    }
}
