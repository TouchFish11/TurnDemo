using Framework;
using Game;
using Game.Battle;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// ’Ω∂∑≤‚ ‘
/// </summary>
public class BattleTest : MonoBehaviour
{
    // Start is called before the first frame update
    private async void Start()
    {
        await BinaryDataManager.Instance.LoadConfig();

        await UIManager.Instance.InitUIManagerAsync();

        await BattleManager.Instance.StartBattle();
    }
}
