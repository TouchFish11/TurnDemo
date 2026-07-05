using HotUpdate.Game.Battle.Damage;
using HotUpdate.Game.Battle.Toughness;

namespace HotUpdate.Game.Battle.ResponsibilityChain.DamageChain
{
    /// <summary>
    /// 韧性处理器
    /// </summary>
    public class ToughnessHandler : Handler<DamageResult>
    {
        public override void HandleRequest(DamageResult request)
        {
            // 获取韧性组件，根据伤害信息扣除对应韧性
            var monster = request.Target;
            monster.GetComponent<ToughnessComponent>().ReduceToughness(request.Source, request.ElementType, request.ResilienceValue, request.SkillId);
            successor.HandleRequest(request);
        }
    }
}
