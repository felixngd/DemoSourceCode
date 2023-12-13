﻿using Cysharp.Threading.Tasks;
using UnityScreenNavigator.Runtime.Core.Shared;

namespace UnityScreenNavigator.Runtime.Foundation.Animation
{
    internal static class AnimationExtensions
    {
        // ReSharper disable Unity.PerformanceAnalysis
        public static async UniTask CreatePlayRoutine(this IAnimation self)
        {
#if UI_ANIMATION_TIMELINE_SUPPORT
            if (self is TimelineTransitionAnimationBehaviour)
            {
                var player = new AnimationPlayer(self);
                UpdateDispatcher.Instance.Register(player);
                player.Play();
                await UniTask.WaitUntil(() => player.IsFinished);
                UpdateDispatcher.Instance.Unregister(player);
            }
#else
            await self.Play(default);
#endif
        }
    }
}