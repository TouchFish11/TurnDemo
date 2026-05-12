using System.Collections.Generic;
using Core.Net.FrameSync.Interface;
using UnityEngine;

namespace Core.Net.FrameSync.Manager
{
    public class NetGameManager
    {
        //����������Ŀͻ��˻��棺�����ͻ���ID��ֵ����Ҷ���
        private readonly Dictionary<int, INetObject> _idToPlayerMap = new Dictionary<int, INetObject>();
        private int priority;

        private NetGameManager()
        {

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
