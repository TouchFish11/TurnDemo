using System.Threading.Tasks;

namespace Core.AssetBundles.Update
{
    /// <summary>
    /// AB包更新器接口
    /// </summary>
    public interface IAssetBundleUpdater
    {
        Task CheckUpdate();
        
        ABUpdateContext GetContext();
        
        void Init();
    }
}
