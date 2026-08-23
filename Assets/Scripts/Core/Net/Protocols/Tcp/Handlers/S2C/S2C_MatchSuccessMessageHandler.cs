using Net.Protocols;
using Net.Protocols.Tcp.Messages.Battle.S2C;

namespace Core.Net.Protocols.Tcp.Handlers.S2C
{
    /// <summary>
    /// 服务器匹配成功消息处理器
    /// </summary>
    public class S2C_MatchSuccessMessageHandler : MessageHandler<S2C_MatchSuccessMessage>
    {
        public override S2C_MatchSuccessMessage Message { get; protected set; }
        
        protected override void OnHandle()
        {
            
        }
    }
}
