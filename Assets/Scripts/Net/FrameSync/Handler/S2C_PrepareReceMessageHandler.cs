using Net.FrameSync.Tcp.Message;
using Net.FrameSync.Tcp.Message.S2C;

namespace Net.FrameSync.Handler
{
    /// <summary>
    /// ׼��������Ϣ������
    /// </summary>
    public class S2C_PrepareReceMessageHandler : MessageHandler<S2C_PrepareReceMessage>
    {
        public override S2C_PrepareReceMessage TcpMessage { get; set; }

        public override void HandleMessage(TcpMessage tcpMessage)
        {
            base.HandleMessage(tcpMessage);

            // // ����ȷ�Ͻ���
            // UIManager.Instance.HidePanel<ConfirmPanel>();
            //
            // // ��ʾ����������
            // UIManager.Instance.ShowPanelAsync<LoadingPanel>(E_UILayer.Top, (panel) =>
            // {
            //     // ���س���
            //     SceneManager.Instance.LoadSceneAsync("GameScene", UnityEngine.SceneManagement.LoadSceneMode.Single, (pro) =>
            //         {
            //             // ���½�����
            //             // ...
            //         },
            //         () =>
            //         {
            //             // ������ɫ
            //             var clientIds = TcpMessage.clientIds;
            //             for (int i = 0; i < clientIds.Count; i++)
            //             {
            //                 int clientId = clientIds[i];
            //
            //                 GameObject obj = AssetBundleLoadManager.Instance.LoadAsset<GameObject>(E_AssetBundleType.Prefab, "Player_1");
            //                 GameObject instance = GameObject.Instantiate(obj);
            //                 PlayerCharacter playerCharacter = instance.GetComponent<PlayerCharacter>();
            //
            //                 // �����ͻ���
            //                 if (NetManager.Instance.ClientID == clientId)
            //                 {
            //                     // �����������
            //                     InputComponent inputComponent = playerCharacter.AddComponent<InputComponent>();
            //                     // ʵ�����������
            //                     GameObject mainCamera = AssetBundleLoadManager.Instance.LoadAsset<GameObject>(E_AssetBundleType.Camera, "Main Camera");
            //                     GameObject mainCameraInstance = GameObject.Instantiate(mainCamera);
            //                     CameraController cameraController = mainCameraInstance.GetComponent<CameraController>();
            //                     cameraController.Init(playerCharacter.transform);
            //                     // ��������������ȴ���ʼ������Ϣ����
            //                     inputComponent.enabled = false;
            //                 }
            //
            //                 // ��ʼ�����
            //                 playerCharacter.Init(clientId);
            //                 // �洢���
            //                 NetGameManager.Instance.AddPlayer(clientId, playerCharacter);
            //             }
            //
            //             // ���͡�׼����������Ϣ��������
            //             NetManager.Instance.SendAsync(new C2S_ReadyMessage() { ClientID = NetManager.Instance.ClientID });
            //         });
            // });
        }
    }
}
