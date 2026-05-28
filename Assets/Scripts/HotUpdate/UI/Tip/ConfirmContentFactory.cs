using System;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using HotUpdate.Base;
using HotUpdate.Base.Enums;
using HotUpdate.UI.Begin;
using UnityEngine;

namespace HotUpdate.UI.Tip
{
    public class ConfirmContentFactory : IDisposable
    {
        [Inject] private ObjectSpawner _objectSpawner;

        private PoolObject _confirmContent;

        public async Task<IConfirmContent> CreateContent(EConfirmContent confirmContent, RectTransform root)
        {
            switch (confirmContent)
            {
                case EConfirmContent.ItemDelete:
                    return null;
                case EConfirmContent.AssetUpdate:
                    var poolObj = await _objectSpawner.SpawnAsync<AssetUpdateConfirmContent>(AssetKeys.AssetUpdateConfirmContent, root);
                    _confirmContent = poolObj;
                    DIContainer.InjectIntoInstance(poolObj.Obj);
                    return poolObj.Obj;
                default:
                    throw new ArgumentOutOfRangeException(nameof(confirmContent), confirmContent, null);
            }
        }

        public void Dispose()
        {
            _confirmContent.Collect();
            _objectSpawner.Dispose();
            _objectSpawner = null;
        }
    }
}
