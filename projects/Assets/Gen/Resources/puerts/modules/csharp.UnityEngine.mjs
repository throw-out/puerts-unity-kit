
const csharp = (function () {
    let _g = this || global || globalThis;
    return _g['CS'] || _g['csharp'] || require('csharp');
})();


function __proxy__(getter) {
    let target;
    function tryload() {
        if (!getter) return;
        target = getter();
        getter = null;
    };
    return new Proxy(tryload, {
        apply: function (_, thisArg, argArray) {
            tryload();
            target.apply(thisArg, argArray);
        },
        construct: function (_, argArray, newTarget) {
            tryload();
            return new target(...argArray);
        },
        get: function (_, property) {
            tryload();
            return target[property];
        },
        set: function (_, property, newValue) {
            tryload();
            target[property] = newValue;
            return true;
        },
        defineProperty: function (_, property, attributes) {
            tryload();
            Object.defineProperty(target, property, attributes);
            return true;
        },
        deleteProperty: function (_, property) {
            tryload();
            delete target[property];
            return true;
        },
        getOwnPropertyDescriptor: function (_, property) {
            tryload();
            return Object.getOwnPropertyDescriptor(target, property);
        },
        getPrototypeOf: function (_) {
            tryload();
            return Object.getPrototypeOf(target);
        },
        setPrototypeOf: function (_, newValue) {
            tryload();
            Object.setPrototypeOf(target, newValue);
            return true;
        },
        has: function (_, property) {
            tryload();
            return property in target;
        },
        isExtensible: function (_) {
            tryload();
            return Object.isExtensible(target);
        },
        ownKeys: function (_) {
            tryload();
            return Reflect.ownKeys(target)?.filter(key => Object.getOwnPropertyDescriptor(target, key)?.configurable);
        },
        preventExtensions: function (_) {
            tryload();
            Object.preventExtensions(target);
            return true;
        },

    });
}


export default csharp.UnityEngine;

//导出名称为Object的类可能与全局域中的Object冲突, 此处生成别名在末尾再一次性导出

