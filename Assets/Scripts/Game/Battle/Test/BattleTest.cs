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
        await BinaryDataMgr.Instance.LoadConfig();

        await BattleManager.Instance.StartBattle();
    }
}
