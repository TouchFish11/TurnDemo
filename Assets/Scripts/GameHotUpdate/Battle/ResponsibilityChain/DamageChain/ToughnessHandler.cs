using Game.Battle.Damage;
using Game.Battle.Toughness;

namespace GameHotUpdate.Battle.ResponsibilityChain.DamageChain
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
            monster.GetComponent<IToughnessComponent>().ReduceToughness(request.Source, request.ElementType, request.SkillInfo);
            successor.HandleRequest(request);
        }
    }
}
