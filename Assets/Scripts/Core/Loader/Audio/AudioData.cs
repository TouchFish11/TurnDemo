using UnityEngine;

namespace Core.Loader.Audio
{
    public struct AudioData
    {
        private readonly AudioClip _clip;

        public AudioClip AudioClip
        {
            get
            {
                ++RefCount;
                return _clip;
            }
        }
            
        public int RefCount { get; private set; }

        public AudioData(AudioClip clip)
        {
            _clip =  clip;
            RefCount = 1;
        }
            
        public void Unload()
        {
            --RefCount;
        }
    }
}
