using Framework;
using Game;
using Game.Battle;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 战斗测试
/// </summary>
public class BattleTest : MonoBehaviour
{
    [SerializeField] private new Light light;

    // Start is called before the first frame update
    private async void Start()
    {
        light.transform.rotation = Quaternion.Euler(-5, -30, 0);

        // 初始化服务定位器
        ServiceLocator.InitService();

        // 初始化工厂
        ServiceLocator.Get<IFactoryManager>().InitFactorys();
        // 初始化配置
        await ServiceLocator.Get<IBinaryDataManager>().LoadConfig();
        // 初始化UI管理器
        await ServiceLocator.Get<IUIManager>().InitUIManagerAsync();
        // 初始化战斗管理器
        ServiceLocator.Register<IBattleManager>(BattleManager.Instance);
        // 开始战斗
        await ServiceLocator.Get<IBattleManager>().StartBattle();
    }
}
