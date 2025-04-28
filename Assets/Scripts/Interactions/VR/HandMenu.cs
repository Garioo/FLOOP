using Unity.XR.CoreUtils;
using Unity.XR.CoreUtils.Bindings;
using Unity.XR.CoreUtils.Bindings.Variables;
using UnityEngine.Assertions;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables.Primitives;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables.SmartTweenableVariables;
using UnityEngine.InputSystem;
using UnityEngine;

namespace UnityEngine.XR.Interaction.Toolkit.UI.BodyUI
{
    [AddComponentMenu("XR/Hand Menu", 22)]
    public class HandMenu : MonoBehaviour
    {
        public enum UpDirection { WorldUp, TransformUp, CameraUp }
        public enum MenuHandedness { None, Left, Right, Either }

        [SerializeField] GameObject m_HandMenuUIGameObject;
        [SerializeField] MenuHandedness m_MenuHandedness = MenuHandedness.Either;
        [SerializeField] UpDirection m_HandMenuUpDirection = UpDirection.TransformUp;
        [SerializeField] Transform m_LeftPalmAnchor;
        [SerializeField] Transform m_RightPalmAnchor;
        [SerializeField] InputActionReference m_OpenMenuAction;
        [SerializeField] FollowPresetDatumProperty m_HandTrackingFollowPreset;
        [SerializeField] FollowPresetDatumProperty m_ControllerFollowPreset;

        [Header("Menu Offset Settings")]
        [SerializeField, Range(0f, 0.5f)] float m_MenuVerticalOffset = 0.15f;
        [SerializeField, Range(-45f, 45f)] float m_MenuTiltAngle = 15f;

        readonly SmartFollowVector3TweenableVariable m_HandAnchorSmartFollow = new SmartFollowVector3TweenableVariable();
        readonly QuaternionTweenableVariable m_RotTweenFollow = new QuaternionTweenableVariable();
        readonly Vector3TweenableVariable m_MenuScaleTweenable = new Vector3TweenableVariable();
        readonly BindingsGroup m_BindingsGroup = new BindingsGroup();
        readonly BindableVariable<bool> m_MenuVisibleBindableVariable = new BindableVariable<bool>(false);

        Transform m_CameraTransform;
        Transform m_LeftOffsetRoot, m_RightOffsetRoot;
        Transform m_LastValidCameraTransform, m_LastValidPalmAnchor, m_LastValidPalmAnchorOffset;

        Vector3 m_InitialMenuLocalScale = Vector3.one;
        MenuHandedness m_LastHandThatMetRequirements = MenuHandedness.Left;
        XRInputModalityManager.InputMode m_CurrentInputMode = XRInputModalityManager.InputMode.None;
        bool m_WasMenuHiddenLastFrame = true;

        protected void Awake()
        {
            m_HandAnchorSmartFollow.minDistanceAllowed = 0.005f;
            m_HandAnchorSmartFollow.maxDistanceAllowed = 0.03f;
            m_HandAnchorSmartFollow.minToMaxDelaySeconds = 1f;

            m_RightOffsetRoot = new GameObject("Right Offset Root").transform;
            m_RightOffsetRoot.SetParent(m_RightPalmAnchor);
            m_RightOffsetRoot.localPosition = new Vector3(0f, m_MenuVerticalOffset, 0f);
            m_RightOffsetRoot.localRotation = Quaternion.Euler(m_MenuTiltAngle, 0f, 0f);

            m_LeftOffsetRoot = new GameObject("Left Offset Root").transform;
            m_LeftOffsetRoot.SetParent(m_LeftPalmAnchor);
            m_LeftOffsetRoot.localPosition = new Vector3(0f, m_MenuVerticalOffset, 0f);
            m_LeftOffsetRoot.localRotation = Quaternion.Euler(m_MenuTiltAngle, 0f, 0f);
        }

        protected void OnEnable()
        {
            if (m_OpenMenuAction != null)
                m_OpenMenuAction.action.Enable();

            m_HandAnchorSmartFollow.Value = m_HandMenuUIGameObject.transform.position;
            m_BindingsGroup.AddBinding(m_HandAnchorSmartFollow.Subscribe(newPosition => m_HandMenuUIGameObject.transform.position = newPosition));

            m_RotTweenFollow.Value = m_HandMenuUIGameObject.transform.rotation;
            m_BindingsGroup.AddBinding(m_RotTweenFollow.Subscribe(newRot => m_HandMenuUIGameObject.transform.rotation = newRot));

            m_InitialMenuLocalScale = m_HandMenuUIGameObject.transform.localScale;
            m_MenuScaleTweenable.Value = m_InitialMenuLocalScale;
            m_BindingsGroup.AddBinding(m_MenuScaleTweenable.Subscribe(value => m_HandMenuUIGameObject.transform.localScale = value));

            m_BindingsGroup.AddBinding(XRInputModalityManager.currentInputMode.SubscribeAndUpdate(OnInputModeChanged));

            m_MenuVisibleBindableVariable.Value = false;
            m_BindingsGroup.AddBinding(m_MenuVisibleBindableVariable.SubscribeAndUpdate(value =>
            {
                m_HandMenuUIGameObject.SetActive(value);
            }));
        }

