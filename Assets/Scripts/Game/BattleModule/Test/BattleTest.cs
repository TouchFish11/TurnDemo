using GameLogic.BattleMoudule.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        BattleManager.Instance.StartBattle();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
