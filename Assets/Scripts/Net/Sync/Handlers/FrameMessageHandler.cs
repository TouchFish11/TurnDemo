using Net.Sync.Msg;

namespace Net.Sync.Handlers
{
    /// <summary>
    /// 帧消息处理器
    /// </summary>
    public class FrameMessageHandler : MessageHandler<OneFrameMessage>
    {
        private OneFrameMessage _lastFrameMsg;
        private const float LogicTime = 1 / 15f;
        
        
        public int FrameId {  get; set; }
        public override OneFrameMessage Message { get; protected set; }
    
        // /// <summary>
        // /// ���շ��������͵����ݰ�
        // /// </summary>
        // /// <param name="oneFrameMessage"></param>
        // public void ServerCommandInput(OneFrameMessage oneFrameMessage)
        // {
        //     // ִֻ�С�֡ID == ������ִ��֡ID+1������Ϣ����˳��ִ�У���������
        //     if (FrameId > oneFrameMessage.FrameID)
        //     {
        //         Debug.Log($"�յ�����֡{oneFrameMessage.FrameID}����ǰ��ִ�е�{FrameId}������");
        //         return;
        //     }
        //
        //     // ��������������¼��طţ������ٱ���
        //     foreach (var optMessage in oneFrameMessage.OptMessages)
        //     {
        //         _rePlayCommand.TryAdd(oneFrameMessage.FrameID, optMessage);
        //     }
        //
        //     // �������һ֡���ݣ�ǿ�ƻع�����һ֡��״̬��������Ⱦ���״̬ǿ��ͬ�����߼���
        //     // ǿ�ƻع�����һ֡�����ݣ�����ִ�У��ع�����һ֡��״̬��
        //     // ��ʵ�����õ���һ֡������,��ִ��һ��
        //     // ...
        //
        //     // ֻ�����߼�λ�ã����ع�
        //     UpdateLogic(_lastFrameMsg);
        //
        //     if (oneFrameMessage.OptMessages.Count > 0)
        //     {
        //         var finalFrameCommand = oneFrameMessage.OptMessages[^1];
        //         // ִ�����һ֡
        //         SyncCommand(finalFrameCommand);
        //
        //         // ִ����󣬽��߼�����������������һ�ν��յ�֡���ݻع�ʹ��
        //         // ���浱ǰ֡������,������һ��֡��������ʱ��,�ع�״̬�Ͳ���
        //         _lastFrameMsg = oneFrameMessage;
        //     }
        //
        //     // ���±���֡ID
        //     FrameId = oneFrameMessage.FrameID;
        //
        //     // ִ�к�ͬ������
        //     SendFrameCommand();
        // }
        //
        // /// <summary>
        // /// ���͵�ǰ֡���ݵ�������
        // /// </summary>
        // private void SendFrameCommand()
        // {
        //     // �ɼ���ǰָ֡����͸�������
        //     var c2S_NextFrameCommand = new OneFrameMessage()
        //     {
        //         FrameID = FrameId + 1,
        //         OptMessages = new List<OptMessage>()
        //     };
        //
        //     if (NetGameManager.Instance.TryGetPlayer(NetManager.Instance.ClientID, out INetObject netObject))
        //     {
        //         // ��ȡ���ƶ�����������ɼ�ָ��
        //         netObject.CollectInput(c2S_NextFrameCommand.ClientFrameCommand);
        //         // ����ָ���������
        //         NetManager.Instance.GetUdp().EnqueueCommand(c2S_NextFrameCommand);
        //     }
        // }
        //
        // /// <summary>
        // /// �����߼�״̬
        // /// </summary>
        // /// <param name="oneFrameMessage"></param>
        // private void UpdateLogic(OneFrameMessage oneFrameMessage)
        // {
        //     if (oneFrameMessage == null)
        //     {
        //         return;
        //     }
        //
        //     CommandArg commandArg = new CommandArg();
        //     foreach (OneFrameCommandInfo oneFrameCommandInfo in ForeachOneFrameCommand(oneFrameMessage))
        //     {
        //         // ���ɸ�ָ������Ĳ���
        //         GenerateCommandArgs(oneFrameCommandInfo.clientFrameCommand.CommandType, ref commandArg);
        //         // �ع�
        //         oneFrameCommandInfo.netObject.SyncLogic(oneFrameCommandInfo.clientFrameCommand, commandArg);
        //     }
        // }
        //
        // /// <summary>
        // /// ͬ��ָ��
        // /// </summary>
        // /// <param name="oneFrameCommand"></param>
        // private void SyncCommand(OneFrameCommand oneFrameCommand)
        // {
        //     if (oneFrameCommand == null)
        //     {
        //         return;
        //     }
        //
        //     CommandArg commandArg = new CommandArg();
        //     foreach (OneFrameCommandInfo oneFrameCommandInfo in ForeachOneFrameCommand(oneFrameCommand))
        //     {
        //         // ���ɸ�ָ������Ĳ���
        //         GenerateCommandArgs(oneFrameCommandInfo.clientFrameCommand.CommandType, ref commandArg);
        //         // ����ָ���ͬ��
        //         oneFrameCommandInfo.netObject.SyncFrame(oneFrameCommandInfo.clientFrameCommand);
        //     }
        // }
        
        protected override void OnHandleMessage()
        {
        
        }
    }
}
