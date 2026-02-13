using System.Threading.Tasks;

namespace Core.AssetBundles.Management
{
    public interface IAssetBundleInitListener
    {
        Task OnAssetBundleInited();
    }
}
