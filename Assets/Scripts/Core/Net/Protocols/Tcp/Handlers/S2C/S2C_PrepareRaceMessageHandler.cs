using Net.Protocols;
using Net.Protocols.Tcp.Messages.Battle.S2C;

namespace Core.Net.Protocols.Tcp.Handlers.S2C
{
    public class S2C_PrepareRaceMessageHandler : MessageHandler<S2C_PrepareRaceMessage>
    {
        public override S2C_PrepareRaceMessage Message { get; protected set; }
        
        protected override void OnHandle()
        {
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
