﻿using System.Collections.Generic;
using UnityEngine;
using UnityScreenNavigator.Runtime.Core.Shared;
using UnityScreenNavigator.Runtime.Core.Shared.Views;
using UnityScreenNavigator.Runtime.Foundation;
using UnityScreenNavigator.Runtime.Foundation.Animation;
using UnityScreenNavigator.Runtime.Foundation.PriorityCollection;
using Cysharp.Threading.Tasks;

namespace UnityScreenNavigator.Runtime.Core.Modal
{
    [DisallowMultipleComponent]
    public class Modal : Window, IModalLifecycleEvent
    {
        [SerializeField] protected bool _isFillRectWhenExit = true;
        [SerializeField] private bool _usePrefabNameAsIdentifier = true;

        [SerializeField] [EnabledIf(nameof(_usePrefabNameAsIdentifier), false)]
        private string _identifier;

        [SerializeField]
        private ModalTransitionAnimationContainer _animationContainer = new ModalTransitionAnimationContainer();

        private readonly PriorityList<IModalLifecycleEvent> _lifecycleEvents = new PriorityList<IModalLifecycleEvent>();

        public override string Identifier
        {
            get => _identifier;
            set => _identifier = value;
        }

        public AsyncReactiveProperty<LifeCycleEnum> LifeCycle = new(default);

        public ModalTransitionAnimationContainer AnimationContainer => _animationContainer;
        
        public virtual UniTask Initialize()
        {
            LifeCycle.Value = LifeCycleEnum.INIT;
            return UniTask.CompletedTask;
        }

        public virtual UniTask WillPushEnter()
        {
            LifeCycle.Value = LifeCycleEnum.WILL_PUSH_ENTER;
            return UniTask.CompletedTask;
        }

        public virtual void DidPushEnter()
        {
            LifeCycle.Value = LifeCycleEnum.DID_PUSH_ENTER;
        }

        public virtual UniTask WillPushExit()
        {
            LifeCycle.Value = LifeCycleEnum.WILL_PUSH_EXIT;
            return UniTask.CompletedTask;
        }

        public virtual void DidPushExit()
        {
            LifeCycle.Value = LifeCycleEnum.DID_PUSH_EXIT;
        }

        public virtual UniTask WillPopEnter()
        {
            LifeCycle.Value = LifeCycleEnum.WILL_POP_ENTER;
            return UniTask.CompletedTask;
        }


        public virtual void DidPopEnter()
        {
            LifeCycle.Value = LifeCycleEnum.DID_POP_ENTER;
        }

        public virtual UniTask WillPopExit()
        {
            LifeCycle.Value = LifeCycleEnum.WILL_POP_EXIT;
            return UniTask.CompletedTask;
        }


        public virtual void DidPopExit()
        {
            LifeCycle.Value = LifeCycleEnum.DID_POP_EXIT;
        }
        // ReSharper disable Unity.PerformanceAnalysis
        public virtual UniTask Cleanup()
        {
            LifeCycle.Value = LifeCycleEnum.CLEANUP;
            return UniTask.CompletedTask;
        }

        public void AddLifecycleEvent(IModalLifecycleEvent lifecycleEvent, int priority = 0)
        {
            _lifecycleEvents.Add(lifecycleEvent, priority);
        }

        public void RemoveLifecycleEvent(IModalLifecycleEvent lifecycleEvent)
        {
            _lifecycleEvents.Remove(lifecycleEvent);
        }

        internal UniTask AfterLoad(RectTransform parentTransform)
        {
            _lifecycleEvents.Add(this, 0);
            _identifier = _usePrefabNameAsIdentifier ? gameObject.name.Replace("(Clone)", string.Empty) : _identifier;
            Parent = parentTransform;
            RectTransform.FillParent((RectTransform) Parent);

            Alpha = 0.0f;

            var tasks = _lifecycleEvents.Select(x => x.Initialize());
            return UniTask.WhenAll(tasks);
        }

        internal UniTask BeforeEnter(bool push, Modal partnerModal)
        {
            return BeforeEnterTask(push, partnerModal);
        }

