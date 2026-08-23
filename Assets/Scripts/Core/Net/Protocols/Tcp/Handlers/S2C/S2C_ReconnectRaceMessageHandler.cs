using Core.Net.Protocols.Tcp.Messages.Common;
using Net.Protocols;

namespace Core.Net.Protocols.Tcp.Handlers.S2C
{
    /// <summary>
    /// 服务器重新连接消息
    /// </summary>
    public class S2C_ReconnectRaceMessageHandler : MessageHandler<ReconnectRaceMessage>
    {
        public override ReconnectRaceMessage Message { get; protected set; }
        
        protected override void OnHandle()
        {
            // if (Message.ClientID == NetManager.Instance.ClientID)
            // {
            //     //GameHandler.Instance.IsStop = false;
            // }
            //Debug.Log($"��ң�{Message.ClientID}����������");
        }
    }
}
