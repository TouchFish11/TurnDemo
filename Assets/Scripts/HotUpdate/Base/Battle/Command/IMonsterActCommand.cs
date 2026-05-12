using HotUpdate.Base.Battle.Toughness;

namespace HotUpdate.Base.Battle.Command
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