        private UniTask BeforeEnterTask(bool push, Modal partnerModal)
        {
            if (push)
            {
                gameObject.SetActive(true);
                RectTransform.FillParent((RectTransform) Parent);

                Alpha = 0.0f;
            }

            if (!UnityScreenNavigatorSettings.Instance.EnableInteractionInTransition)
            {
                Interactable = false;
            }

            var routines = push
                ? _lifecycleEvents.Select(x => x.WillPushEnter())
                : _lifecycleEvents.Select(x => x.WillPopEnter());
            return UniTask.WhenAll(routines);
        }

        internal UniTask Enter(bool push, bool playAnimation, Modal partnerModal)
        {
            return EnterTask(push, playAnimation, partnerModal);
        }

        private UniTask EnterTask(bool push, bool playAnimation, Modal partnerModal)
        {
            if (push)
            {
                Alpha = 1.0f;

                if (playAnimation)
                {
                    var anim = _animationContainer.GetAnimation(true, partnerModal?.Identifier);
                    if (anim == null)
                    {
                        anim = UnityScreenNavigatorSettings.Instance.GetDefaultModalTransitionAnimation(true);
                    }

                    anim.SetPartner(partnerModal?.transform as RectTransform);
                    anim.Setup(RectTransform);
                    return anim.CreatePlayRoutine();
                }

                RectTransform.FillParent((RectTransform) Parent);
                return UniTask.CompletedTask;
            }

            return UniTask.CompletedTask;
        }

        internal void AfterEnter(bool push, Modal partnerModal)
        {
            if (push)
            {
                foreach (var lifecycleEvent in _lifecycleEvents)
                {
                    lifecycleEvent.DidPushEnter();
                }
            }
            else
            {
                foreach (var lifecycleEvent in _lifecycleEvents)
                {
                    lifecycleEvent.DidPopEnter();
                }
            }

            if (!UnityScreenNavigatorSettings.Instance.EnableInteractionInTransition)
            {
                Interactable = true;
            }

        //    DoneEnter.TrySetResult();
        }


        internal UniTask BeforeExit(bool push, Modal partnerModal)
        {
            return BeforeExitTask(push, partnerModal);
        }
        // ReSharper disable Unity.PerformanceAnalysis
        private UniTask BeforeExitTask(bool push, Modal partnerModal)
        {
            if (!push)
            {
                gameObject.SetActive(true);
                if (_isFillRectWhenExit)
                {
                    RectTransform.FillParent((RectTransform)Parent);
                }
                Alpha = 1.0f;
            }

            if (!UnityScreenNavigatorSettings.Instance.EnableInteractionInTransition)
            {
                Interactable = false;
            }

            var tasks = push
                ? _lifecycleEvents.Select(x => x.WillPushExit())
                : _lifecycleEvents.Select(x => x.WillPopExit());
            return UniTask.WhenAll(tasks);
        }
        
        internal UniTask Exit(bool push, bool playAnimation, Modal partnerModal)
        {
            return ExitTask(push, playAnimation, partnerModal);
        }
        // ReSharper disable Unity.PerformanceAnalysis
        private UniTask ExitTask(bool push, bool playAnimation, Modal partnerModal)
        {
            if (!push)
            {
                if (playAnimation)
                {
                    var anim = _animationContainer.GetAnimation(false, partnerModal?._identifier);
                    if (anim == null)
                    {
                        anim = UnityScreenNavigatorSettings.Instance.GetDefaultModalTransitionAnimation(false);
                    }

                    anim.SetPartner(partnerModal?.transform as RectTransform);
                    anim.Setup(RectTransform);
                    return anim.CreatePlayRoutine();
                }

                Alpha = 0.0f;
            }

            return UniTask.CompletedTask;
        }

        internal void AfterExit(bool push, Modal partnerModal)
        {
            //
            if (push)
            {
                foreach (var lifecycleEvent in _lifecycleEvents)
                {
                    lifecycleEvent.DidPushExit();
                }
            }
            else
            {
                foreach (var lifecycleEvent in _lifecycleEvents)
                {
                    lifecycleEvent.DidPopExit();
                }
            }
        }

        internal UniTask BeforeRelease()
        {
            var tasks = new List<UniTask>();
            foreach (var lifecycleEvent in _lifecycleEvents)
            {
                tasks.Add(lifecycleEvent.Cleanup());
            }
            return UniTask.WhenAll(tasks);
        }
    }
}