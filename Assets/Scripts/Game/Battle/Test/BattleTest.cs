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
    // Start is called before the first frame update
    private async void Start()
    {
        // 初始化服务定位器
        ServiceLocator.Instance.InitService();
        // 初始化配置
        await ServiceLocator.Instance.Get<IBinaryDataManager>().LoadConfig();
        // 初始化UI管理器
        await ServiceLocator.Instance.Get<IUIManager>().InitUIManagerAsync();
        // 初始化战斗相关管理器
        InitBattle();
        // 开始战斗
        await ServiceLocator.Instance.Get<IBattleManager>().StartBattle();
    }

    private void InitBattle()
    {
        ServiceLocator.Instance.Register<IBattleManager>(BattleManager.Instance);
        ServiceLocator.Instance.Register<IDamageCalcManager>(DamageCalcManager.Instance);
        ServiceLocator.Instance.Register<ISkillManager>(SkillManager.Instance);
        ServiceLocator.Instance.Register<ITargetSelectManager>(TargetSelectManager.Instance);
    }
}
