using HotUpdate.Base.Component;
using HotUpdate.Game.Inputs;

namespace HotUpdate.Game
{
    public class Test
    {
        private class Father
        {
        
        }
    
        private class Son : Father
        {
        
        }

        public interface IFather<out T> where T : IComponent
        {
            
        }

        private void TestFun()
        {

        }
    }
}
