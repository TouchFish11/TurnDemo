using UnityEngine;

namespace Game
{
    public abstract class BaseComponent : MonoBehaviour, IComponent
    {
        public IEntityObject EntityObject { get; private set; }

        protected virtual void Awake()
        {
            EntityObject = this.GetComponent<IEntityObject>();
        }

        public void Init(IEntityObject entityObject)
        {

        }

        public virtual void Destroy()
        {
            EntityObject = null;
        }
    }
}
