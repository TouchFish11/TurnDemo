namespace GameHotUpdate.Battle.Projectile.Projectiles.Role.Herta
{
    /// <summary>
    /// Herta�սἼ���ܵ�����
    /// </summary>
    public class HertaUltimateSkillProjectile : InstantProjectile
    {
        protected override void OnInit()
        {
            dmgTimes = new float[] { 0.46f, 1.1f, 1.3f, 1.5f, 1.7f, 1.9f, };
            // ������Buff
            base.OnInit();
        }
    }
}
