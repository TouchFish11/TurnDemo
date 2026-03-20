using System;
using UnityEngine;

namespace Core.Mono.YieldInstruction
{
    /// <summary>
    /// 等待条件满或超时
    /// </summary>
    public class WaitUntilOrTimeout : CustomYieldInstruction
    {
        private Func<bool> _condition;
        private float _timeoutTime;
        private bool _hasTimeout;

        public override bool keepWaiting
        {
            get
            {
                //条件满足，停止等待
                if (_condition())
                {
                    return false;
                }

                //超时，停止等待
                if (UnityEngine.Time.time > _timeoutTime)
                {
                    //改变标识
                    _hasTimeout = true;
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// 是否超时
        /// </summary>
        public bool HasTimeout => _hasTimeout;

        public WaitUntilOrTimeout(Func<bool> condition, float timeout)
        {
            _condition = condition;
            _timeoutTime = UnityEngine.Time.time + timeout;
            _hasTimeout = false;
        }
    }
}
