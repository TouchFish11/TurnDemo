using System;
using kcp2k;
using Net.Sync;
using Net.Sync.Msg;
using UnityEngine;
using KcpClient = kcp2k.KcpClient;

namespace Net.Test
{
    public class KcpClientTest : MonoBehaviour
    {
        private KcpClient client;
        public string serverIp = "127.0.0.1"; // 本地测试用，部署后改服务端IP
        public ushort serverPort = 8080;

        private void Start()
        {
            // 初始化KCP客户端，注册回调
            client = new KcpClient(OnConnected, OnMessageReceived, OnDisconnected, OnError, new KcpConfig());
            // 连接服务端
            client.Connect(serverIp, serverPort);
            Debug.Log("正在连接KCP服务端...");
        }

        // Unity每帧调用，驱动KCP协议
        private void Update()
        {
            client?.Tick();
        }

        // 连接成功回调
        private void OnConnected()
        {
            Debug.Log("连接服务端成功！");
            // 给服务端发消息
            var msg = System.Text.Encoding.UTF8.GetBytes("控制台服务端你好！我是Unity客户端");
            client.Send(new ArraySegment<byte>(msg), KcpChannel.Reliable);
        }

        // 接收服务端数据回调
        private static void OnMessageReceived(ArraySegment<byte> message, KcpChannel channel)
        {
            Debug.Log($"收到服务端：{message}");
        }

        // 断开连接回调
        private static void OnDisconnected()
        {
            Debug.Log("与服务端断开连接");
        }

        // 错误回调
        private static void OnError(ErrorCode code, string message)
        {
            Debug.LogError($"连接错误：{code}.{message}");
        }

        // 关闭Unity时断开连接
        private void OnDestroy()
        {
            client?.Disconnect();
        }
    }
}
