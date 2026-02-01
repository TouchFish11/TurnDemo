namespace GameHotUpdate.Battle.Projectile.Projectiles.Role.FireFly
{
    /// <summary>
    /// FireFly�սἼ������
    /// </summary>
    public class FireFlyUltimateSkillProjectile : InstantProjectile
    {
        protected override void OnInit()
        {
            dmgTimes = new float[] { 0.16f, 0.39f, 0.58f, 0.64f, 0.79f, 0.81f };
            // ������Buff
            base.OnInit();
        }
    }
}
