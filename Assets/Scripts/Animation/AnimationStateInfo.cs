using System;
using System.Collections.Generic;

namespace MainScripts.Spine
{
    public class AnimationStateInfo
    {
        public string Name;
        public bool IsLoop;

        public List<AnimationTransition> Transitions = new();

        public AnimationStateInfo AddTransition(string target, bool onComplete, Func<bool> condition)
        {
            Transitions.Add(
                new AnimationTransition 
                {
                    TargetStateName = target, 
                    Condition = condition,
                    OnComplete = onComplete
                }
            );
        
            return this;
        }
        
        //ON COMPLETE WORKS ONLY FOR NON-LOOP ANIMATIONS
        public AnimationStateInfo AddTransitionOnComplete(string target)
        {
            Transitions.Add(
                new AnimationTransition 
                {
                    TargetStateName = target, 
                    Condition = null,
                    OnComplete = true
                }
            );
        
            return this;
        }
    }
}