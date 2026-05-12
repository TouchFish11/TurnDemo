using System.Threading.Tasks;
using Core.DI;
using Core.EditorRes;
using UnityEngine;

namespace Core.Serialize.Binary.Loader
{
    /// <summary>
    /// Excel配置模拟加载器
    /// </summary>
    public class ExcelConfigMockLoader : ExcelConfigLoader
    {
        [Inject] private IEditorResManager _editorResManager;
        
        public override async Task LoadConfigAsync<T, K>()
        {
            // 加载编辑器数据
            var config = _editorResManager.LoadEditorAsset<TextAsset>($"{typeof(K).Name}");
            await Task.CompletedTask;
            // 转换二进制到数据类
            ConvertFrom<T, K>(config);
        }
    }
}
