using System;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using HotUpdate.Base.Enums;
using HotUpdate.Base.UI;
using HotUpdate.UI.Begin;
using HotUpdate.UI.Inventory;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HotUpdate.UI.Tip
{
    public class ConfirmContentFactory : IDisposable
    {
        [Inject] private ObjectSpawner _objectSpawner;
        
        private IConfirmContent _confirmContent;

        public async Task<IConfirmContent> CreateContent(EConfirmContent confirmContent, RectTransform root)
        {
            switch (confirmContent)
            {
                case EConfirmContent.ItemDelete:
                    _confirmContent = await _objectSpawner.SpawnAsync<DeleteItemConfirmContent>(AssetKeys.DeleteItemConfirmContent, root);
                    DIContainer.InjectIntoInstance(_confirmContent);
                    return _confirmContent;
                case EConfirmContent.AssetUpdate:
                    _confirmContent = await _objectSpawner.SpawnAsync<AssetUpdateConfirmContent>(AssetKeys.AssetUpdateConfirmContent, root);
                    DIContainer.InjectIntoInstance(_confirmContent);
                    return _confirmContent;
                default:
                    throw new ArgumentOutOfRangeException(nameof(confirmContent), confirmContent, null);
            }
        }

        public void Dispose()
        {
            _objectSpawner.Release((Object)_confirmContent);
            _objectSpawner.Dispose();
            _objectSpawner = null;
        }
    }
}
