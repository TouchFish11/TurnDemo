using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(IEntityObject))]
    public abstract class BaseComponent : MonoBehaviour, IComponent
    {
        public IEntityObject EntityObject { get; private set; }

        private void Awake()
        {
            EntityObject = this.GetComponent<IEntityObject>();
        }

        public abstract void Init(IEntityObject entityObject);

        public virtual void Destroy()
        {
            EntityObject = null;
        }
    }
}
