using Core.UI;
using UnityEngine;

namespace GameHotUpdate.UI.General
{
    /// <summary>
    /// ��Ʒ����
    /// </summary>
    public class ItemGrid : BaseUIBehaviour
    {
        public Transform Transform { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            Transform = transform;
        }

        public void Init()
        {

        }
    }
}
