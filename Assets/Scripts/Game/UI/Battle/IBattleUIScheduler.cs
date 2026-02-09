using Core.UI.MVC;
using Game.Battle.Context;
using Game.Battle.Objects;
using UnityEngine;

namespace Game.UI.Battle
{
    public interface IBattleUIScheduler
    {
        GameObject GameObject { get; }    
        
        /// <summary>
        /// �սἼ�ͷ�ʱ
        /// </summary>
        void UltimateCasting();

        IuiController BattleController { get; }
    }
}
