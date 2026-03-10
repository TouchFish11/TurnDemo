using System.Threading.Tasks;
using Core.Input.ActionAsset;
using Core.Log;
using Core.Music;
using Core.Serialize.Binary;
using Core.Utility;
using HotUpdate.Common;
using HotUpdate.Core.Main;
using HotUpdate.Core.Provider;

namespace HotUpdate.Main.Data
{
    /// <summary>
    /// 主模块数据提供器
    /// </summary>
    public class MainDataProvider : IDataProvider<IMainDataCollection>
    {
        private readonly IBinaryDataManager _binaryDataManager;
        // 主数据集合
        private IMainDataCollection _mainDataCollection;
        
        public MainDataProvider(IBinaryDataManager binaryDataManager)
        {
            _binaryDataManager = binaryDataManager;
        }

        public async Task LoadDataAsync()
        {
            // 添加配置数据
            _binaryDataManager.AddConfig(EConfigLoadType.Excel, async loader =>
            {
                await loader.LoadConfigAsync<RoleInfoContainer, RoleInfo>();
                await loader.LoadConfigAsync<MonsterInfoContainer, MonsterInfo>();
                await loader.LoadConfigAsync<SkillInfoContainer, SkillInfo>();
                await loader.LoadConfigAsync<StatusInfoContainer, StatusInfo>();
                await loader.LoadConfigAsync<DialogueInfoContainer, DialogueInfo>();
                await loader.LoadConfigAsync<BranchInfoContainer, BranchInfo>();
                await loader.LoadConfigAsync<TaskInfoContainer, TaskInfo>();
                await loader.LoadConfigAsync<TaskConditionInfoContainer, TaskConditionInfo>();
                await loader.LoadConfigAsync<NpcInfoContainer, NpcInfo>();
            
                await loader.LoadConfigAsync<ActivityInfoContainer,ActivityInfo>();
                await loader.LoadConfigAsync<ItemInfoContainer,ItemInfo>();
            });
            
            // 加载二进制配置
            await _binaryDataManager.LoadConfigAsync(AbKeyCollection.Gameconfig);
            LogManager.Log($"配置数据加载成功");
            
            // 读取本地音乐数据
            var MusicData = await _binaryDataManager.LoadAsync<MusicData>(FileUtility.LocalMusicDataFileName);
            LogManager.Log($"本地音乐数据加载成功，{MusicData}");
            
            // 读取本地输入数据
            var InputActionContainer = await _binaryDataManager.LoadAsync<MainActionMapDataContainer>(FileUtility.LocalInputDataFileName);
            LogManager.Log($"本地输入数据加载成功，{InputActionContainer}");

            // 构造主数据集合
            _mainDataCollection = new MainDataCollection
            {
                InputActionContainer = InputActionContainer,
                InputDataContainer = null,
                MusicData = MusicData,
            };
        }

        public async Task SaveDataAsync()
        {
            // 保存音乐数据
            await _binaryDataManager.SaveAsync(FileUtility.LocalMusicDataFileName, _mainDataCollection.MusicData);
            LogManager.Log($"音乐数据保存成功，{_mainDataCollection.MusicData}");
            
            // 保存输入数据
            await _binaryDataManager.SaveAsync(FileUtility.LocalInputDataFileName, _mainDataCollection.InputActionContainer);
            LogManager.Log($"输入数据保存成功，{_mainDataCollection.InputActionContainer}");
        }
        
        public IMainDataCollection GetData()
        {
            return _mainDataCollection;
        }
    }
}
