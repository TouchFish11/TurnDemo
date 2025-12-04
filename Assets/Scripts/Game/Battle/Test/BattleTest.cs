using Framework;
using Game.Battle;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class BattleTest : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        await UIManager.Instance.InitUIManagerAsync();
        // 初始化游戏数据
        await GameDataMgr.Instance.InitDataAsync();
        MainController mainController = await UIManager.Instance.ShowViewAsync<MainView, MainModel, MainController>(E_UILayer.Mid);
        FloatingTextManager.Instance.Init();
    }
}