const $AndroidJavaRunnable = __proxy__(() => csharp.UnityEngine.AndroidJavaRunnable);
const $AndroidJavaException = __proxy__(() => csharp.UnityEngine.AndroidJavaException);
const $AndroidJavaObject = __proxy__(() => csharp.UnityEngine.AndroidJavaObject);
const $AndroidJavaClass = __proxy__(() => csharp.UnityEngine.AndroidJavaClass);
const $AndroidJavaProxy = __proxy__(() => csharp.UnityEngine.AndroidJavaProxy);
const $jvalue = __proxy__(() => csharp.UnityEngine.jvalue);
const $AndroidJNIHelper = __proxy__(() => csharp.UnityEngine.AndroidJNIHelper);
const $AndroidJNI = __proxy__(() => csharp.UnityEngine.AndroidJNI);
const $Object = __proxy__(() => csharp.UnityEngine.Object);
const $Component = __proxy__(() => csharp.UnityEngine.Component);
const $Behaviour = __proxy__(() => csharp.UnityEngine.Behaviour);
const $Animator = __proxy__(() => csharp.UnityEngine.Animator);
const $AnimationInfo = __proxy__(() => csharp.UnityEngine.AnimationInfo);
const $Vector3 = __proxy__(() => csharp.UnityEngine.Vector3);
const $Quaternion = __proxy__(() => csharp.UnityEngine.Quaternion);
const $AnimatorUpdateMode = __proxy__(() => csharp.UnityEngine.AnimatorUpdateMode);
const $AvatarIKGoal = __proxy__(() => csharp.UnityEngine.AvatarIKGoal);
const $AvatarIKHint = __proxy__(() => csharp.UnityEngine.AvatarIKHint);
const $HumanBodyBones = __proxy__(() => csharp.UnityEngine.HumanBodyBones);
const $ScriptableObject = __proxy__(() => csharp.UnityEngine.ScriptableObject);
const $StateMachineBehaviour = __proxy__(() => csharp.UnityEngine.StateMachineBehaviour);
const $AnimatorStateInfo = __proxy__(() => csharp.UnityEngine.AnimatorStateInfo);
const $AnimatorTransitionInfo = __proxy__(() => csharp.UnityEngine.AnimatorTransitionInfo);
const $AnimatorClipInfo = __proxy__(() => csharp.UnityEngine.AnimatorClipInfo);
const $AnimatorControllerParameter = __proxy__(() => csharp.UnityEngine.AnimatorControllerParameter);
const $AvatarTarget = __proxy__(() => csharp.UnityEngine.AvatarTarget);
const $MatchTargetWeightMask = __proxy__(() => csharp.UnityEngine.MatchTargetWeightMask);
const $Transform = __proxy__(() => csharp.UnityEngine.Transform);
const $AnimatorCullingMode = __proxy__(() => csharp.UnityEngine.AnimatorCullingMode);
const $AnimatorRecorderMode = __proxy__(() => csharp.UnityEngine.AnimatorRecorderMode);
const $RuntimeAnimatorController = __proxy__(() => csharp.UnityEngine.RuntimeAnimatorController);
const $Avatar = __proxy__(() => csharp.UnityEngine.Avatar);
const $IAnimationClipSource = __proxy__(() => csharp.UnityEngine.IAnimationClipSource);
const $Motion = __proxy__(() => csharp.UnityEngine.Motion);
const $AnimationClip = __proxy__(() => csharp.UnityEngine.AnimationClip);
const $SharedBetweenAnimatorsAttribute = __proxy__(() => csharp.UnityEngine.SharedBetweenAnimatorsAttribute);
const $GameObject = __proxy__(() => csharp.UnityEngine.GameObject);
const $AnimationCurve = __proxy__(() => csharp.UnityEngine.AnimationCurve);
const $WrapMode = __proxy__(() => csharp.UnityEngine.WrapMode);
const $Bounds = __proxy__(() => csharp.UnityEngine.Bounds);
const $AnimationEvent = __proxy__(() => csharp.UnityEngine.AnimationEvent);
const $AnimatorControllerParameterType = __proxy__(() => csharp.UnityEngine.AnimatorControllerParameterType);
const $DurationUnit = __proxy__(() => csharp.UnityEngine.DurationUnit);
const $AnimatorOverrideController = __proxy__(() => csharp.UnityEngine.AnimatorOverrideController);
const $AnimationClipPair = __proxy__(() => csharp.UnityEngine.AnimationClipPair);
const $AnimatorUtility = __proxy__(() => csharp.UnityEngine.AnimatorUtility);
const $BodyDof = __proxy__(() => csharp.UnityEngine.BodyDof);
const $HeadDof = __proxy__(() => csharp.UnityEngine.HeadDof);
const $LegDof = __proxy__(() => csharp.UnityEngine.LegDof);
const $ArmDof = __proxy__(() => csharp.UnityEngine.ArmDof);
const $FingerDof = __proxy__(() => csharp.UnityEngine.FingerDof);
const $HumanPartDof = __proxy__(() => csharp.UnityEngine.HumanPartDof);
const $HumanDescription = __proxy__(() => csharp.UnityEngine.HumanDescription);
const $SkeletonBone = __proxy__(() => csharp.UnityEngine.SkeletonBone);
const $HumanLimit = __proxy__(() => csharp.UnityEngine.HumanLimit);
const $HumanBone = __proxy__(() => csharp.UnityEngine.HumanBone);
const $AvatarBuilder = __proxy__(() => csharp.UnityEngine.AvatarBuilder);
const $AvatarMaskBodyPart = __proxy__(() => csharp.UnityEngine.AvatarMaskBodyPart);
const $AvatarMask = __proxy__(() => csharp.UnityEngine.AvatarMask);
const $HumanPose = __proxy__(() => csharp.UnityEngine.HumanPose);
const $HumanPoseHandler = __proxy__(() => csharp.UnityEngine.HumanPoseHandler);
const $HumanTrait = __proxy__(() => csharp.UnityEngine.HumanTrait);
const $SendMessageOptions = __proxy__(() => csharp.UnityEngine.SendMessageOptions);
const $TrackedReference = __proxy__(() => csharp.UnityEngine.TrackedReference);
const $AnimationState = __proxy__(() => csharp.UnityEngine.AnimationState);
const $PlayMode = __proxy__(() => csharp.UnityEngine.PlayMode);
const $QueueMode = __proxy__(() => csharp.UnityEngine.QueueMode);
const $AnimationBlendMode = __proxy__(() => csharp.UnityEngine.AnimationBlendMode);
const $AnimationPlayMode = __proxy__(() => csharp.UnityEngine.AnimationPlayMode);
const $AnimationCullingType = __proxy__(() => csharp.UnityEngine.AnimationCullingType);
const $Animation = __proxy__(() => csharp.UnityEngine.Animation);
const $AssetBundleLoadResult = __proxy__(() => csharp.UnityEngine.AssetBundleLoadResult);
const $AssetBundle = __proxy__(() => csharp.UnityEngine.AssetBundle);
const $YieldInstruction = __proxy__(() => csharp.UnityEngine.YieldInstruction);
const $AsyncOperation = __proxy__(() => csharp.UnityEngine.AsyncOperation);
const $AssetBundleCreateRequest = __proxy__(() => csharp.UnityEngine.AssetBundleCreateRequest);
const $AssetBundleRequest = __proxy__(() => csharp.UnityEngine.AssetBundleRequest);
const $AssetBundleRecompressOperation = __proxy__(() => csharp.UnityEngine.AssetBundleRecompressOperation);
const $BuildCompression = __proxy__(() => csharp.UnityEngine.BuildCompression);
const $ThreadPriority = __proxy__(() => csharp.UnityEngine.ThreadPriority);
const $AssetBundleManifest = __proxy__(() => csharp.UnityEngine.AssetBundleManifest);
const $Hash128 = __proxy__(() => csharp.UnityEngine.Hash128);
const $CompressionType = __proxy__(() => csharp.UnityEngine.CompressionType);
const $CompressionLevel = __proxy__(() => csharp.UnityEngine.CompressionLevel);
const $AudioSettings = __proxy__(() => csharp.UnityEngine.AudioSettings);
const $AudioSpeakerMode = __proxy__(() => csharp.UnityEngine.AudioSpeakerMode);
const $AudioConfiguration = __proxy__(() => csharp.UnityEngine.AudioConfiguration);
const $AudioBehaviour = __proxy__(() => csharp.UnityEngine.AudioBehaviour);
const $AudioSource = __proxy__(() => csharp.UnityEngine.AudioSource);
const $AudioClip = __proxy__(() => csharp.UnityEngine.AudioClip);
const $AudioVelocityUpdateMode = __proxy__(() => csharp.UnityEngine.AudioVelocityUpdateMode);
const $AudioSourceCurveType = __proxy__(() => csharp.UnityEngine.AudioSourceCurveType);
const $AudioRolloffMode = __proxy__(() => csharp.UnityEngine.AudioRolloffMode);
const $FFTWindow = __proxy__(() => csharp.UnityEngine.FFTWindow);
const $AudioLowPassFilter = __proxy__(() => csharp.UnityEngine.AudioLowPassFilter);
const $AudioHighPassFilter = __proxy__(() => csharp.UnityEngine.AudioHighPassFilter);
const $AudioReverbFilter = __proxy__(() => csharp.UnityEngine.AudioReverbFilter);
const $AudioReverbPreset = __proxy__(() => csharp.UnityEngine.AudioReverbPreset);
const $AudioDataLoadState = __proxy__(() => csharp.UnityEngine.AudioDataLoadState);
const $AudioCompressionFormat = __proxy__(() => csharp.UnityEngine.AudioCompressionFormat);
const $AudioClipLoadType = __proxy__(() => csharp.UnityEngine.AudioClipLoadType);
const $AudioListener = __proxy__(() => csharp.UnityEngine.AudioListener);
const $AudioReverbZone = __proxy__(() => csharp.UnityEngine.AudioReverbZone);
const $AudioDistortionFilter = __proxy__(() => csharp.UnityEngine.AudioDistortionFilter);
const $AudioEchoFilter = __proxy__(() => csharp.UnityEngine.AudioEchoFilter);
const $AudioChorusFilter = __proxy__(() => csharp.UnityEngine.AudioChorusFilter);
const $AudioRenderer = __proxy__(() => csharp.UnityEngine.AudioRenderer);
const $WebCamFlags = __proxy__(() => csharp.UnityEngine.WebCamFlags);
const $WebCamKind = __proxy__(() => csharp.UnityEngine.WebCamKind);
const $WebCamDevice = __proxy__(() => csharp.UnityEngine.WebCamDevice);
const $Resolution = __proxy__(() => csharp.UnityEngine.Resolution);
const $Texture = __proxy__(() => csharp.UnityEngine.Texture);
const $WebCamTexture = __proxy__(() => csharp.UnityEngine.WebCamTexture);
const $Vector2 = __proxy__(() => csharp.UnityEngine.Vector2);
const $Color = __proxy__(() => csharp.UnityEngine.Color);
const $Color32 = __proxy__(() => csharp.UnityEngine.Color32);
const $ClothSphereColliderPair = __proxy__(() => csharp.UnityEngine.ClothSphereColliderPair);
const $Collider = __proxy__(() => csharp.UnityEngine.Collider);
const $SphereCollider = __proxy__(() => csharp.UnityEngine.SphereCollider);
const $ClothSkinningCoefficient = __proxy__(() => csharp.UnityEngine.ClothSkinningCoefficient);
const $Cloth = __proxy__(() => csharp.UnityEngine.Cloth);
const $CapsuleCollider = __proxy__(() => csharp.UnityEngine.CapsuleCollider);
const $ClusterInputType = __proxy__(() => csharp.UnityEngine.ClusterInputType);
const $PrimitiveType = __proxy__(() => csharp.UnityEngine.PrimitiveType);
const $Space = __proxy__(() => csharp.UnityEngine.Space);
const $RuntimePlatform = __proxy__(() => csharp.UnityEngine.RuntimePlatform);
const $SystemLanguage = __proxy__(() => csharp.UnityEngine.SystemLanguage);
const $LogType = __proxy__(() => csharp.UnityEngine.LogType);
const $LogOption = __proxy__(() => csharp.UnityEngine.LogOption);
const $SortingLayer = __proxy__(() => csharp.UnityEngine.SortingLayer);
const $WeightedMode = __proxy__(() => csharp.UnityEngine.WeightedMode);
const $Keyframe = __proxy__(() => csharp.UnityEngine.Keyframe);
const $Application = __proxy__(() => csharp.UnityEngine.Application);
const $ApplicationInstallMode = __proxy__(() => csharp.UnityEngine.ApplicationInstallMode);
const $ApplicationSandboxType = __proxy__(() => csharp.UnityEngine.ApplicationSandboxType);
const $StackTraceLogType = __proxy__(() => csharp.UnityEngine.StackTraceLogType);
const $UserAuthorization = __proxy__(() => csharp.UnityEngine.UserAuthorization);
const $NetworkReachability = __proxy__(() => csharp.UnityEngine.NetworkReachability);
const $AudioType = __proxy__(() => csharp.UnityEngine.AudioType);
const $CachedAssetBundle = __proxy__(() => csharp.UnityEngine.CachedAssetBundle);
const $Cache = __proxy__(() => csharp.UnityEngine.Cache);
const $Camera = __proxy__(() => csharp.UnityEngine.Camera);
const $RenderingPath = __proxy__(() => csharp.UnityEngine.RenderingPath);
const $TransparencySortMode = __proxy__(() => csharp.UnityEngine.TransparencySortMode);
const $CameraType = __proxy__(() => csharp.UnityEngine.CameraType);
const $Matrix4x4 = __proxy__(() => csharp.UnityEngine.Matrix4x4);
const $CameraClearFlags = __proxy__(() => csharp.UnityEngine.CameraClearFlags);
const $DepthTextureMode = __proxy__(() => csharp.UnityEngine.DepthTextureMode);
const $Shader = __proxy__(() => csharp.UnityEngine.Shader);
const $Rect = __proxy__(() => csharp.UnityEngine.Rect);
const $RenderTexture = __proxy__(() => csharp.UnityEngine.RenderTexture);
const $RenderBuffer = __proxy__(() => csharp.UnityEngine.RenderBuffer);
const $Vector4 = __proxy__(() => csharp.UnityEngine.Vector4);
const $Ray = __proxy__(() => csharp.UnityEngine.Ray);
const $StereoTargetEyeMask = __proxy__(() => csharp.UnityEngine.StereoTargetEyeMask);
const $Cubemap = __proxy__(() => csharp.UnityEngine.Cubemap);
const $BoundingSphere = __proxy__(() => csharp.UnityEngine.BoundingSphere);
const $CullingGroupEvent = __proxy__(() => csharp.UnityEngine.CullingGroupEvent);
const $CullingGroup = __proxy__(() => csharp.UnityEngine.CullingGroup);
const $FlareLayer = __proxy__(() => csharp.UnityEngine.FlareLayer);
const $ReflectionProbe = __proxy__(() => csharp.UnityEngine.ReflectionProbe);
const $CrashReport = __proxy__(() => csharp.UnityEngine.CrashReport);
const $Debug = __proxy__(() => csharp.UnityEngine.Debug);
const $ILogger = __proxy__(() => csharp.UnityEngine.ILogger);
const $ILogHandler = __proxy__(() => csharp.UnityEngine.ILogHandler);
const $ExposedPropertyResolver = __proxy__(() => csharp.UnityEngine.ExposedPropertyResolver);
const $IExposedPropertyTable = __proxy__(() => csharp.UnityEngine.IExposedPropertyTable);
const $PropertyName = __proxy__(() => csharp.UnityEngine.PropertyName);
const $BoundsInt = __proxy__(() => csharp.UnityEngine.BoundsInt);
const $Vector3Int = __proxy__(() => csharp.UnityEngine.Vector3Int);
const $GeometryUtility = __proxy__(() => csharp.UnityEngine.GeometryUtility);
const $Plane = __proxy__(() => csharp.UnityEngine.Plane);
const $Ray2D = __proxy__(() => csharp.UnityEngine.Ray2D);
const $RectInt = __proxy__(() => csharp.UnityEngine.RectInt);
const $Vector2Int = __proxy__(() => csharp.UnityEngine.Vector2Int);
const $RectOffset = __proxy__(() => csharp.UnityEngine.RectOffset);
const $DynamicGI = __proxy__(() => csharp.UnityEngine.DynamicGI);
const $Renderer = __proxy__(() => csharp.UnityEngine.Renderer);
const $Gizmos = __proxy__(() => csharp.UnityEngine.Gizmos);
const $Mesh = __proxy__(() => csharp.UnityEngine.Mesh);
const $Material = __proxy__(() => csharp.UnityEngine.Material);
const $BeforeRenderOrderAttribute = __proxy__(() => csharp.UnityEngine.BeforeRenderOrderAttribute);
const $BillboardAsset = __proxy__(() => csharp.UnityEngine.BillboardAsset);
const $BillboardRenderer = __proxy__(() => csharp.UnityEngine.BillboardRenderer);
const $Display = __proxy__(() => csharp.UnityEngine.Display);
const $FullScreenMode = __proxy__(() => csharp.UnityEngine.FullScreenMode);
const $SleepTimeout = __proxy__(() => csharp.UnityEngine.SleepTimeout);
const $Screen = __proxy__(() => csharp.UnityEngine.Screen);
const $ScreenOrientation = __proxy__(() => csharp.UnityEngine.ScreenOrientation);
const $ComputeBufferMode = __proxy__(() => csharp.UnityEngine.ComputeBufferMode);
const $Graphics = __proxy__(() => csharp.UnityEngine.Graphics);
const $ColorGamut = __proxy__(() => csharp.UnityEngine.ColorGamut);
const $CubemapFace = __proxy__(() => csharp.UnityEngine.CubemapFace);
const $RenderTargetSetup = __proxy__(() => csharp.UnityEngine.RenderTargetSetup);
const $ComputeBuffer = __proxy__(() => csharp.UnityEngine.ComputeBuffer);
const $MaterialPropertyBlock = __proxy__(() => csharp.UnityEngine.MaterialPropertyBlock);
const $LightProbeProxyVolume = __proxy__(() => csharp.UnityEngine.LightProbeProxyVolume);
const $MeshTopology = __proxy__(() => csharp.UnityEngine.MeshTopology);
const $GraphicsBuffer = __proxy__(() => csharp.UnityEngine.GraphicsBuffer);
const $GL = __proxy__(() => csharp.UnityEngine.GL);
const $ScalableBufferManager = __proxy__(() => csharp.UnityEngine.ScalableBufferManager);
const $FrameTiming = __proxy__(() => csharp.UnityEngine.FrameTiming);
const $FrameTimingManager = __proxy__(() => csharp.UnityEngine.FrameTimingManager);
const $LightmapData = __proxy__(() => csharp.UnityEngine.LightmapData);
const $Texture2D = __proxy__(() => csharp.UnityEngine.Texture2D);
const $LightmapSettings = __proxy__(() => csharp.UnityEngine.LightmapSettings);
const $LightmapsMode = __proxy__(() => csharp.UnityEngine.LightmapsMode);
const $LightProbes = __proxy__(() => csharp.UnityEngine.LightProbes);
const $LightmapsModeLegacy = __proxy__(() => csharp.UnityEngine.LightmapsModeLegacy);
const $ColorSpace = __proxy__(() => csharp.UnityEngine.ColorSpace);
const $D3DHDRDisplayBitDepth = __proxy__(() => csharp.UnityEngine.D3DHDRDisplayBitDepth);
const $HDROutputSettings = __proxy__(() => csharp.UnityEngine.HDROutputSettings);
const $QualitySettings = __proxy__(() => csharp.UnityEngine.QualitySettings);
const $QualityLevel = __proxy__(() => csharp.UnityEngine.QualityLevel);
const $ShadowQuality = __proxy__(() => csharp.UnityEngine.ShadowQuality);
const $ShadowProjection = __proxy__(() => csharp.UnityEngine.ShadowProjection);
const $ShadowResolution = __proxy__(() => csharp.UnityEngine.ShadowResolution);
const $ShadowmaskMode = __proxy__(() => csharp.UnityEngine.ShadowmaskMode);
const $AnisotropicFiltering = __proxy__(() => csharp.UnityEngine.AnisotropicFiltering);
const $BlendWeights = __proxy__(() => csharp.UnityEngine.BlendWeights);
const $SkinWeights = __proxy__(() => csharp.UnityEngine.SkinWeights);
const $RendererExtensions = __proxy__(() => csharp.UnityEngine.RendererExtensions);
const $ImageEffectTransformsToLDR = __proxy__(() => csharp.UnityEngine.ImageEffectTransformsToLDR);
const $ImageEffectAllowedInSceneView = __proxy__(() => csharp.UnityEngine.ImageEffectAllowedInSceneView);
const $ImageEffectOpaque = __proxy__(() => csharp.UnityEngine.ImageEffectOpaque);
const $ImageEffectAfterScale = __proxy__(() => csharp.UnityEngine.ImageEffectAfterScale);
const $ImageEffectUsesCommandBuffer = __proxy__(() => csharp.UnityEngine.ImageEffectUsesCommandBuffer);
const $BoneWeight1 = __proxy__(() => csharp.UnityEngine.BoneWeight1);
const $BoneWeight = __proxy__(() => csharp.UnityEngine.BoneWeight);
const $CombineInstance = __proxy__(() => csharp.UnityEngine.CombineInstance);
const $MotionVectorGenerationMode = __proxy__(() => csharp.UnityEngine.MotionVectorGenerationMode);
const $Projector = __proxy__(() => csharp.UnityEngine.Projector);
const $TexGenMode = __proxy__(() => csharp.UnityEngine.TexGenMode);
const $TrailRenderer = __proxy__(() => csharp.UnityEngine.TrailRenderer);
const $LineTextureMode = __proxy__(() => csharp.UnityEngine.LineTextureMode);
const $LineAlignment = __proxy__(() => csharp.UnityEngine.LineAlignment);
const $Gradient = __proxy__(() => csharp.UnityEngine.Gradient);
const $LineRenderer = __proxy__(() => csharp.UnityEngine.LineRenderer);
const $RenderSettings = __proxy__(() => csharp.UnityEngine.RenderSettings);
const $FogMode = __proxy__(() => csharp.UnityEngine.FogMode);
const $Light = __proxy__(() => csharp.UnityEngine.Light);
const $MaterialGlobalIlluminationFlags = __proxy__(() => csharp.UnityEngine.MaterialGlobalIlluminationFlags);
const $OcclusionPortal = __proxy__(() => csharp.UnityEngine.OcclusionPortal);
const $OcclusionArea = __proxy__(() => csharp.UnityEngine.OcclusionArea);
const $Flare = __proxy__(() => csharp.UnityEngine.Flare);
const $LensFlare = __proxy__(() => csharp.UnityEngine.LensFlare);
const $LightBakingOutput = __proxy__(() => csharp.UnityEngine.LightBakingOutput);
const $LightmapBakeType = __proxy__(() => csharp.UnityEngine.LightmapBakeType);
const $MixedLightingMode = __proxy__(() => csharp.UnityEngine.MixedLightingMode);
const $LightShadowCasterMode = __proxy__(() => csharp.UnityEngine.LightShadowCasterMode);
const $LightType = __proxy__(() => csharp.UnityEngine.LightType);
const $LightShape = __proxy__(() => csharp.UnityEngine.LightShape);
const $LightShadows = __proxy__(() => csharp.UnityEngine.LightShadows);
const $LightRenderMode = __proxy__(() => csharp.UnityEngine.LightRenderMode);
const $LightmappingMode = __proxy__(() => csharp.UnityEngine.LightmappingMode);
const $Skybox = __proxy__(() => csharp.UnityEngine.Skybox);
const $MeshFilter = __proxy__(() => csharp.UnityEngine.MeshFilter);
const $ComputeBufferType = __proxy__(() => csharp.UnityEngine.ComputeBufferType);
const $ReceiveGI = __proxy__(() => csharp.UnityEngine.ReceiveGI);
const $SkinQuality = __proxy__(() => csharp.UnityEngine.SkinQuality);
const $FilterMode = __proxy__(() => csharp.UnityEngine.FilterMode);
const $TextureWrapMode = __proxy__(() => csharp.UnityEngine.TextureWrapMode);
const $NPOTSupport = __proxy__(() => csharp.UnityEngine.NPOTSupport);
const $TextureFormat = __proxy__(() => csharp.UnityEngine.TextureFormat);
const $RenderTextureFormat = __proxy__(() => csharp.UnityEngine.RenderTextureFormat);
const $VRTextureUsage = __proxy__(() => csharp.UnityEngine.VRTextureUsage);
const $RenderTextureCreationFlags = __proxy__(() => csharp.UnityEngine.RenderTextureCreationFlags);
const $RenderTextureReadWrite = __proxy__(() => csharp.UnityEngine.RenderTextureReadWrite);
const $RenderTextureMemoryless = __proxy__(() => csharp.UnityEngine.RenderTextureMemoryless);
const $CustomRenderTextureInitializationSource = __proxy__(() => csharp.UnityEngine.CustomRenderTextureInitializationSource);
const $CustomRenderTextureUpdateMode = __proxy__(() => csharp.UnityEngine.CustomRenderTextureUpdateMode);
const $CustomRenderTextureUpdateZoneSpace = __proxy__(() => csharp.UnityEngine.CustomRenderTextureUpdateZoneSpace);
const $SkinnedMeshRenderer = __proxy__(() => csharp.UnityEngine.SkinnedMeshRenderer);
const $LightProbeGroup = __proxy__(() => csharp.UnityEngine.LightProbeGroup);
const $LineUtility = __proxy__(() => csharp.UnityEngine.LineUtility);
const $LODFadeMode = __proxy__(() => csharp.UnityEngine.LODFadeMode);
const $LOD = __proxy__(() => csharp.UnityEngine.LOD);
const $LODGroup = __proxy__(() => csharp.UnityEngine.LODGroup);
const $Texture3D = __proxy__(() => csharp.UnityEngine.Texture3D);
const $Texture2DArray = __proxy__(() => csharp.UnityEngine.Texture2DArray);
const $CubemapArray = __proxy__(() => csharp.UnityEngine.CubemapArray);
const $SparseTexture = __proxy__(() => csharp.UnityEngine.SparseTexture);
const $RenderTextureDescriptor = __proxy__(() => csharp.UnityEngine.RenderTextureDescriptor);
const $CustomRenderTextureUpdateZone = __proxy__(() => csharp.UnityEngine.CustomRenderTextureUpdateZone);
const $CustomRenderTexture = __proxy__(() => csharp.UnityEngine.CustomRenderTexture);
const $FullScreenMovieControlMode = __proxy__(() => csharp.UnityEngine.FullScreenMovieControlMode);
const $FullScreenMovieScalingMode = __proxy__(() => csharp.UnityEngine.FullScreenMovieScalingMode);
const $AndroidActivityIndicatorStyle = __proxy__(() => csharp.UnityEngine.AndroidActivityIndicatorStyle);
const $HashUtilities = __proxy__(() => csharp.UnityEngine.HashUtilities);
const $HashUnsafeUtilities = __proxy__(() => csharp.UnityEngine.HashUnsafeUtilities);
const $CursorMode = __proxy__(() => csharp.UnityEngine.CursorMode);
const $CursorLockMode = __proxy__(() => csharp.UnityEngine.CursorLockMode);
const $Cursor = __proxy__(() => csharp.UnityEngine.Cursor);
const $KeyCode = __proxy__(() => csharp.UnityEngine.KeyCode);
const $iPhoneScreenOrientation = __proxy__(() => csharp.UnityEngine.iPhoneScreenOrientation);
const $iPhoneNetworkReachability = __proxy__(() => csharp.UnityEngine.iPhoneNetworkReachability);
const $iPhoneGeneration = __proxy__(() => csharp.UnityEngine.iPhoneGeneration);
const $iPhoneTouchPhase = __proxy__(() => csharp.UnityEngine.iPhoneTouchPhase);
const $iPhoneMovieControlMode = __proxy__(() => csharp.UnityEngine.iPhoneMovieControlMode);
const $iPhoneMovieScalingMode = __proxy__(() => csharp.UnityEngine.iPhoneMovieScalingMode);
const $iPhoneKeyboardType = __proxy__(() => csharp.UnityEngine.iPhoneKeyboardType);
const $iPhoneOrientation = __proxy__(() => csharp.UnityEngine.iPhoneOrientation);
const $iOSActivityIndicatorStyle = __proxy__(() => csharp.UnityEngine.iOSActivityIndicatorStyle);
const $CalendarIdentifier = __proxy__(() => csharp.UnityEngine.CalendarIdentifier);
const $CalendarUnit = __proxy__(() => csharp.UnityEngine.CalendarUnit);
const $RemoteNotificationType = __proxy__(() => csharp.UnityEngine.RemoteNotificationType);
const $Logger = __proxy__(() => csharp.UnityEngine.Logger);
const $ColorUtility = __proxy__(() => csharp.UnityEngine.ColorUtility);
const $GradientColorKey = __proxy__(() => csharp.UnityEngine.GradientColorKey);
const $GradientAlphaKey = __proxy__(() => csharp.UnityEngine.GradientAlphaKey);
const $GradientMode = __proxy__(() => csharp.UnityEngine.GradientMode);
const $FrustumPlanes = __proxy__(() => csharp.UnityEngine.FrustumPlanes);
const $Mathf = __proxy__(() => csharp.UnityEngine.Mathf);
const $RPCMode = __proxy__(() => csharp.UnityEngine.RPCMode);
const $ConnectionTesterStatus = __proxy__(() => csharp.UnityEngine.ConnectionTesterStatus);
const $NetworkConnectionError = __proxy__(() => csharp.UnityEngine.NetworkConnectionError);
const $NetworkDisconnection = __proxy__(() => csharp.UnityEngine.NetworkDisconnection);
const $MasterServerEvent = __proxy__(() => csharp.UnityEngine.MasterServerEvent);
const $NetworkStateSynchronization = __proxy__(() => csharp.UnityEngine.NetworkStateSynchronization);
const $NetworkPeerType = __proxy__(() => csharp.UnityEngine.NetworkPeerType);
const $NetworkLogLevel = __proxy__(() => csharp.UnityEngine.NetworkLogLevel);
const $PlayerPrefsException = __proxy__(() => csharp.UnityEngine.PlayerPrefsException);
const $PlayerPrefs = __proxy__(() => csharp.UnityEngine.PlayerPrefs);
const $PropertyAttribute = __proxy__(() => csharp.UnityEngine.PropertyAttribute);
const $ContextMenuItemAttribute = __proxy__(() => csharp.UnityEngine.ContextMenuItemAttribute);
const $InspectorNameAttribute = __proxy__(() => csharp.UnityEngine.InspectorNameAttribute);
const $TooltipAttribute = __proxy__(() => csharp.UnityEngine.TooltipAttribute);
const $SpaceAttribute = __proxy__(() => csharp.UnityEngine.SpaceAttribute);
const $HeaderAttribute = __proxy__(() => csharp.UnityEngine.HeaderAttribute);
const $RangeAttribute = __proxy__(() => csharp.UnityEngine.RangeAttribute);
const $MinAttribute = __proxy__(() => csharp.UnityEngine.MinAttribute);
const $MultilineAttribute = __proxy__(() => csharp.UnityEngine.MultilineAttribute);
const $TextAreaAttribute = __proxy__(() => csharp.UnityEngine.TextAreaAttribute);
const $ColorUsageAttribute = __proxy__(() => csharp.UnityEngine.ColorUsageAttribute);
const $GradientUsageAttribute = __proxy__(() => csharp.UnityEngine.GradientUsageAttribute);
const $DelayedAttribute = __proxy__(() => csharp.UnityEngine.DelayedAttribute);
const $Random = __proxy__(() => csharp.UnityEngine.Random);
const $ResourceRequest = __proxy__(() => csharp.UnityEngine.ResourceRequest);
const $Resources = __proxy__(() => csharp.UnityEngine.Resources);
const $DisallowMultipleComponent = __proxy__(() => csharp.UnityEngine.DisallowMultipleComponent);
const $RequireComponent = __proxy__(() => csharp.UnityEngine.RequireComponent);
const $AddComponentMenu = __proxy__(() => csharp.UnityEngine.AddComponentMenu);
const $CreateAssetMenuAttribute = __proxy__(() => csharp.UnityEngine.CreateAssetMenuAttribute);
const $ContextMenu = __proxy__(() => csharp.UnityEngine.ContextMenu);
const $ExecuteInEditMode = __proxy__(() => csharp.UnityEngine.ExecuteInEditMode);
const $ExecuteAlways = __proxy__(() => csharp.UnityEngine.ExecuteAlways);
const $HideInInspector = __proxy__(() => csharp.UnityEngine.HideInInspector);
const $HelpURLAttribute = __proxy__(() => csharp.UnityEngine.HelpURLAttribute);
const $DefaultExecutionOrder = __proxy__(() => csharp.UnityEngine.DefaultExecutionOrder);
const $AssemblyIsEditorAssembly = __proxy__(() => csharp.UnityEngine.AssemblyIsEditorAssembly);
const $ExcludeFromPresetAttribute = __proxy__(() => csharp.UnityEngine.ExcludeFromPresetAttribute);
const $Coroutine = __proxy__(() => csharp.UnityEngine.Coroutine);
const $CustomYieldInstruction = __proxy__(() => csharp.UnityEngine.CustomYieldInstruction);
const $ExcludeFromObjectFactoryAttribute = __proxy__(() => csharp.UnityEngine.ExcludeFromObjectFactoryAttribute);
const $LayerMask = __proxy__(() => csharp.UnityEngine.LayerMask);
const $MonoBehaviour = __proxy__(() => csharp.UnityEngine.MonoBehaviour);
const $RangeInt = __proxy__(() => csharp.UnityEngine.RangeInt);
const $RuntimeInitializeLoadType = __proxy__(() => csharp.UnityEngine.RuntimeInitializeLoadType);
const $RuntimeInitializeOnLoadMethodAttribute = __proxy__(() => csharp.UnityEngine.RuntimeInitializeOnLoadMethodAttribute);
const $SelectionBaseAttribute = __proxy__(() => csharp.UnityEngine.SelectionBaseAttribute);
const $StackTraceUtility = __proxy__(() => csharp.UnityEngine.StackTraceUtility);
const $UnityException = __proxy__(() => csharp.UnityEngine.UnityException);
const $MissingComponentException = __proxy__(() => csharp.UnityEngine.MissingComponentException);
const $UnassignedReferenceException = __proxy__(() => csharp.UnityEngine.UnassignedReferenceException);
const $MissingReferenceException = __proxy__(() => csharp.UnityEngine.MissingReferenceException);
const $TextAsset = __proxy__(() => csharp.UnityEngine.TextAsset);
const $UnityAPICompatibilityVersionAttribute = __proxy__(() => csharp.UnityEngine.UnityAPICompatibilityVersionAttribute);
const $HideFlags = __proxy__(() => csharp.UnityEngine.HideFlags);
const $WaitForEndOfFrame = __proxy__(() => csharp.UnityEngine.WaitForEndOfFrame);
const $WaitForFixedUpdate = __proxy__(() => csharp.UnityEngine.WaitForFixedUpdate);
const $WaitForSeconds = __proxy__(() => csharp.UnityEngine.WaitForSeconds);
const $WaitForSecondsRealtime = __proxy__(() => csharp.UnityEngine.WaitForSecondsRealtime);
const $WaitUntil = __proxy__(() => csharp.UnityEngine.WaitUntil);
const $WaitWhile = __proxy__(() => csharp.UnityEngine.WaitWhile);
const $Security = __proxy__(() => csharp.UnityEngine.Security);
const $Types = __proxy__(() => csharp.UnityEngine.Types);
const $SerializeField = __proxy__(() => csharp.UnityEngine.SerializeField);
const $SerializeReference = __proxy__(() => csharp.UnityEngine.SerializeReference);
const $PreferBinarySerialization = __proxy__(() => csharp.UnityEngine.PreferBinarySerialization);
const $ISerializationCallbackReceiver = __proxy__(() => csharp.UnityEngine.ISerializationCallbackReceiver);
const $ShaderVariantCollection = __proxy__(() => csharp.UnityEngine.ShaderVariantCollection);
const $ComputeShader = __proxy__(() => csharp.UnityEngine.ComputeShader);
const $SnapAxis = __proxy__(() => csharp.UnityEngine.SnapAxis);
const $Snapping = __proxy__(() => csharp.UnityEngine.Snapping);
const $StaticBatchingUtility = __proxy__(() => csharp.UnityEngine.StaticBatchingUtility);
const $BatteryStatus = __proxy__(() => csharp.UnityEngine.BatteryStatus);
const $OperatingSystemFamily = __proxy__(() => csharp.UnityEngine.OperatingSystemFamily);
const $DeviceType = __proxy__(() => csharp.UnityEngine.DeviceType);
const $SystemInfo = __proxy__(() => csharp.UnityEngine.SystemInfo);
const $Time = __proxy__(() => csharp.UnityEngine.Time);
const $TouchScreenKeyboard = __proxy__(() => csharp.UnityEngine.TouchScreenKeyboard);
const $TouchScreenKeyboardType = __proxy__(() => csharp.UnityEngine.TouchScreenKeyboardType);
const $UnityEventQueueSystem = __proxy__(() => csharp.UnityEngine.UnityEventQueueSystem);
const $Pose = __proxy__(() => csharp.UnityEngine.Pose);
const $DrivenTransformProperties = __proxy__(() => csharp.UnityEngine.DrivenTransformProperties);
const $DrivenRectTransformTracker = __proxy__(() => csharp.UnityEngine.DrivenRectTransformTracker);
const $RectTransform = __proxy__(() => csharp.UnityEngine.RectTransform);
const $SpriteDrawMode = __proxy__(() => csharp.UnityEngine.SpriteDrawMode);
const $SpriteTileMode = __proxy__(() => csharp.UnityEngine.SpriteTileMode);
const $SpriteMaskInteraction = __proxy__(() => csharp.UnityEngine.SpriteMaskInteraction);
const $SpriteRenderer = __proxy__(() => csharp.UnityEngine.SpriteRenderer);
const $Sprite = __proxy__(() => csharp.UnityEngine.Sprite);
const $SpriteSortPoint = __proxy__(() => csharp.UnityEngine.SpriteSortPoint);
const $SpriteMeshType = __proxy__(() => csharp.UnityEngine.SpriteMeshType);
const $SpriteAlignment = __proxy__(() => csharp.UnityEngine.SpriteAlignment);
const $SpritePackingMode = __proxy__(() => csharp.UnityEngine.SpritePackingMode);
const $SpritePackingRotation = __proxy__(() => csharp.UnityEngine.SpritePackingRotation);
const $SecondarySpriteTexture = __proxy__(() => csharp.UnityEngine.SecondarySpriteTexture);
const $Social = __proxy__(() => csharp.UnityEngine.Social);
const $GridLayout = __proxy__(() => csharp.UnityEngine.GridLayout);
const $Grid = __proxy__(() => csharp.UnityEngine.Grid);
const $Event = __proxy__(() => csharp.UnityEngine.Event);
const $EventType = __proxy__(() => csharp.UnityEngine.EventType);
const $PointerType = __proxy__(() => csharp.UnityEngine.PointerType);
const $EventModifiers = __proxy__(() => csharp.UnityEngine.EventModifiers);
const $GUI = __proxy__(() => csharp.UnityEngine.GUI);
const $GUISkin = __proxy__(() => csharp.UnityEngine.GUISkin);
const $GUIContent = __proxy__(() => csharp.UnityEngine.GUIContent);
const $GUIStyle = __proxy__(() => csharp.UnityEngine.GUIStyle);
const $ScaleMode = __proxy__(() => csharp.UnityEngine.ScaleMode);
const $FocusType = __proxy__(() => csharp.UnityEngine.FocusType);
const $GUILayout = __proxy__(() => csharp.UnityEngine.GUILayout);
const $GUILayoutOption = __proxy__(() => csharp.UnityEngine.GUILayoutOption);
const $GUILayoutUtility = __proxy__(() => csharp.UnityEngine.GUILayoutUtility);
const $GUISettings = __proxy__(() => csharp.UnityEngine.GUISettings);
const $Font = __proxy__(() => csharp.UnityEngine.Font);
const $GUIStyleState = __proxy__(() => csharp.UnityEngine.GUIStyleState);
const $ImagePosition = __proxy__(() => csharp.UnityEngine.ImagePosition);
const $TextAnchor = __proxy__(() => csharp.UnityEngine.TextAnchor);
const $TextClipping = __proxy__(() => csharp.UnityEngine.TextClipping);
const $FontStyle = __proxy__(() => csharp.UnityEngine.FontStyle);
const $GUITargetAttribute = __proxy__(() => csharp.UnityEngine.GUITargetAttribute);
const $GUIUtility = __proxy__(() => csharp.UnityEngine.GUIUtility);
const $ExitGUIException = __proxy__(() => csharp.UnityEngine.ExitGUIException);
const $TextEditor = __proxy__(() => csharp.UnityEngine.TextEditor);
const $ImageConversion = __proxy__(() => csharp.UnityEngine.ImageConversion);
const $TouchPhase = __proxy__(() => csharp.UnityEngine.TouchPhase);
const $IMECompositionMode = __proxy__(() => csharp.UnityEngine.IMECompositionMode);
const $TouchType = __proxy__(() => csharp.UnityEngine.TouchType);
const $Touch = __proxy__(() => csharp.UnityEngine.Touch);
const $DeviceOrientation = __proxy__(() => csharp.UnityEngine.DeviceOrientation);
const $AccelerationEvent = __proxy__(() => csharp.UnityEngine.AccelerationEvent);
const $Gyroscope = __proxy__(() => csharp.UnityEngine.Gyroscope);
const $LocationInfo = __proxy__(() => csharp.UnityEngine.LocationInfo);
const $LocationServiceStatus = __proxy__(() => csharp.UnityEngine.LocationServiceStatus);
const $LocationService = __proxy__(() => csharp.UnityEngine.LocationService);
const $Compass = __proxy__(() => csharp.UnityEngine.Compass);
const $Input = __proxy__(() => csharp.UnityEngine.Input);
const $JsonUtility = __proxy__(() => csharp.UnityEngine.JsonUtility);
const $LocalizationAsset = __proxy__(() => csharp.UnityEngine.LocalizationAsset);
const $ParticleSystemEmissionType = __proxy__(() => csharp.UnityEngine.ParticleSystemEmissionType);
const $ParticleSystem = __proxy__(() => csharp.UnityEngine.ParticleSystem);
const $ParticleSystemSimulationSpace = __proxy__(() => csharp.UnityEngine.ParticleSystemSimulationSpace);
const $ParticleSystemScalingMode = __proxy__(() => csharp.UnityEngine.ParticleSystemScalingMode);
const $ParticleSystemCustomData = __proxy__(() => csharp.UnityEngine.ParticleSystemCustomData);
const $ParticleSystemStopBehavior = __proxy__(() => csharp.UnityEngine.ParticleSystemStopBehavior);
const $ParticleSystemCurveMode = __proxy__(() => csharp.UnityEngine.ParticleSystemCurveMode);
const $ParticleSystemEmitterVelocityMode = __proxy__(() => csharp.UnityEngine.ParticleSystemEmitterVelocityMode);
const $ParticleSystemStopAction = __proxy__(() => csharp.UnityEngine.ParticleSystemStopAction);
const $ParticleSystemRingBufferMode = __proxy__(() => csharp.UnityEngine.ParticleSystemRingBufferMode);
const $ParticleSystemCullingMode = __proxy__(() => csharp.UnityEngine.ParticleSystemCullingMode);
const $ParticleSystemShapeType = __proxy__(() => csharp.UnityEngine.ParticleSystemShapeType);
const $ParticleSystemShapeMultiModeValue = __proxy__(() => csharp.UnityEngine.ParticleSystemShapeMultiModeValue);
const $ParticleSystemMeshShapeType = __proxy__(() => csharp.UnityEngine.ParticleSystemMeshShapeType);
const $MeshRenderer = __proxy__(() => csharp.UnityEngine.MeshRenderer);
const $ParticleSystemShapeTextureChannel = __proxy__(() => csharp.UnityEngine.ParticleSystemShapeTextureChannel);
const $ParticleSystemSubEmitterType = __proxy__(() => csharp.UnityEngine.ParticleSystemSubEmitterType);
const $ParticleSystemSubEmitterProperties = __proxy__(() => csharp.UnityEngine.ParticleSystemSubEmitterProperties);
const $ParticleSystemAnimationMode = __proxy__(() => csharp.UnityEngine.ParticleSystemAnimationMode);
const $ParticleSystemAnimationTimeMode = __proxy__(() => csharp.UnityEngine.ParticleSystemAnimationTimeMode);
const $ParticleSystemAnimationType = __proxy__(() => csharp.UnityEngine.ParticleSystemAnimationType);
const $ParticleSystemAnimationRowMode = __proxy__(() => csharp.UnityEngine.ParticleSystemAnimationRowMode);
const $ParticleSystemGradientMode = __proxy__(() => csharp.UnityEngine.ParticleSystemGradientMode);
const $ParticleSystemInheritVelocityMode = __proxy__(() => csharp.UnityEngine.ParticleSystemInheritVelocityMode);
const $ParticleSystemGameObjectFilter = __proxy__(() => csharp.UnityEngine.ParticleSystemGameObjectFilter);
const $ParticleSystemForceField = __proxy__(() => csharp.UnityEngine.ParticleSystemForceField);
const $ParticleSystemNoiseQuality = __proxy__(() => csharp.UnityEngine.ParticleSystemNoiseQuality);
const $ParticleSystemCollisionType = __proxy__(() => csharp.UnityEngine.ParticleSystemCollisionType);
const $ParticleSystemCollisionMode = __proxy__(() => csharp.UnityEngine.ParticleSystemCollisionMode);
const $ParticleSystemCollisionQuality = __proxy__(() => csharp.UnityEngine.ParticleSystemCollisionQuality);
const $ParticleSystemOverlapAction = __proxy__(() => csharp.UnityEngine.ParticleSystemOverlapAction);
const $ParticleSystemTrailMode = __proxy__(() => csharp.UnityEngine.ParticleSystemTrailMode);
const $ParticleSystemTrailTextureMode = __proxy__(() => csharp.UnityEngine.ParticleSystemTrailTextureMode);
const $ParticleSystemCustomDataMode = __proxy__(() => csharp.UnityEngine.ParticleSystemCustomDataMode);
const $ParticlePhysicsExtensions = __proxy__(() => csharp.UnityEngine.ParticlePhysicsExtensions);
const $ParticleCollisionEvent = __proxy__(() => csharp.UnityEngine.ParticleCollisionEvent);
const $ParticleSystemTriggerEventType = __proxy__(() => csharp.UnityEngine.ParticleSystemTriggerEventType);
const $ParticleSystemRenderMode = __proxy__(() => csharp.UnityEngine.ParticleSystemRenderMode);
const $ParticleSystemSortMode = __proxy__(() => csharp.UnityEngine.ParticleSystemSortMode);
const $ParticleSystemRenderSpace = __proxy__(() => csharp.UnityEngine.ParticleSystemRenderSpace);
const $ParticleSystemVertexStream = __proxy__(() => csharp.UnityEngine.ParticleSystemVertexStream);
const $ParticleSystemForceFieldShape = __proxy__(() => csharp.UnityEngine.ParticleSystemForceFieldShape);
const $ParticleSystemVertexStreams = __proxy__(() => csharp.UnityEngine.ParticleSystemVertexStreams);
const $RigidbodyConstraints = __proxy__(() => csharp.UnityEngine.RigidbodyConstraints);
const $ForceMode = __proxy__(() => csharp.UnityEngine.ForceMode);
const $JointDriveMode = __proxy__(() => csharp.UnityEngine.JointDriveMode);
const $JointProjectionMode = __proxy__(() => csharp.UnityEngine.JointProjectionMode);
const $MeshColliderCookingOptions = __proxy__(() => csharp.UnityEngine.MeshColliderCookingOptions);
const $WheelFrictionCurve = __proxy__(() => csharp.UnityEngine.WheelFrictionCurve);
const $SoftJointLimit = __proxy__(() => csharp.UnityEngine.SoftJointLimit);
const $SoftJointLimitSpring = __proxy__(() => csharp.UnityEngine.SoftJointLimitSpring);
const $JointDrive = __proxy__(() => csharp.UnityEngine.JointDrive);
const $RigidbodyInterpolation = __proxy__(() => csharp.UnityEngine.RigidbodyInterpolation);
const $JointMotor = __proxy__(() => csharp.UnityEngine.JointMotor);
const $JointSpring = __proxy__(() => csharp.UnityEngine.JointSpring);
const $JointLimits = __proxy__(() => csharp.UnityEngine.JointLimits);
const $ControllerColliderHit = __proxy__(() => csharp.UnityEngine.ControllerColliderHit);
const $CharacterController = __proxy__(() => csharp.UnityEngine.CharacterController);
const $Rigidbody = __proxy__(() => csharp.UnityEngine.Rigidbody);
const $PhysicMaterialCombine = __proxy__(() => csharp.UnityEngine.PhysicMaterialCombine);
const $Collision = __proxy__(() => csharp.UnityEngine.Collision);
const $ContactPoint = __proxy__(() => csharp.UnityEngine.ContactPoint);
const $CollisionFlags = __proxy__(() => csharp.UnityEngine.CollisionFlags);
const $QueryTriggerInteraction = __proxy__(() => csharp.UnityEngine.QueryTriggerInteraction);
const $CollisionDetectionMode = __proxy__(() => csharp.UnityEngine.CollisionDetectionMode);
const $ConfigurableJointMotion = __proxy__(() => csharp.UnityEngine.ConfigurableJointMotion);
const $RotationDriveMode = __proxy__(() => csharp.UnityEngine.RotationDriveMode);
const $PhysicMaterial = __proxy__(() => csharp.UnityEngine.PhysicMaterial);
const $RaycastHit = __proxy__(() => csharp.UnityEngine.RaycastHit);
const $MeshCollider = __proxy__(() => csharp.UnityEngine.MeshCollider);
const $BoxCollider = __proxy__(() => csharp.UnityEngine.BoxCollider);
const $ConstantForce = __proxy__(() => csharp.UnityEngine.ConstantForce);
const $Joint = __proxy__(() => csharp.UnityEngine.Joint);
const $HingeJoint = __proxy__(() => csharp.UnityEngine.HingeJoint);
const $SpringJoint = __proxy__(() => csharp.UnityEngine.SpringJoint);
const $FixedJoint = __proxy__(() => csharp.UnityEngine.FixedJoint);
const $CharacterJoint = __proxy__(() => csharp.UnityEngine.CharacterJoint);
const $ConfigurableJoint = __proxy__(() => csharp.UnityEngine.ConfigurableJoint);
const $PhysicsScene = __proxy__(() => csharp.UnityEngine.PhysicsScene);
const $PhysicsSceneExtensions = __proxy__(() => csharp.UnityEngine.PhysicsSceneExtensions);
const $Physics = __proxy__(() => csharp.UnityEngine.Physics);
const $RaycastCommand = __proxy__(() => csharp.UnityEngine.RaycastCommand);
const $SpherecastCommand = __proxy__(() => csharp.UnityEngine.SpherecastCommand);
const $CapsulecastCommand = __proxy__(() => csharp.UnityEngine.CapsulecastCommand);
const $BoxcastCommand = __proxy__(() => csharp.UnityEngine.BoxcastCommand);
const $PhysicsScene2D = __proxy__(() => csharp.UnityEngine.PhysicsScene2D);
const $RaycastHit2D = __proxy__(() => csharp.UnityEngine.RaycastHit2D);
const $ContactFilter2D = __proxy__(() => csharp.UnityEngine.ContactFilter2D);
const $CapsuleDirection2D = __proxy__(() => csharp.UnityEngine.CapsuleDirection2D);
const $Collider2D = __proxy__(() => csharp.UnityEngine.Collider2D);
const $PhysicsSceneExtensions2D = __proxy__(() => csharp.UnityEngine.PhysicsSceneExtensions2D);
const $Physics2D = __proxy__(() => csharp.UnityEngine.Physics2D);
const $PhysicsJobOptions2D = __proxy__(() => csharp.UnityEngine.PhysicsJobOptions2D);
const $ColliderDistance2D = __proxy__(() => csharp.UnityEngine.ColliderDistance2D);
const $Rigidbody2D = __proxy__(() => csharp.UnityEngine.Rigidbody2D);
const $ContactPoint2D = __proxy__(() => csharp.UnityEngine.ContactPoint2D);
const $RigidbodyConstraints2D = __proxy__(() => csharp.UnityEngine.RigidbodyConstraints2D);
const $RigidbodyInterpolation2D = __proxy__(() => csharp.UnityEngine.RigidbodyInterpolation2D);
const $RigidbodySleepMode2D = __proxy__(() => csharp.UnityEngine.RigidbodySleepMode2D);
const $CollisionDetectionMode2D = __proxy__(() => csharp.UnityEngine.CollisionDetectionMode2D);
const $RigidbodyType2D = __proxy__(() => csharp.UnityEngine.RigidbodyType2D);
const $ForceMode2D = __proxy__(() => csharp.UnityEngine.ForceMode2D);
const $JointLimitState2D = __proxy__(() => csharp.UnityEngine.JointLimitState2D);
const $EffectorSelection2D = __proxy__(() => csharp.UnityEngine.EffectorSelection2D);
const $EffectorForceMode2D = __proxy__(() => csharp.UnityEngine.EffectorForceMode2D);
const $Collision2D = __proxy__(() => csharp.UnityEngine.Collision2D);
const $JointAngleLimits2D = __proxy__(() => csharp.UnityEngine.JointAngleLimits2D);
const $JointTranslationLimits2D = __proxy__(() => csharp.UnityEngine.JointTranslationLimits2D);
const $JointMotor2D = __proxy__(() => csharp.UnityEngine.JointMotor2D);
const $JointSuspension2D = __proxy__(() => csharp.UnityEngine.JointSuspension2D);
const $PhysicsMaterial2D = __proxy__(() => csharp.UnityEngine.PhysicsMaterial2D);
const $CompositeCollider2D = __proxy__(() => csharp.UnityEngine.CompositeCollider2D);
const $CircleCollider2D = __proxy__(() => csharp.UnityEngine.CircleCollider2D);
const $CapsuleCollider2D = __proxy__(() => csharp.UnityEngine.CapsuleCollider2D);
const $EdgeCollider2D = __proxy__(() => csharp.UnityEngine.EdgeCollider2D);
const $BoxCollider2D = __proxy__(() => csharp.UnityEngine.BoxCollider2D);
const $PolygonCollider2D = __proxy__(() => csharp.UnityEngine.PolygonCollider2D);
const $Joint2D = __proxy__(() => csharp.UnityEngine.Joint2D);
const $AnchoredJoint2D = __proxy__(() => csharp.UnityEngine.AnchoredJoint2D);
const $SpringJoint2D = __proxy__(() => csharp.UnityEngine.SpringJoint2D);
const $DistanceJoint2D = __proxy__(() => csharp.UnityEngine.DistanceJoint2D);
const $FrictionJoint2D = __proxy__(() => csharp.UnityEngine.FrictionJoint2D);
const $HingeJoint2D = __proxy__(() => csharp.UnityEngine.HingeJoint2D);
const $RelativeJoint2D = __proxy__(() => csharp.UnityEngine.RelativeJoint2D);
const $SliderJoint2D = __proxy__(() => csharp.UnityEngine.SliderJoint2D);
const $TargetJoint2D = __proxy__(() => csharp.UnityEngine.TargetJoint2D);
const $FixedJoint2D = __proxy__(() => csharp.UnityEngine.FixedJoint2D);
const $WheelJoint2D = __proxy__(() => csharp.UnityEngine.WheelJoint2D);
const $Effector2D = __proxy__(() => csharp.UnityEngine.Effector2D);
const $AreaEffector2D = __proxy__(() => csharp.UnityEngine.AreaEffector2D);
const $BuoyancyEffector2D = __proxy__(() => csharp.UnityEngine.BuoyancyEffector2D);
const $PointEffector2D = __proxy__(() => csharp.UnityEngine.PointEffector2D);
const $PlatformEffector2D = __proxy__(() => csharp.UnityEngine.PlatformEffector2D);
const $SurfaceEffector2D = __proxy__(() => csharp.UnityEngine.SurfaceEffector2D);
const $PhysicsUpdateBehaviour2D = __proxy__(() => csharp.UnityEngine.PhysicsUpdateBehaviour2D);
const $ConstantForce2D = __proxy__(() => csharp.UnityEngine.ConstantForce2D);
const $ScreenCapture = __proxy__(() => csharp.UnityEngine.ScreenCapture);
const $SpriteMask = __proxy__(() => csharp.UnityEngine.SpriteMask);
const $StreamingController = __proxy__(() => csharp.UnityEngine.StreamingController);
const $ProceduralProcessorUsage = __proxy__(() => csharp.UnityEngine.ProceduralProcessorUsage);
const $ProceduralCacheSize = __proxy__(() => csharp.UnityEngine.ProceduralCacheSize);
const $ProceduralLoadingBehavior = __proxy__(() => csharp.UnityEngine.ProceduralLoadingBehavior);
const $ProceduralPropertyType = __proxy__(() => csharp.UnityEngine.ProceduralPropertyType);
const $ProceduralOutputType = __proxy__(() => csharp.UnityEngine.ProceduralOutputType);
const $ISubsystemDescriptor = __proxy__(() => csharp.UnityEngine.ISubsystemDescriptor);
const $ISubsystem = __proxy__(() => csharp.UnityEngine.ISubsystem);
const $IntegratedSubsystemDescriptor = __proxy__(() => csharp.UnityEngine.IntegratedSubsystemDescriptor);
const $ISubsystemDescriptorImpl = __proxy__(() => csharp.UnityEngine.ISubsystemDescriptorImpl);
const $SubsystemDescriptor = __proxy__(() => csharp.UnityEngine.SubsystemDescriptor);
const $SubsystemManager = __proxy__(() => csharp.UnityEngine.SubsystemManager);
const $IntegratedSubsystem = __proxy__(() => csharp.UnityEngine.IntegratedSubsystem);
const $Subsystem = __proxy__(() => csharp.UnityEngine.Subsystem);
const $TerrainChangedFlags = __proxy__(() => csharp.UnityEngine.TerrainChangedFlags);
const $TerrainRenderFlags = __proxy__(() => csharp.UnityEngine.TerrainRenderFlags);
const $Terrain = __proxy__(() => csharp.UnityEngine.Terrain);
const $TerrainData = __proxy__(() => csharp.UnityEngine.TerrainData);
const $TreeInstance = __proxy__(() => csharp.UnityEngine.TreeInstance);
const $TerrainExtensions = __proxy__(() => csharp.UnityEngine.TerrainExtensions);
const $Tree = __proxy__(() => csharp.UnityEngine.Tree);
const $TreePrototype = __proxy__(() => csharp.UnityEngine.TreePrototype);
const $DetailRenderMode = __proxy__(() => csharp.UnityEngine.DetailRenderMode);
const $DetailPrototype = __proxy__(() => csharp.UnityEngine.DetailPrototype);
const $SplatPrototype = __proxy__(() => csharp.UnityEngine.SplatPrototype);
const $PatchExtents = __proxy__(() => csharp.UnityEngine.PatchExtents);
const $TerrainHeightmapSyncControl = __proxy__(() => csharp.UnityEngine.TerrainHeightmapSyncControl);
const $TerrainLayer = __proxy__(() => csharp.UnityEngine.TerrainLayer);
const $TerrainCollider = __proxy__(() => csharp.UnityEngine.TerrainCollider);
const $TextGenerationSettings = __proxy__(() => csharp.UnityEngine.TextGenerationSettings);
const $VerticalWrapMode = __proxy__(() => csharp.UnityEngine.VerticalWrapMode);
const $HorizontalWrapMode = __proxy__(() => csharp.UnityEngine.HorizontalWrapMode);
const $TextGenerator = __proxy__(() => csharp.UnityEngine.TextGenerator);
const $UICharInfo = __proxy__(() => csharp.UnityEngine.UICharInfo);
const $UILineInfo = __proxy__(() => csharp.UnityEngine.UILineInfo);
const $UIVertex = __proxy__(() => csharp.UnityEngine.UIVertex);
const $TextAlignment = __proxy__(() => csharp.UnityEngine.TextAlignment);
const $TextMesh = __proxy__(() => csharp.UnityEngine.TextMesh);
const $CharacterInfo = __proxy__(() => csharp.UnityEngine.CharacterInfo);
const $CustomGridBrushAttribute = __proxy__(() => csharp.UnityEngine.CustomGridBrushAttribute);
const $GridBrushBase = __proxy__(() => csharp.UnityEngine.GridBrushBase);
const $ICanvasRaycastFilter = __proxy__(() => csharp.UnityEngine.ICanvasRaycastFilter);
const $CanvasGroup = __proxy__(() => csharp.UnityEngine.CanvasGroup);
const $CanvasRenderer = __proxy__(() => csharp.UnityEngine.CanvasRenderer);
const $RectTransformUtility = __proxy__(() => csharp.UnityEngine.RectTransformUtility);
const $Canvas = __proxy__(() => csharp.UnityEngine.Canvas);
const $RenderMode = __proxy__(() => csharp.UnityEngine.RenderMode);
const $AdditionalCanvasShaderChannels = __proxy__(() => csharp.UnityEngine.AdditionalCanvasShaderChannels);
const $UISystemProfilerApi = __proxy__(() => csharp.UnityEngine.UISystemProfilerApi);
const $RemoteSettings = __proxy__(() => csharp.UnityEngine.RemoteSettings);
const $RemoteConfigSettings = __proxy__(() => csharp.UnityEngine.RemoteConfigSettings);
const $WWWForm = __proxy__(() => csharp.UnityEngine.WWWForm);
const $WWWAudioExtensions = __proxy__(() => csharp.UnityEngine.WWWAudioExtensions);
const $WWW = __proxy__(() => csharp.UnityEngine.WWW);
const $MovieTexture = __proxy__(() => csharp.UnityEngine.MovieTexture);
const $WheelHit = __proxy__(() => csharp.UnityEngine.WheelHit);
const $WheelCollider = __proxy__(() => csharp.UnityEngine.WheelCollider);
const $WebGLInput = __proxy__(() => csharp.UnityEngine.WebGLInput);
const $WindZoneMode = __proxy__(() => csharp.UnityEngine.WindZoneMode);
const $WindZone = __proxy__(() => csharp.UnityEngine.WindZone);

