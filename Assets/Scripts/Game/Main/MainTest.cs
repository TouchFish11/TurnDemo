using Framework;
using Game;
using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 测试主入口
/// </summary>
public class MainTest : SingletonMono<MainTest>
{
    private async void Start()
    {
        // 初始化服务定位器
        ServiceLocator.InitService();
        // 初始化工厂
        ServiceLocator.Get<IFactoryManager>().InitFactorys();
        // 初始化动态游戏数据
        await ServiceLocator.Get<IGameDataManager>().InitDataAsync();
        // 初始化UI管理器
        await ServiceLocator.Get<IUIManager>().InitUIManagerAsync();
        InitScene();
    }

    public async void InitScene()
    {
        // 测试创建Npc、玩家
        NpcObject villager = await ObjectBuilder.GetOrCreateInstance<NpcObject>(E_AssetBundleType.Prefab, ResKeyCollection.Prefab_Npc, new Vector3(0, 1, 8.39f), Quaternion.identity);
        villager.BaseInit(1);

        NpcObject Vagrant = await ObjectBuilder.GetOrCreateInstance<NpcObject>(E_AssetBundleType.Prefab, ResKeyCollection.Prefab_Npc, new Vector3(6.94f, 1, 8.39f), Quaternion.identity);
        Vagrant.BaseInit(2);

        // 创建玩家用户
        await ServiceLocator.Get<IPlayerManager>().CreatePlayer(1001);

        MainController mainController = await ServiceLocator.Get<IUIManager>().CreateViewAsync<MainView, MainModel, MainController>(E_UILayer.Mid);
        ServiceLocator.Get<IFloatingTextManager>().Init();
    }

    /// <summary>
    /// 清理场景
    /// </summary>
    public void ClearScene()
    {
        // 清理对象
        ServiceLocator.Get<IPlayerManager>().Clear();
        // 清理浮动文本管理器
        ServiceLocator.Get<IFloatingTextManager>().ClearCache();
        ServiceLocator.Get<IPoolManager>().Clear();
    }
}
