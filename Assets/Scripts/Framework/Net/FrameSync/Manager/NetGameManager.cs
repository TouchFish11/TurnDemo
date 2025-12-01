using Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Net.TCP
{
    /// <summary>
    /// 网络游戏管理器
    /// ——管理玩家联机
    /// </summary>
    public class NetGameManager : SingletonBase<NetGameManager>
    {
        //连入服务器的客户端缓存：键：客户端ID；值：玩家对象
        private readonly Dictionary<int, INetObject> _idToPlayerMap = new Dictionary<int, INetObject>();

        private NetGameManager()
        {

        }

        /// <summary>
        /// 获取玩家对象
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="playerCharacter"></param>
        /// <returns></returns>
        public bool TryGetPlayer(int clientId, out INetObject netObject)
        {
            return _idToPlayerMap.TryGetValue(clientId, out netObject);
        }

        /// <summary>
        /// 添加玩家对象
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="playerCharacter"></param>
        public void AddPlayer(int clientId, INetObject netObject)
        {
            if (_idToPlayerMap.TryAdd(clientId, netObject))
            {
                Debug.Log($"玩家：{clientId}，加入游戏");
            }
        }
    }
}
