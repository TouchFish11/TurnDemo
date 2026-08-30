namespace HotUpdate.Game.Battle.StatSystem.Formulas
{
    public interface IStatFormula
    {
        float Calculate(StatEvaluationContext context);
    }
}
