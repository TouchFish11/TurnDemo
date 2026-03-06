using HotUpdate.Core.Battle.Toughness;

namespace HotUpdate.Core.Battle.Command
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
