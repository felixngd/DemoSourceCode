﻿using System;
using System.IO;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityScreenNavigator.Runtime.Core.Modal;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace UnityScreenNavigator.Runtime.Core.Shared
{
    internal sealed class UnityScreenNavigatorSettings : ScriptableObject
    {
        private const string DefaultModalBackdropPrefabKey = "DefaultModalBackdrop";
        private static UnityScreenNavigatorSettings _instance;

        [SerializeField] private TransitionAnimationObject _sheetEnterAnimation;

        [SerializeField] private TransitionAnimationObject _sheetExitAnimation;

        [SerializeField] private TransitionAnimationObject _pagePushEnterAnimation;

        [SerializeField] private TransitionAnimationObject _pagePushExitAnimation;

        [SerializeField] private TransitionAnimationObject _pagePopEnterAnimation;

        [SerializeField] private TransitionAnimationObject _pagePopExitAnimation;

        [SerializeField] private TransitionAnimationObject _modalEnterAnimation;

        [SerializeField] private TransitionAnimationObject _modalExitAnimation;

        [SerializeField] private TransitionAnimationObject _modalBackdropEnterAnimation;

        [SerializeField] private TransitionAnimationObject _modalBackdropExitAnimation;

        [SerializeField] private TransitionAnimationObject _tooltipEnterAnimation;

        [SerializeField] private TransitionAnimationObject _tooltipExitAnimation;

        [SerializeField] private ModalBackdrop _modalBackdropPrefab;


        [SerializeField] private bool _enableInteractionInTransition;

        [SerializeField] private bool useBlocksRaycastsInsteadOfInteractable;


        private ModalBackdrop _defaultModalBackdrop;


        public ITransitionAnimation SheetEnterAnimation => _sheetEnterAnimation != null
            ? Instantiate(_sheetEnterAnimation)
            : SimpleTransitionAnimationObject.CreateInstance(beforeAlpha: 0.0f, easeType: Ease.Linear);

        public ITransitionAnimation SheetExitAnimation => _sheetExitAnimation != null
            ? Instantiate(_sheetExitAnimation)
            : SimpleTransitionAnimationObject.CreateInstance(afterAlpha: 0.0f, easeType: Ease.Linear);

        public ITransitionAnimation PagePushEnterAnimation => _pagePushEnterAnimation != null
            ? Instantiate(_pagePushEnterAnimation)
            : SimpleTransitionAnimationObject.CreateInstance(beforeAlignment: SheetAlignment.Right,
                afterAlignment: SheetAlignment.Center);

        public ITransitionAnimation PagePushExitAnimation => _pagePushExitAnimation != null
            ? Instantiate(_pagePushExitAnimation)
            : SimpleTransitionAnimationObject.CreateInstance(beforeAlignment: SheetAlignment.Center,
                afterAlignment: SheetAlignment.Left);

        public ITransitionAnimation TooltipEnterAnimation => _tooltipEnterAnimation != null
            ? Instantiate(_tooltipEnterAnimation)
            : SimpleTransitionAnimationObject.CreateInstance(beforeScale: Vector3.zero,
                afterScale: Vector3.one);

        public ITransitionAnimation TooltipExitAnimation => _tooltipExitAnimation != null
            ? Instantiate(_tooltipExitAnimation)
            : SimpleTransitionAnimationObject.CreateInstance(beforeScale: Vector3.one,
                afterScale: Vector3.zero);


        public ITransitionAnimation PagePopEnterAnimation => _pagePopEnterAnimation != null
            ? Instantiate(_pagePopEnterAnimation)
            : SimpleTransitionAnimationObject.CreateInstance(beforeAlignment: SheetAlignment.Left,
                afterAlignment: SheetAlignment.Center);

        public ITransitionAnimation PagePopExitAnimation => _pagePopExitAnimation != null
            ? Instantiate(_pagePopExitAnimation)
            : SimpleTransitionAnimationObject.CreateInstance(beforeAlignment: SheetAlignment.Center,
                afterAlignment: SheetAlignment.Right);

        public ITransitionAnimation ModalEnterAnimation => _modalEnterAnimation != null
            ? Instantiate(_modalEnterAnimation)
            : SimpleTransitionAnimationObject.CreateInstance(beforeScale: Vector3.one * 0.3f, beforeAlpha: 0.0f);

        public ITransitionAnimation ModalExitAnimation => _modalExitAnimation != null
            ? Instantiate(_modalExitAnimation)
            : SimpleTransitionAnimationObject.CreateInstance(afterScale: Vector3.one * 0.3f, afterAlpha: 0.0f);

        public ITransitionAnimation ModalBackdropEnterAnimation => _modalBackdropEnterAnimation != null
            ? Instantiate(_modalBackdropEnterAnimation)
            : SimpleTransitionAnimationObject.CreateInstance(beforeAlpha: 0.0f, easeType: Ease.Linear);

        public ITransitionAnimation ModalBackdropExitAnimation => _modalBackdropExitAnimation != null
            ? Instantiate(_modalBackdropExitAnimation)
            : SimpleTransitionAnimationObject.CreateInstance(afterAlpha: 0.0f, easeType: Ease.Linear);

        public ModalBackdrop ModalBackdropPrefab
        {
            get
            {
                if (_modalBackdropPrefab != null)
                {
                    return _modalBackdropPrefab;
                }

                if (_defaultModalBackdrop == null)
                {
                    _defaultModalBackdrop = Resources.Load<ModalBackdrop>(DefaultModalBackdropPrefabKey);
                }

                return _defaultModalBackdrop;
            }
        }

        public bool EnableInteractionInTransition => _enableInteractionInTransition;

        public bool UseBlocksRaycastsInsteadOfInteractable => useBlocksRaycastsInsteadOfInteractable;

        public static UnityScreenNavigatorSettings Instance
        {
            get
            {
#if UNITY_EDITOR
                if (_instance == null)
                {
                    //find scriptable object asset of type UnityScreenNavigatorSettings
                    var guids = AssetDatabase.FindAssets($"t:{nameof(UnityScreenNavigatorSettings)}");
                    if (guids.Length == 0)
                    {
                        Debug.LogError($"Could not find {nameof(UnityScreenNavigatorSettings)} asset");
                    }
                    else
                    {
                        //load scriptable object asset
                        var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                        _instance = AssetDatabase.LoadAssetAtPath<UnityScreenNavigatorSettings>(path);
                    }
                }

                return _instance;

#else
                if (_instance == null)
                {
                    var settings = Resources.Load<UnityScreenNavigatorSettings>("UnityScreenNavigatorSettings");
                    _instance = settings;
                }

                return _instance;
#endif
            }
            private set => _instance = value;
        }

        private void OnEnable()
        {
            _instance = this;
        }

        public ITransitionAnimation GetDefaultScreenTransitionAnimation(bool push, bool enter)
        {
            if (push)
            {
                return enter ? PagePushEnterAnimation : PagePushExitAnimation;
            }

            return enter ? PagePopEnterAnimation : PagePopExitAnimation;
        }

        public ITransitionAnimation GetDefaultModalTransitionAnimation(bool enter)
        {
            return enter ? ModalEnterAnimation : ModalExitAnimation;
        }

        public ITransitionAnimation GetDefaultSheetTransitionAnimation(bool enter)
        {
            return enter ? SheetEnterAnimation : SheetExitAnimation;
        }


#if UNITY_EDITOR

        [MenuItem("Assets/Create/Screen Navigator Settings", priority = -1)]
        private static void Create()
        {
            var asset = PlayerSettings.GetPreloadedAssets().OfType<UnityScreenNavigatorSettings>().FirstOrDefault();
            if (asset != null)
            {
                var path = AssetDatabase.GetAssetPath(asset);
                throw new InvalidOperationException($"{nameof(UnityScreenNavigatorSettings)} already exists at {path}");
            }

            var assetPath = EditorUtility.SaveFilePanelInProject($"Save {nameof(UnityScreenNavigatorSettings)}",
                nameof(UnityScreenNavigatorSettings),
                "asset", "", "Assets");

            if (string.IsNullOrEmpty(assetPath))
            {
                // Return if canceled.
                return;
            }

            // Create folders if needed.
            var folderPath = Path.GetDirectoryName(assetPath);
            if (!string.IsNullOrEmpty(folderPath) && !Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var instance = CreateInstance<UnityScreenNavigatorSettings>();
            AssetDatabase.CreateAsset(instance, assetPath);
            var preloadedAssets = PlayerSettings.GetPreloadedAssets().ToList();
            preloadedAssets.Add(instance);
            PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
            AssetDatabase.SaveAssets();
        }

        private void OnValidate()
        {
            _instance = this;
        }
#endif
    }
}