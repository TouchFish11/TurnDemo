using Game.Battle.Toughness;

namespace Game.Battle.Command
{
    public interface IMonsterActCommand : ICommand
    {
        /// <summary>
        /// 
        /// </summary>
        IToughnessComponent ToughnessComponent { get; }

        void Init(IToughnessComponent toughnessComponent, ISkillCommand skillCommand);
    }
}
