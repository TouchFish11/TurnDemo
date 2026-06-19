using UnityEngine;

namespace HotUpdate.Game.VFX
{
    /// <summary>
    /// 等待特效播放结束或主动停止
    /// </summary>
    public class WaitForVFX : CustomYieldInstruction
    {
        private VFXInfo _vfxInfo;

        public override bool keepWaiting => _vfxInfo.IsAlive;
        
        public WaitForVFX(VFXInfo vfxInfo)
        {
            _vfxInfo = vfxInfo;
        }

        public override void Reset()
        {
            _vfxInfo = null;
        }
    }
}
