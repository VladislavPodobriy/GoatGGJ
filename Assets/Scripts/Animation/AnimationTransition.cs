using System;

namespace MainScripts.Spine
{
    public class AnimationTransition
    {
        public string TargetStateName;
        public bool OnComplete;
        public Func<bool> Condition;
    }
}