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
        ServiceLocator.Instance.InitService();
        // 初始化配置
        await ServiceLocator.Instance.Get<IBinaryDataManager>().LoadConfig();
        // 初始化UI管理器
        await ServiceLocator.Instance.Get<IUIManager>().InitUIManagerAsync();
        // 初始化战斗管理器
        ServiceLocator.Instance.Register<IBattleManager>(BattleManager.Instance);
        // 开始战斗
        await ServiceLocator.Instance.Get<IBattleManager>().StartBattle();
    }
}
