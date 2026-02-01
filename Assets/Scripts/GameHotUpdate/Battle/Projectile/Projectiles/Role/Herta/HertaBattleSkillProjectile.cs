namespace GameHotUpdate.Battle.Projectile.Projectiles.Role.Herta
{
    /// <summary>
    /// Hertaս�����ܵ�����
    /// </summary>
    public class HertaBattleSkillProjectile : InstantProjectile
    {
        protected override void OnInit()
        {
            dmgTimes = new float[] { 1.52f };
            // ������Buff
            base.OnInit();
        }
    }
}