const $Playables = __proxy__(() => csharp.UnityEngine.Playables);
const $Animations = __proxy__(() => csharp.UnityEngine.Animations);
const $Audio = __proxy__(() => csharp.UnityEngine.Audio);
const $Internal = __proxy__(() => csharp.UnityEngine.Internal);
const $Events = __proxy__(() => csharp.UnityEngine.Events);
const $Rendering = __proxy__(() => csharp.UnityEngine.Rendering);
const $SceneManagement = __proxy__(() => csharp.UnityEngine.SceneManagement);
const $ADBannerView = __proxy__(() => csharp.UnityEngine.ADBannerView);
const $ADInterstitialAd = __proxy__(() => csharp.UnityEngine.ADInterstitialAd);
const $Scripting = __proxy__(() => csharp.UnityEngine.Scripting);
const $SocialPlatforms = __proxy__(() => csharp.UnityEngine.SocialPlatforms);
const $UI = __proxy__(() => csharp.UnityEngine.UI);
const $EventSystems = __proxy__(() => csharp.UnityEngine.EventSystems);
const $Experimental = __proxy__(() => csharp.UnityEngine.Experimental);

export {

    $AndroidJavaRunnable as AndroidJavaRunnable,
    $AndroidJavaException as AndroidJavaException,
    $AndroidJavaObject as AndroidJavaObject,
    $AndroidJavaClass as AndroidJavaClass,
    $AndroidJavaProxy as AndroidJavaProxy,
    $jvalue as jvalue,
    $AndroidJNIHelper as AndroidJNIHelper,
    $AndroidJNI as AndroidJNI,
    $Object as Object,
    $Component as Component,
    $Behaviour as Behaviour,
    $Animator as Animator,
    $AnimationInfo as AnimationInfo,
    $Vector3 as Vector3,
    $Quaternion as Quaternion,
    $AnimatorUpdateMode as AnimatorUpdateMode,
    $AvatarIKGoal as AvatarIKGoal,
    $AvatarIKHint as AvatarIKHint,
    $HumanBodyBones as HumanBodyBones,
    $ScriptableObject as ScriptableObject,
    $StateMachineBehaviour as StateMachineBehaviour,
    $AnimatorStateInfo as AnimatorStateInfo,
    $AnimatorTransitionInfo as AnimatorTransitionInfo,
    $AnimatorClipInfo as AnimatorClipInfo,
    $AnimatorControllerParameter as AnimatorControllerParameter,
    $AvatarTarget as AvatarTarget,
    $MatchTargetWeightMask as MatchTargetWeightMask,
    $Transform as Transform,
    $AnimatorCullingMode as AnimatorCullingMode,
    $AnimatorRecorderMode as AnimatorRecorderMode,
    $RuntimeAnimatorController as RuntimeAnimatorController,
    $Avatar as Avatar,
    $IAnimationClipSource as IAnimationClipSource,
    $Motion as Motion,
    $AnimationClip as AnimationClip,
    $SharedBetweenAnimatorsAttribute as SharedBetweenAnimatorsAttribute,
    $GameObject as GameObject,
    $AnimationCurve as AnimationCurve,
    $WrapMode as WrapMode,
    $Bounds as Bounds,
    $AnimationEvent as AnimationEvent,
    $AnimatorControllerParameterType as AnimatorControllerParameterType,
    $DurationUnit as DurationUnit,
    $AnimatorOverrideController as AnimatorOverrideController,
    $AnimationClipPair as AnimationClipPair,
    $AnimatorUtility as AnimatorUtility,
    $BodyDof as BodyDof,
    $HeadDof as HeadDof,
    $LegDof as LegDof,
    $ArmDof as ArmDof,
    $FingerDof as FingerDof,
    $HumanPartDof as HumanPartDof,
    $HumanDescription as HumanDescription,
    $SkeletonBone as SkeletonBone,
    $HumanLimit as HumanLimit,
    $HumanBone as HumanBone,
    $AvatarBuilder as AvatarBuilder,
    $AvatarMaskBodyPart as AvatarMaskBodyPart,
    $AvatarMask as AvatarMask,
    $HumanPose as HumanPose,
    $HumanPoseHandler as HumanPoseHandler,
    $HumanTrait as HumanTrait,
    $SendMessageOptions as SendMessageOptions,
    $TrackedReference as TrackedReference,
    $AnimationState as AnimationState,
    $PlayMode as PlayMode,
    $QueueMode as QueueMode,
    $AnimationBlendMode as AnimationBlendMode,
    $AnimationPlayMode as AnimationPlayMode,
    $AnimationCullingType as AnimationCullingType,
    $Animation as Animation,
    $AssetBundleLoadResult as AssetBundleLoadResult,
    $AssetBundle as AssetBundle,
    $YieldInstruction as YieldInstruction,
    $AsyncOperation as AsyncOperation,
    $AssetBundleCreateRequest as AssetBundleCreateRequest,
    $AssetBundleRequest as AssetBundleRequest,
    $AssetBundleRecompressOperation as AssetBundleRecompressOperation,
    $BuildCompression as BuildCompression,
    $ThreadPriority as ThreadPriority,
    $AssetBundleManifest as AssetBundleManifest,
    $Hash128 as Hash128,
    $CompressionType as CompressionType,
    $CompressionLevel as CompressionLevel,
    $AudioSettings as AudioSettings,
    $AudioSpeakerMode as AudioSpeakerMode,
    $AudioConfiguration as AudioConfiguration,
    $AudioBehaviour as AudioBehaviour,
    $AudioSource as AudioSource,
    $AudioClip as AudioClip,
    $AudioVelocityUpdateMode as AudioVelocityUpdateMode,
    $AudioSourceCurveType as AudioSourceCurveType,
    $AudioRolloffMode as AudioRolloffMode,
    $FFTWindow as FFTWindow,
    $AudioLowPassFilter as AudioLowPassFilter,
    $AudioHighPassFilter as AudioHighPassFilter,
    $AudioReverbFilter as AudioReverbFilter,
    $AudioReverbPreset as AudioReverbPreset,
    $AudioDataLoadState as AudioDataLoadState,
    $AudioCompressionFormat as AudioCompressionFormat,
    $AudioClipLoadType as AudioClipLoadType,
    $AudioListener as AudioListener,
    $AudioReverbZone as AudioReverbZone,
    $AudioDistortionFilter as AudioDistortionFilter,
    $AudioEchoFilter as AudioEchoFilter,
    $AudioChorusFilter as AudioChorusFilter,
    $AudioRenderer as AudioRenderer,
    $WebCamFlags as WebCamFlags,
    $WebCamKind as WebCamKind,
    $WebCamDevice as WebCamDevice,
    $Resolution as Resolution,
    $Texture as Texture,
    $WebCamTexture as WebCamTexture,
    $Vector2 as Vector2,
    $Color as Color,
    $Color32 as Color32,
    $ClothSphereColliderPair as ClothSphereColliderPair,
    $Collider as Collider,
    $SphereCollider as SphereCollider,
    $ClothSkinningCoefficient as ClothSkinningCoefficient,
    $Cloth as Cloth,
    $CapsuleCollider as CapsuleCollider,
    $ClusterInputType as ClusterInputType,
    $PrimitiveType as PrimitiveType,
    $Space as Space,
    $RuntimePlatform as RuntimePlatform,
    $SystemLanguage as SystemLanguage,
    $LogType as LogType,
    $LogOption as LogOption,
    $SortingLayer as SortingLayer,
    $WeightedMode as WeightedMode,
    $Keyframe as Keyframe,
    $Application as Application,
    $ApplicationInstallMode as ApplicationInstallMode,
    $ApplicationSandboxType as ApplicationSandboxType,
    $StackTraceLogType as StackTraceLogType,
    $UserAuthorization as UserAuthorization,
    $NetworkReachability as NetworkReachability,
    $AudioType as AudioType,
    $CachedAssetBundle as CachedAssetBundle,
    $Cache as Cache,
    $Camera as Camera,
    $RenderingPath as RenderingPath,
    $TransparencySortMode as TransparencySortMode,
    $CameraType as CameraType,
    $Matrix4x4 as Matrix4x4,
    $CameraClearFlags as CameraClearFlags,
    $DepthTextureMode as DepthTextureMode,
    $Shader as Shader,
    $Rect as Rect,
    $RenderTexture as RenderTexture,
    $RenderBuffer as RenderBuffer,
    $Vector4 as Vector4,
    $Ray as Ray,
    $StereoTargetEyeMask as StereoTargetEyeMask,
    $Cubemap as Cubemap,
    $BoundingSphere as BoundingSphere,
    $CullingGroupEvent as CullingGroupEvent,
    $CullingGroup as CullingGroup,
    $FlareLayer as FlareLayer,
    $ReflectionProbe as ReflectionProbe,
    $CrashReport as CrashReport,
    $Debug as Debug,
    $ILogger as ILogger,
    $ILogHandler as ILogHandler,
    $ExposedPropertyResolver as ExposedPropertyResolver,
    $IExposedPropertyTable as IExposedPropertyTable,
    $PropertyName as PropertyName,
    $BoundsInt as BoundsInt,
    $Vector3Int as Vector3Int,
    $GeometryUtility as GeometryUtility,
    $Plane as Plane,
    $Ray2D as Ray2D,
    $RectInt as RectInt,
    $Vector2Int as Vector2Int,
    $RectOffset as RectOffset,
    $DynamicGI as DynamicGI,
    $Renderer as Renderer,
    $Gizmos as Gizmos,
    $Mesh as Mesh,
    $Material as Material,
    $BeforeRenderOrderAttribute as BeforeRenderOrderAttribute,
    $BillboardAsset as BillboardAsset,
    $BillboardRenderer as BillboardRenderer,
    $Display as Display,
    $FullScreenMode as FullScreenMode,
    $SleepTimeout as SleepTimeout,
    $Screen as Screen,
    $ScreenOrientation as ScreenOrientation,
    $ComputeBufferMode as ComputeBufferMode,
    $Graphics as Graphics,
    $ColorGamut as ColorGamut,
    $CubemapFace as CubemapFace,
    $RenderTargetSetup as RenderTargetSetup,
    $ComputeBuffer as ComputeBuffer,
    $MaterialPropertyBlock as MaterialPropertyBlock,
    $LightProbeProxyVolume as LightProbeProxyVolume,
    $MeshTopology as MeshTopology,
    $GraphicsBuffer as GraphicsBuffer,
    $GL as GL,
    $ScalableBufferManager as ScalableBufferManager,
    $FrameTiming as FrameTiming,
    $FrameTimingManager as FrameTimingManager,
    $LightmapData as LightmapData,
    $Texture2D as Texture2D,
    $LightmapSettings as LightmapSettings,
    $LightmapsMode as LightmapsMode,
    $LightProbes as LightProbes,
    $LightmapsModeLegacy as LightmapsModeLegacy,
    $ColorSpace as ColorSpace,
    $D3DHDRDisplayBitDepth as D3DHDRDisplayBitDepth,
    $HDROutputSettings as HDROutputSettings,
    $QualitySettings as QualitySettings,
    $QualityLevel as QualityLevel,
    $ShadowQuality as ShadowQuality,
    $ShadowProjection as ShadowProjection,
    $ShadowResolution as ShadowResolution,
    $ShadowmaskMode as ShadowmaskMode,
    $AnisotropicFiltering as AnisotropicFiltering,
    $BlendWeights as BlendWeights,
    $SkinWeights as SkinWeights,
    $RendererExtensions as RendererExtensions,
    $ImageEffectTransformsToLDR as ImageEffectTransformsToLDR,
    $ImageEffectAllowedInSceneView as ImageEffectAllowedInSceneView,
    $ImageEffectOpaque as ImageEffectOpaque,
    $ImageEffectAfterScale as ImageEffectAfterScale,
    $ImageEffectUsesCommandBuffer as ImageEffectUsesCommandBuffer,
    $BoneWeight1 as BoneWeight1,
    $BoneWeight as BoneWeight,
    $CombineInstance as CombineInstance,
    $MotionVectorGenerationMode as MotionVectorGenerationMode,
    $Projector as Projector,
    $TexGenMode as TexGenMode,
    $TrailRenderer as TrailRenderer,
    $LineTextureMode as LineTextureMode,
    $LineAlignment as LineAlignment,
    $Gradient as Gradient,
    $LineRenderer as LineRenderer,
    $RenderSettings as RenderSettings,
    $FogMode as FogMode,
    $Light as Light,
    $MaterialGlobalIlluminationFlags as MaterialGlobalIlluminationFlags,
    $OcclusionPortal as OcclusionPortal,
    $OcclusionArea as OcclusionArea,
    $Flare as Flare,
    $LensFlare as LensFlare,
    $LightBakingOutput as LightBakingOutput,
    $LightmapBakeType as LightmapBakeType,
    $MixedLightingMode as MixedLightingMode,
    $LightShadowCasterMode as LightShadowCasterMode,
    $LightType as LightType,
    $LightShape as LightShape,
    $LightShadows as LightShadows,
    $LightRenderMode as LightRenderMode,
    $LightmappingMode as LightmappingMode,
    $Skybox as Skybox,
    $MeshFilter as MeshFilter,
    $ComputeBufferType as ComputeBufferType,
    $ReceiveGI as ReceiveGI,
    $SkinQuality as SkinQuality,
    $FilterMode as FilterMode,
    $TextureWrapMode as TextureWrapMode,
    $NPOTSupport as NPOTSupport,
    $TextureFormat as TextureFormat,
    $RenderTextureFormat as RenderTextureFormat,
    $VRTextureUsage as VRTextureUsage,
    $RenderTextureCreationFlags as RenderTextureCreationFlags,
    $RenderTextureReadWrite as RenderTextureReadWrite,
    $RenderTextureMemoryless as RenderTextureMemoryless,
    $CustomRenderTextureInitializationSource as CustomRenderTextureInitializationSource,
    $CustomRenderTextureUpdateMode as CustomRenderTextureUpdateMode,
    $CustomRenderTextureUpdateZoneSpace as CustomRenderTextureUpdateZoneSpace,
    $SkinnedMeshRenderer as SkinnedMeshRenderer,
    $LightProbeGroup as LightProbeGroup,
    $LineUtility as LineUtility,
    $LODFadeMode as LODFadeMode,
    $LOD as LOD,
    $LODGroup as LODGroup,
    $Texture3D as Texture3D,
    $Texture2DArray as Texture2DArray,
    $CubemapArray as CubemapArray,
    $SparseTexture as SparseTexture,
    $RenderTextureDescriptor as RenderTextureDescriptor,
    $CustomRenderTextureUpdateZone as CustomRenderTextureUpdateZone,
    $CustomRenderTexture as CustomRenderTexture,
    $FullScreenMovieControlMode as FullScreenMovieControlMode,
    $FullScreenMovieScalingMode as FullScreenMovieScalingMode,
    $AndroidActivityIndicatorStyle as AndroidActivityIndicatorStyle,
    $HashUtilities as HashUtilities,
    $HashUnsafeUtilities as HashUnsafeUtilities,
    $CursorMode as CursorMode,
    $CursorLockMode as CursorLockMode,
    $Cursor as Cursor,
    $KeyCode as KeyCode,
    $iPhoneScreenOrientation as iPhoneScreenOrientation,
    $iPhoneNetworkReachability as iPhoneNetworkReachability,
    $iPhoneGeneration as iPhoneGeneration,
    $iPhoneTouchPhase as iPhoneTouchPhase,
    $iPhoneMovieControlMode as iPhoneMovieControlMode,
    $iPhoneMovieScalingMode as iPhoneMovieScalingMode,
    $iPhoneKeyboardType as iPhoneKeyboardType,
    $iPhoneOrientation as iPhoneOrientation,
    $iOSActivityIndicatorStyle as iOSActivityIndicatorStyle,
    $CalendarIdentifier as CalendarIdentifier,
    $CalendarUnit as CalendarUnit,
    $RemoteNotificationType as RemoteNotificationType,
    $Logger as Logger,
    $ColorUtility as ColorUtility,
    $GradientColorKey as GradientColorKey,
    $GradientAlphaKey as GradientAlphaKey,
    $GradientMode as GradientMode,
    $FrustumPlanes as FrustumPlanes,
    $Mathf as Mathf,
    $RPCMode as RPCMode,
    $ConnectionTesterStatus as ConnectionTesterStatus,
    $NetworkConnectionError as NetworkConnectionError,
    $NetworkDisconnection as NetworkDisconnection,
    $MasterServerEvent as MasterServerEvent,
    $NetworkStateSynchronization as NetworkStateSynchronization,
    $NetworkPeerType as NetworkPeerType,
    $NetworkLogLevel as NetworkLogLevel,
    $PlayerPrefsException as PlayerPrefsException,
    $PlayerPrefs as PlayerPrefs,
    $PropertyAttribute as PropertyAttribute,
    $ContextMenuItemAttribute as ContextMenuItemAttribute,
    $InspectorNameAttribute as InspectorNameAttribute,
    $TooltipAttribute as TooltipAttribute,
    $SpaceAttribute as SpaceAttribute,
    $HeaderAttribute as HeaderAttribute,
    $RangeAttribute as RangeAttribute,
    $MinAttribute as MinAttribute,
    $MultilineAttribute as MultilineAttribute,
    $TextAreaAttribute as TextAreaAttribute,
    $ColorUsageAttribute as ColorUsageAttribute,
    $GradientUsageAttribute as GradientUsageAttribute,
    $DelayedAttribute as DelayedAttribute,
    $Random as Random,
    $ResourceRequest as ResourceRequest,
    $Resources as Resources,
    $DisallowMultipleComponent as DisallowMultipleComponent,
    $RequireComponent as RequireComponent,
    $AddComponentMenu as AddComponentMenu,
    $CreateAssetMenuAttribute as CreateAssetMenuAttribute,
    $ContextMenu as ContextMenu,
    $ExecuteInEditMode as ExecuteInEditMode,
    $ExecuteAlways as ExecuteAlways,
    $HideInInspector as HideInInspector,
    $HelpURLAttribute as HelpURLAttribute,
    $DefaultExecutionOrder as DefaultExecutionOrder,
    $AssemblyIsEditorAssembly as AssemblyIsEditorAssembly,
    $ExcludeFromPresetAttribute as ExcludeFromPresetAttribute,
    $Coroutine as Coroutine,
    $CustomYieldInstruction as CustomYieldInstruction,
    $ExcludeFromObjectFactoryAttribute as ExcludeFromObjectFactoryAttribute,
    $LayerMask as LayerMask,
    $MonoBehaviour as MonoBehaviour,
    $RangeInt as RangeInt,
    $RuntimeInitializeLoadType as RuntimeInitializeLoadType,
    $RuntimeInitializeOnLoadMethodAttribute as RuntimeInitializeOnLoadMethodAttribute,
    $SelectionBaseAttribute as SelectionBaseAttribute,
    $StackTraceUtility as StackTraceUtility,
    $UnityException as UnityException,
    $MissingComponentException as MissingComponentException,
    $UnassignedReferenceException as UnassignedReferenceException,
    $MissingReferenceException as MissingReferenceException,
    $TextAsset as TextAsset,
    $UnityAPICompatibilityVersionAttribute as UnityAPICompatibilityVersionAttribute,
    $HideFlags as HideFlags,
    $WaitForEndOfFrame as WaitForEndOfFrame,
    $WaitForFixedUpdate as WaitForFixedUpdate,
    $WaitForSeconds as WaitForSeconds,
    $WaitForSecondsRealtime as WaitForSecondsRealtime,
    $WaitUntil as WaitUntil,
    $WaitWhile as WaitWhile,
    $Security as Security,
    $Types as Types,
    $SerializeField as SerializeField,
    $SerializeReference as SerializeReference,
    $PreferBinarySerialization as PreferBinarySerialization,
    $ISerializationCallbackReceiver as ISerializationCallbackReceiver,
    $ShaderVariantCollection as ShaderVariantCollection,
    $ComputeShader as ComputeShader,
    $SnapAxis as SnapAxis,
    $Snapping as Snapping,
    $StaticBatchingUtility as StaticBatchingUtility,
    $BatteryStatus as BatteryStatus,
    $OperatingSystemFamily as OperatingSystemFamily,
    $DeviceType as DeviceType,
    $SystemInfo as SystemInfo,
    $Time as Time,
    $TouchScreenKeyboard as TouchScreenKeyboard,
    $TouchScreenKeyboardType as TouchScreenKeyboardType,
    $UnityEventQueueSystem as UnityEventQueueSystem,
    $Pose as Pose,
    $DrivenTransformProperties as DrivenTransformProperties,
    $DrivenRectTransformTracker as DrivenRectTransformTracker,
    $RectTransform as RectTransform,
    $SpriteDrawMode as SpriteDrawMode,
    $SpriteTileMode as SpriteTileMode,
    $SpriteMaskInteraction as SpriteMaskInteraction,
    $SpriteRenderer as SpriteRenderer,
    $Sprite as Sprite,
    $SpriteSortPoint as SpriteSortPoint,
    $SpriteMeshType as SpriteMeshType,
    $SpriteAlignment as SpriteAlignment,
    $SpritePackingMode as SpritePackingMode,
    $SpritePackingRotation as SpritePackingRotation,
    $SecondarySpriteTexture as SecondarySpriteTexture,
    $Social as Social,
    $GridLayout as GridLayout,
    $Grid as Grid,
    $Event as Event,
    $EventType as EventType,
    $PointerType as PointerType,
    $EventModifiers as EventModifiers,
    $GUI as GUI,
    $GUISkin as GUISkin,
    $GUIContent as GUIContent,
    $GUIStyle as GUIStyle,
    $ScaleMode as ScaleMode,
    $FocusType as FocusType,
    $GUILayout as GUILayout,
    $GUILayoutOption as GUILayoutOption,
    $GUILayoutUtility as GUILayoutUtility,
    $GUISettings as GUISettings,
    $Font as Font,
    $GUIStyleState as GUIStyleState,
    $ImagePosition as ImagePosition,
    $TextAnchor as TextAnchor,
    $TextClipping as TextClipping,
    $FontStyle as FontStyle,
    $GUITargetAttribute as GUITargetAttribute,
    $GUIUtility as GUIUtility,
    $ExitGUIException as ExitGUIException,
    $TextEditor as TextEditor,
    $ImageConversion as ImageConversion,
    $TouchPhase as TouchPhase,
    $IMECompositionMode as IMECompositionMode,
    $TouchType as TouchType,
    $Touch as Touch,
    $DeviceOrientation as DeviceOrientation,
    $AccelerationEvent as AccelerationEvent,
    $Gyroscope as Gyroscope,
    $LocationInfo as LocationInfo,
    $LocationServiceStatus as LocationServiceStatus,
    $LocationService as LocationService,
    $Compass as Compass,
    $Input as Input,
    $JsonUtility as JsonUtility,
    $LocalizationAsset as LocalizationAsset,
    $ParticleSystemEmissionType as ParticleSystemEmissionType,
    $ParticleSystem as ParticleSystem,
    $ParticleSystemSimulationSpace as ParticleSystemSimulationSpace,
    $ParticleSystemScalingMode as ParticleSystemScalingMode,
    $ParticleSystemCustomData as ParticleSystemCustomData,
    $ParticleSystemStopBehavior as ParticleSystemStopBehavior,
    $ParticleSystemCurveMode as ParticleSystemCurveMode,
    $ParticleSystemEmitterVelocityMode as ParticleSystemEmitterVelocityMode,
    $ParticleSystemStopAction as ParticleSystemStopAction,
    $ParticleSystemRingBufferMode as ParticleSystemRingBufferMode,
    $ParticleSystemCullingMode as ParticleSystemCullingMode,
    $ParticleSystemShapeType as ParticleSystemShapeType,
    $ParticleSystemShapeMultiModeValue as ParticleSystemShapeMultiModeValue,
    $ParticleSystemMeshShapeType as ParticleSystemMeshShapeType,
    $MeshRenderer as MeshRenderer,
    $ParticleSystemShapeTextureChannel as ParticleSystemShapeTextureChannel,
    $ParticleSystemSubEmitterType as ParticleSystemSubEmitterType,
    $ParticleSystemSubEmitterProperties as ParticleSystemSubEmitterProperties,
    $ParticleSystemAnimationMode as ParticleSystemAnimationMode,
    $ParticleSystemAnimationTimeMode as ParticleSystemAnimationTimeMode,
    $ParticleSystemAnimationType as ParticleSystemAnimationType,
    $ParticleSystemAnimationRowMode as ParticleSystemAnimationRowMode,
    $ParticleSystemGradientMode as ParticleSystemGradientMode,
    $ParticleSystemInheritVelocityMode as ParticleSystemInheritVelocityMode,
    $ParticleSystemGameObjectFilter as ParticleSystemGameObjectFilter,
    $ParticleSystemForceField as ParticleSystemForceField,
    $ParticleSystemNoiseQuality as ParticleSystemNoiseQuality,
    $ParticleSystemCollisionType as ParticleSystemCollisionType,
    $ParticleSystemCollisionMode as ParticleSystemCollisionMode,
    $ParticleSystemCollisionQuality as ParticleSystemCollisionQuality,
    $ParticleSystemOverlapAction as ParticleSystemOverlapAction,
    $ParticleSystemTrailMode as ParticleSystemTrailMode,
    $ParticleSystemTrailTextureMode as ParticleSystemTrailTextureMode,
    $ParticleSystemCustomDataMode as ParticleSystemCustomDataMode,
    $ParticlePhysicsExtensions as ParticlePhysicsExtensions,
    $ParticleCollisionEvent as ParticleCollisionEvent,
    $ParticleSystemTriggerEventType as ParticleSystemTriggerEventType,
    $ParticleSystemRenderMode as ParticleSystemRenderMode,
    $ParticleSystemSortMode as ParticleSystemSortMode,
    $ParticleSystemRenderSpace as ParticleSystemRenderSpace,
    $ParticleSystemVertexStream as ParticleSystemVertexStream,
    $ParticleSystemForceFieldShape as ParticleSystemForceFieldShape,
    $ParticleSystemVertexStreams as ParticleSystemVertexStreams,
    $RigidbodyConstraints as RigidbodyConstraints,
    $ForceMode as ForceMode,
    $JointDriveMode as JointDriveMode,
    $JointProjectionMode as JointProjectionMode,
    $MeshColliderCookingOptions as MeshColliderCookingOptions,
    $WheelFrictionCurve as WheelFrictionCurve,
    $SoftJointLimit as SoftJointLimit,
    $SoftJointLimitSpring as SoftJointLimitSpring,
    $JointDrive as JointDrive,
    $RigidbodyInterpolation as RigidbodyInterpolation,
    $JointMotor as JointMotor,
    $JointSpring as JointSpring,
    $JointLimits as JointLimits,
    $ControllerColliderHit as ControllerColliderHit,
    $CharacterController as CharacterController,
    $Rigidbody as Rigidbody,
    $PhysicMaterialCombine as PhysicMaterialCombine,
    $Collision as Collision,
    $ContactPoint as ContactPoint,
    $CollisionFlags as CollisionFlags,
    $QueryTriggerInteraction as QueryTriggerInteraction,
    $CollisionDetectionMode as CollisionDetectionMode,
    $ConfigurableJointMotion as ConfigurableJointMotion,
    $RotationDriveMode as RotationDriveMode,
    $PhysicMaterial as PhysicMaterial,
    $RaycastHit as RaycastHit,
    $MeshCollider as MeshCollider,
    $BoxCollider as BoxCollider,
    $ConstantForce as ConstantForce,
    $Joint as Joint,
    $HingeJoint as HingeJoint,
    $SpringJoint as SpringJoint,
    $FixedJoint as FixedJoint,
    $CharacterJoint as CharacterJoint,
    $ConfigurableJoint as ConfigurableJoint,
    $PhysicsScene as PhysicsScene,
    $PhysicsSceneExtensions as PhysicsSceneExtensions,
    $Physics as Physics,
    $RaycastCommand as RaycastCommand,
    $SpherecastCommand as SpherecastCommand,
    $CapsulecastCommand as CapsulecastCommand,
    $BoxcastCommand as BoxcastCommand,
    $PhysicsScene2D as PhysicsScene2D,
    $RaycastHit2D as RaycastHit2D,
    $ContactFilter2D as ContactFilter2D,
    $CapsuleDirection2D as CapsuleDirection2D,
    $Collider2D as Collider2D,
    $PhysicsSceneExtensions2D as PhysicsSceneExtensions2D,
    $Physics2D as Physics2D,
    $PhysicsJobOptions2D as PhysicsJobOptions2D,
    $ColliderDistance2D as ColliderDistance2D,
    $Rigidbody2D as Rigidbody2D,
    $ContactPoint2D as ContactPoint2D,
    $RigidbodyConstraints2D as RigidbodyConstraints2D,
    $RigidbodyInterpolation2D as RigidbodyInterpolation2D,
    $RigidbodySleepMode2D as RigidbodySleepMode2D,
    $CollisionDetectionMode2D as CollisionDetectionMode2D,
    $RigidbodyType2D as RigidbodyType2D,
    $ForceMode2D as ForceMode2D,
    $JointLimitState2D as JointLimitState2D,
    $EffectorSelection2D as EffectorSelection2D,
    $EffectorForceMode2D as EffectorForceMode2D,
    $Collision2D as Collision2D,
    $JointAngleLimits2D as JointAngleLimits2D,
    $JointTranslationLimits2D as JointTranslationLimits2D,
    $JointMotor2D as JointMotor2D,
    $JointSuspension2D as JointSuspension2D,
    $PhysicsMaterial2D as PhysicsMaterial2D,
    $CompositeCollider2D as CompositeCollider2D,
    $CircleCollider2D as CircleCollider2D,
    $CapsuleCollider2D as CapsuleCollider2D,
    $EdgeCollider2D as EdgeCollider2D,
    $BoxCollider2D as BoxCollider2D,
    $PolygonCollider2D as PolygonCollider2D,
    $Joint2D as Joint2D,
    $AnchoredJoint2D as AnchoredJoint2D,
    $SpringJoint2D as SpringJoint2D,
    $DistanceJoint2D as DistanceJoint2D,
    $FrictionJoint2D as FrictionJoint2D,
    $HingeJoint2D as HingeJoint2D,
    $RelativeJoint2D as RelativeJoint2D,
    $SliderJoint2D as SliderJoint2D,
    $TargetJoint2D as TargetJoint2D,
    $FixedJoint2D as FixedJoint2D,
    $WheelJoint2D as WheelJoint2D,
    $Effector2D as Effector2D,
    $AreaEffector2D as AreaEffector2D,
    $BuoyancyEffector2D as BuoyancyEffector2D,
    $PointEffector2D as PointEffector2D,
    $PlatformEffector2D as PlatformEffector2D,
    $SurfaceEffector2D as SurfaceEffector2D,
    $PhysicsUpdateBehaviour2D as PhysicsUpdateBehaviour2D,
    $ConstantForce2D as ConstantForce2D,
    $ScreenCapture as ScreenCapture,
    $SpriteMask as SpriteMask,
    $StreamingController as StreamingController,
    $ProceduralProcessorUsage as ProceduralProcessorUsage,
    $ProceduralCacheSize as ProceduralCacheSize,
    $ProceduralLoadingBehavior as ProceduralLoadingBehavior,
    $ProceduralPropertyType as ProceduralPropertyType,
    $ProceduralOutputType as ProceduralOutputType,
    $ISubsystemDescriptor as ISubsystemDescriptor,
    $ISubsystem as ISubsystem,
    $IntegratedSubsystemDescriptor as IntegratedSubsystemDescriptor,
    $ISubsystemDescriptorImpl as ISubsystemDescriptorImpl,
    $SubsystemDescriptor as SubsystemDescriptor,
    $SubsystemManager as SubsystemManager,
    $IntegratedSubsystem as IntegratedSubsystem,
    $Subsystem as Subsystem,
    $TerrainChangedFlags as TerrainChangedFlags,
    $TerrainRenderFlags as TerrainRenderFlags,
    $Terrain as Terrain,
    $TerrainData as TerrainData,
    $TreeInstance as TreeInstance,
    $TerrainExtensions as TerrainExtensions,
    $Tree as Tree,
    $TreePrototype as TreePrototype,
    $DetailRenderMode as DetailRenderMode,
    $DetailPrototype as DetailPrototype,
    $SplatPrototype as SplatPrototype,
    $PatchExtents as PatchExtents,
    $TerrainHeightmapSyncControl as TerrainHeightmapSyncControl,
    $TerrainLayer as TerrainLayer,
    $TerrainCollider as TerrainCollider,
    $TextGenerationSettings as TextGenerationSettings,
    $VerticalWrapMode as VerticalWrapMode,
    $HorizontalWrapMode as HorizontalWrapMode,
    $TextGenerator as TextGenerator,
    $UICharInfo as UICharInfo,
    $UILineInfo as UILineInfo,
    $UIVertex as UIVertex,
    $TextAlignment as TextAlignment,
    $TextMesh as TextMesh,
    $CharacterInfo as CharacterInfo,
    $CustomGridBrushAttribute as CustomGridBrushAttribute,
    $GridBrushBase as GridBrushBase,
    $ICanvasRaycastFilter as ICanvasRaycastFilter,
    $CanvasGroup as CanvasGroup,
    $CanvasRenderer as CanvasRenderer,
    $RectTransformUtility as RectTransformUtility,
    $Canvas as Canvas,
    $RenderMode as RenderMode,
    $AdditionalCanvasShaderChannels as AdditionalCanvasShaderChannels,
    $UISystemProfilerApi as UISystemProfilerApi,
    $RemoteSettings as RemoteSettings,
    $RemoteConfigSettings as RemoteConfigSettings,
    $WWWForm as WWWForm,
    $WWWAudioExtensions as WWWAudioExtensions,
    $WWW as WWW,
    $MovieTexture as MovieTexture,
    $WheelHit as WheelHit,
    $WheelCollider as WheelCollider,
    $WebGLInput as WebGLInput,
    $WindZoneMode as WindZoneMode,
    $WindZone as WindZone,

    $Playables as Playables,
    $Animations as Animations,
    $Audio as Audio,
    $Internal as Internal,
    $Events as Events,
    $Rendering as Rendering,
    $SceneManagement as SceneManagement,
    $ADBannerView as ADBannerView,
    $ADInterstitialAd as ADInterstitialAd,
    $Scripting as Scripting,
    $SocialPlatforms as SocialPlatforms,
    $UI as UI,
    $EventSystems as EventSystems,
    $Experimental as Experimental,
}

