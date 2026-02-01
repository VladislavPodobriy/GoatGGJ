using System.Collections.Generic;
using System.Linq;
using Spine;
using Spine.Unity;
using Spine.Unity.AttachmentTools;
using UnityEngine;

namespace MainScripts.Spine
{
    public class SpineSkinsController : MonoBehaviour
    {
        private SkeletonAnimation _skeleton;
        private Skin _combinedSkin;
        private List<Skin> _currentSkins = new();

        private void Awake()
        {
            _skeleton = GetComponent<SkeletonAnimation>();
        }
        
        public void TryAddSkin(string skinName)
        {
            var skin = _skeleton.Skeleton.Data.FindSkin(skinName);
            _skeleton.Skeleton.SetSkin(skin);
            
            /*var existingSkin = _currentSkins.FirstOrDefault(x => x.Name == skinName);
            if (existingSkin != null)
                return;
            
           
            if (skin != null)
            {
                _currentSkins.Add(skin);
                AssignCombinedSkin();
            }*/
        }
        
        public void RemoveSkin(string skinName)
        {
            var skin = _currentSkins.FirstOrDefault(x => x.Name == skinName);
            if (skin != null)
            {
                _currentSkins.Remove(skin);  
                AssignCombinedSkin();
            }
        }

        private void AssignCombinedSkin()
        {
            _combinedSkin = _combinedSkin ?? new Skin("combined");
            _combinedSkin.Clear();
            foreach (var skin in _currentSkins) { 
                _combinedSkin.AddAttachments(skin);
            }
            _skeleton.Skeleton.SetSkin(_combinedSkin);
            _skeleton.Skeleton.SetToSetupPose();
            var animationStateComponent = _skeleton as IAnimationStateComponent;
            if (animationStateComponent != null) animationStateComponent.AnimationState.Apply(_skeleton.Skeleton);
        }
    }
}