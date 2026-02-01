namespace GameHotUpdate.Battle.Projectile.Projectiles.Role.FireFly
{
    /// <summary>
    /// FireFly�չ����ܵ�����
    /// </summary>
    public class FireFlyNormalSkillProjectile : InstantProjectile
    {
        protected override void OnInit()
        {
            dmgTimes = new float[] { 0.2f };

            base.OnInit();
        }
    }
}
