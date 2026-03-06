using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Net.FrameSync.Interface;
using Core.Singleton;
using UnityEngine;

namespace Core.Net.FrameSync.Manager
{
    /// <summary>
    /// ������Ϸ������
    /// ���������������
    /// </summary>
    public class NetGameManager : SingletonBase<NetGameManager>
    {
        public override int Priority => -1;

        //����������Ŀͻ��˻��棺�����ͻ���ID��ֵ����Ҷ���
        private readonly Dictionary<int, INetObject> _idToPlayerMap = new Dictionary<int, INetObject>();
        private int priority;

        private NetGameManager()
        {

        }

        public override Task InitAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// ��ȡ��Ҷ���
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="playerCharacter"></param>
        /// <returns></returns>
        public bool TryGetPlayer(int clientId, out INetObject netObject)
        {
            return _idToPlayerMap.TryGetValue(clientId, out netObject);
        }

        /// <summary>
        /// ������Ҷ���
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="playerCharacter"></param>
        public void AddPlayer(int clientId, INetObject netObject)
        {
            if (_idToPlayerMap.TryAdd(clientId, netObject))
            {
                Debug.Log($"��ң�{clientId}��������Ϸ");
            }
        }
    }
}
