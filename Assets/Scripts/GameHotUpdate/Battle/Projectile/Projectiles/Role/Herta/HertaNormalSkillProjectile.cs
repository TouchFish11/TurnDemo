namespace GameHotUpdate.Battle.Projectile.Projectiles.Role.Herta
{
    /// <summary>
    /// Herta��ɫ�չ����ܵ�����
    /// </summary>
    public class HertaNormalSkillProjectile : InstantProjectile
    {
        protected override void OnInit()
        {
            dmgTimes = new float[] { 0.29f };  //TODO��������
            // ������Buff
            base.OnInit();
        }
    }
}
