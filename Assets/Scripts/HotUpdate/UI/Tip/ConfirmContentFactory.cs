using System;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using HotUpdate.Base;
using HotUpdate.Base.Enums;
using HotUpdate.Base.UI;
using HotUpdate.UI.Begin;
using UnityEngine;

namespace HotUpdate.UI.Tip
{
    public class ConfirmContentFactory : IDisposable
    {
        [Inject] private ObjectSpawner _objectSpawner;

        private AssetUpdateConfirmContent _confirmContent;

        public async Task<IConfirmContent> CreateContent(EConfirmContent confirmContent, RectTransform root)
        {
            switch (confirmContent)
            {
                case EConfirmContent.ItemDelete:
                    return null;
                case EConfirmContent.AssetUpdate:
                    var assetUpdateConfirmContent = await _objectSpawner.SpawnAsync<AssetUpdateConfirmContent>(AssetKeys.AssetUpdateConfirmContent, root);
                    _confirmContent = assetUpdateConfirmContent;
                    DIContainer.InjectIntoInstance(assetUpdateConfirmContent);
                    return assetUpdateConfirmContent;
                default:
                    throw new ArgumentOutOfRangeException(nameof(confirmContent), confirmContent, null);
            }
        }

        public void Dispose()
        {
            _objectSpawner.Release(_confirmContent);
            _objectSpawner.Dispose();
            _objectSpawner = null;
        }
    }
}
