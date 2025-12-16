using Framework;
using Game;
using Game.Battle;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class BattleTest : MonoBehaviour
{
    // Start is called before the first frame update
    private async void Start()
    {
        await UIManager.Instance.InitUIManagerAsync();
        // 初始化游戏数据
        await GameDataMgr.Instance.InitDataAsync();

        // 测试创建Npc、玩家
        NpcObject villager = await ObjectBuilder.GetOrCreateInstance<NpcObject>(E_AssetBundleType.Prefab, ResConfigCollection.Npc, new Vector3(0,1,8.39f), Quaternion.identity);
        villager.BaseInit(1);

        NpcObject Vagrant = await ObjectBuilder.GetOrCreateInstance<NpcObject>(E_AssetBundleType.Prefab, ResConfigCollection.Npc, new Vector3(6.94f, 1, 8.39f), Quaternion.identity);
        Vagrant.BaseInit(2);

        FireFly fireFly = await ObjectBuilder.GetOrCreateInstance<FireFly>(E_AssetBundleType.Prefab, ResConfigCollection.Dog, new Vector3(0, 0, -5.6f), Quaternion.identity);
        fireFly.BaseInit(-1);

        MainController mainController = await UIManager.Instance.CreateViewAsync<MainView, MainModel, MainController>(E_UILayer.Mid);
        FloatingTextManager.Instance.Init();
    }
}
