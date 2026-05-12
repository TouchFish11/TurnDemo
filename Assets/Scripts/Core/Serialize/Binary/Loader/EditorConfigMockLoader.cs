using System.Threading.Tasks;
using Core.DI;
using Core.EditorRes;
using UnityEngine;

namespace Core.Serialize.Binary.Loader
{
    /// <summary>
    /// 编辑器配置模拟加载器
    /// </summary>
    public class EditorConfigMockLoader : EditorConfigLoader
    {
        [Inject] private IEditorResManager _editorResManager;
        
        public override async Task LoadConfigAsync<T, K>()
        {
            // 加载编辑器数据
            var config = _editorResManager.LoadEditorAsset<TextAsset>($"{typeof(K).Name}", ".bytes");
            await Task.CompletedTask;
            ConvertFrom<T, K>(config);
        }
    }
}
