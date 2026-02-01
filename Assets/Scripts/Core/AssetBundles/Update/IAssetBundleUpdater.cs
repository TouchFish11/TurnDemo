using System.Threading.Tasks;
using Core.AssetBundles.Update.Enum;

namespace Core.AssetBundles.Update
{
    /// <summary>
    /// AB包更新器接口
    /// </summary>
    public interface IAssetBundleUpdater
    {
        void ChangeState(EUpdatePhase updatePhase);
        Task<bool> CheckUpdate();
        ABUpdateContext GetContext();
        void Init();
    }
}
