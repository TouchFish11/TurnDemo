using HotUpdate.Battle.Toughness;

namespace HotUpdate.Battle.Command
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
