using System.Collections.Generic;
using System.Linq;
using Pixelplacement;
using Pixelplacement.TweenSystem;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;
using Event = Spine.Event;

namespace MainScripts.Spine
{
    public class AnimationEventData
    {
        public Event EventData;
        public int TrackIndex;
    }

    public class AnimationCompleteData
    {
        public string StateName;
        public int TrackIndex;
    }
    
    public class SpineAnimationController : MonoBehaviour
    {
        private SkeletonAnimation _animation;
        private Dictionary<int, ActiveAnimationState> _activeAnimationStates;

        private List<AnimationStateInfo> _animationStatesInfo = new();
        
        [HideInInspector] public UnityEvent<AnimationCompleteData> OnAnimationComplete;
        [HideInInspector] public UnityEvent<AnimationEventData> OnAnimationEvent;
        
        [HideInInspector] public UnityEvent OnAnimationChanged;
        [HideInInspector] public UnityEvent OnEnterViewport;
        [HideInInspector] public UnityEvent OnExitViewport;
        
        private MaterialPropertyBlock _materialPropertyBlock;
        private TweenBase _tintTween;
        private MeshRenderer _meshRenderer;

        [SerializeField] private bool _isVisible;
        private bool _isVisibleOld;
        
        private void Awake()
        {
            _materialPropertyBlock = new MaterialPropertyBlock();
            _animation = GetComponent<SkeletonAnimation>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _activeAnimationStates = new Dictionary<int, ActiveAnimationState>();
        }
        
        private void Update()
        {
            if (_isVisible != _isVisibleOld)
            {
                if (!_isVisible)
                    OnBecameInvisible();
                else
                    OnBecameVisible();
            }
            
            _isVisibleOld = _isVisible;
        }

        public void Enable()
        {
            enabled = true;
            _animation.enabled = true;
        }

        public void Disable()
        {
            enabled = false;
            _animation.enabled = false;
        }
        
        private void LateUpdate()
        {
            CheckTransitionsForActiveStates();
        }

        private void CheckTransitionsForActiveStates()
        {
            foreach (var activeState in _activeAnimationStates.Values.ToList())
            {
                var transition = activeState.AnimationStateInfo.Transitions
                    .FirstOrDefault(x => (x.Condition == null || x.Condition()) && !x.OnComplete);
                if (transition != null)
                    PlayAnimation(transition.TargetStateName, activeState.TrackEntry.TrackIndex);
            }
        }

        public TrackEntry PlayAnimation(string stateName, int trackIndex = 0)
        {
            var animationStateInfo = _animationStatesInfo.FirstOrDefault(x => x.Name == stateName);
            if (animationStateInfo == null)
                return null;
            
            if (_activeAnimationStates.TryGetValue(trackIndex, out var activeState))
            {
                activeState.TrackEntry.Complete -= OnTrackEntryComplete;
                activeState.TrackEntry.Event -= OnTrackEntryEvent;
                _activeAnimationStates.Remove(trackIndex);
            }

            var trackEntry = _animation.AnimationState.SetAnimation(trackIndex, animationStateInfo.Name, animationStateInfo.IsLoop);
            
            trackEntry.Complete += OnTrackEntryComplete;
            trackEntry.Event += OnTrackEntryEvent;

            var state = new ActiveAnimationState
            {
                AnimationStateInfo = animationStateInfo,
                TrackEntry = trackEntry
            };
            
            _activeAnimationStates.Add(trackIndex, state);

            OnAnimationChanged?.Invoke();
        
            return trackEntry;
        }

        private void OnTrackEntryComplete(TrackEntry trackEntry)
        {
            if (_activeAnimationStates.TryGetValue(trackEntry.TrackIndex, out var activeState))
            {
                if(!trackEntry.Loop && Mathf.Approximately(trackEntry.AnimationEnd, trackEntry.AnimationTime));
                {
                    OnAnimationComplete?.Invoke(new AnimationCompleteData
                    {
                        StateName = activeState.AnimationStateInfo.Name,
                        TrackIndex = trackEntry.TrackIndex
                    });
                    
                    var transition = activeState.AnimationStateInfo.Transitions
                        .FirstOrDefault(x => (x.Condition == null || x.Condition()) && x.OnComplete);
                    if (transition != null)
                        PlayAnimation(transition.TargetStateName, trackEntry.TrackIndex);
                }
            }
        }

        private void OnTrackEntryEvent(TrackEntry trackEntry, Event e)
        {
            OnAnimationEvent?.Invoke(new AnimationEventData
            {
                EventData = e,
                TrackIndex = trackEntry.TrackIndex
            });
        }
    
        public void ClearAnimationTrack(int trackIndex)
        {
            _animation.AnimationState.SetEmptyAnimation(trackIndex, 0);
            if (_activeAnimationStates.TryGetValue(trackIndex, out var activeState))
            {
                activeState.TrackEntry.Complete -= OnTrackEntryComplete;
                activeState.TrackEntry.Event -= OnTrackEntryEvent;
                _activeAnimationStates.Remove(trackIndex);
            }
        }
        
        public void SetTimeScale(float timeScale)
        {
            _animation.timeScale = timeScale;
        }
    
        public void SetOpacity(float value)
        {
            _animation.skeleton.A = value;
        }

        public void EnemyDamageTint()
        {
            if (_tintTween != null)
                _tintTween.Stop();
            var animationCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
            _materialPropertyBlock.SetColor("_FillColor", Color.white);
            _tintTween = Tween.Value(0, 0.5f, val =>
            {
                _materialPropertyBlock.SetFloat("_FillPhase", val);
                _meshRenderer.SetPropertyBlock(_materialPropertyBlock);
            }, 0.25f, 0, animationCurve);
        }
        
        public AnimationStateInfo CreateAnimationState(string stateName, bool isLoop)
        {
            var animationState = new AnimationStateInfo { Name = stateName, IsLoop = isLoop };
            _animationStatesInfo.Add(animationState);
            return animationState;
        }
        
        //Legacy
        private void OnBecameVisible()
        {
            OnEnterViewport?.Invoke();
        }

        private void OnBecameInvisible()
        {
            OnExitViewport?.Invoke();
        }
    }
}
