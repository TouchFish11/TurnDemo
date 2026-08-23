using System.Collections.Generic;
using Core.Net.SyncModule.Interface;

namespace Core.Net.SyncModule.Manager
{
    public class NetGameManager
    {
        private readonly Dictionary<int, INetObject> _idToPlayerMap = new();

        private NetGameManager()
        {

        }

        public bool TryGetPlayer(int clientId, out INetObject netObject)
        {
            return _idToPlayerMap.TryGetValue(clientId, out netObject);
        }

        public void AddPlayer(int clientId, INetObject netObject)
        {
            if (_idToPlayerMap.TryAdd(clientId, netObject))
            {

            }
        }
    }
}