        protected void OnDisable()
        {
            if (m_OpenMenuAction != null)
                m_OpenMenuAction.action.Disable();

            m_BindingsGroup.Clear();
        }

        protected void LateUpdate()
        {
            // --- NEW: Live update offsets if changed in inspector ---
            if (m_LeftOffsetRoot != null)
            {
                m_LeftOffsetRoot.localPosition = new Vector3(0f, m_MenuVerticalOffset, 0f);
                m_LeftOffsetRoot.localRotation = Quaternion.Euler(m_MenuTiltAngle, 0f, 0f);
            }
            if (m_RightOffsetRoot != null)
            {
                m_RightOffsetRoot.localPosition = new Vector3(0f, m_MenuVerticalOffset, 0f);
                m_RightOffsetRoot.localRotation = Quaternion.Euler(m_MenuTiltAngle, 0f, 0f);
            }

            // --- Keep your current code ---
            if (m_OpenMenuAction != null && m_OpenMenuAction.action.triggered)
            {
                m_MenuVisibleBindableVariable.Value = !m_MenuVisibleBindableVariable.Value;
                return;
            }

            if (m_CurrentInputMode == XRInputModalityManager.InputMode.None)
            {
                m_MenuVisibleBindableVariable.Value = false;
                return;
            }

            bool showMenu = false;
            var currentPreset = GetCurrentPreset();
            if (TryGetTrackedAnchors(m_MenuHandedness, currentPreset, out var targetHandedness, out var cameraTransform, out var palmAnchor, out var palmAnchorOffset))
            {
                m_LastValidCameraTransform = cameraTransform;
                m_LastValidPalmAnchor = palmAnchor;
                m_LastValidPalmAnchorOffset = palmAnchorOffset;
            }

            if (!m_HandMenuUIGameObject.activeSelf)
                return;

            if (m_LastValidCameraTransform == null || m_LastValidPalmAnchor == null || m_LastValidPalmAnchorOffset == null)
                return;

            var palmAnchorOffsetPose = m_LastValidPalmAnchorOffset.GetWorldPose();
            m_HandAnchorSmartFollow.target = palmAnchorOffsetPose.position;
            m_RotTweenFollow.target = palmAnchorOffsetPose.rotation;

            if (m_WasMenuHiddenLastFrame || !currentPreset.allowSmoothing)
            {
                m_HandAnchorSmartFollow.HandleTween(1f);
                m_RotTweenFollow.HandleTween(1f);
            }
            else
            {
                m_HandAnchorSmartFollow.HandleSmartTween(Time.deltaTime, currentPreset.followLowerSmoothingValue, currentPreset.followUpperSmoothingValue);
                m_RotTweenFollow.HandleTween(Time.deltaTime * currentPreset.followLowerSmoothingValue);
            }
        }


        void OnInputModeChanged(XRInputModalityManager.InputMode newInputMode)
        {
            m_CurrentInputMode = newInputMode;
            GetCurrentPreset()?.ApplyPreset(m_LeftOffsetRoot, m_RightOffsetRoot);
        }

        FollowPreset GetCurrentPreset()
        {
            return m_CurrentInputMode == XRInputModalityManager.InputMode.MotionController ?
                m_ControllerFollowPreset.Value : m_HandTrackingFollowPreset.Value;
        }

        bool TryGetTrackedAnchors(MenuHandedness desiredHandedness, in FollowPreset currentPreset, out MenuHandedness targetHandedness, out Transform cameraTransform, out Transform palmAnchor, out Transform palmAnchorOffset)
        {
            palmAnchor = null;
            palmAnchorOffset = null;
            targetHandedness = MenuHandedness.None;

            if (!TryGetCamera(out cameraTransform) || desiredHandedness == MenuHandedness.None)
                return false;

            palmAnchor = desiredHandedness == MenuHandedness.Left ? m_LeftPalmAnchor : m_RightPalmAnchor;
            palmAnchorOffset = desiredHandedness == MenuHandedness.Left ? m_LeftOffsetRoot : m_RightOffsetRoot;
            targetHandedness = desiredHandedness;
            return true;
        }

        bool TryGetCamera(out Transform cameraTransform)
        {
            if (m_CameraTransform == null)
            {
                var mainCamera = Camera.main;
                if (mainCamera == null)
                {
                    cameraTransform = null;
                    return false;
                }
                m_CameraTransform = mainCamera.transform;
            }
            cameraTransform = m_CameraTransform;
            return true;
        }
    }
}
