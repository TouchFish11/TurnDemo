using Framework;
using Game.Battle;
using System.Threading.Tasks;
using UnityEngine;

public class BattleTest : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        await UIManager.Instance.InitUIManagerAsync();
        MainController mainController = await UIManager.Instance.ShowViewAsync<MainView, MainModel, MainController>(E_UILayer.Mid);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
