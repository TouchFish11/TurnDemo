using System;
using UnityEngine;

namespace Test.Other
{
    public class OtherTest : MonoBehaviour
    {
        private class AA
        {
            public AA()
            {
                Debug.Log("AA");
            }
        }
        
        // Start is called before the first frame update
        void Start()
        {
            typeof(AA).GetConstructor(Type.EmptyTypes).Invoke(null);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
