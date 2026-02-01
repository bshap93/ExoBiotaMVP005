using FirstPersonPlayer;
using Manager.Global;
using UnityEngine;

namespace SharedUI.Hotbar
{
    /// <summary>
    ///     Connects the RewiredFirstPersonInputs to the Hotbar system
    /// </summary>
    [RequireComponent(typeof(RewiredFirstPersonInputs))]
    public class HotbarInputHandler : MonoBehaviour
    {
        [SerializeField] FPHUDHotbars fpHudHotbars;

        [Header("Mouse Wheel Settings")] [SerializeField]
        bool enableMouseWheelToolCycling = true;
        [SerializeField] float scrollThreshold = 0.1f; // Minimum scroll amount to register
        [SerializeField] float scrollCooldown = 0.2f; // Cooldown between scroll actions

        RewiredFirstPersonInputs _inputs;
        float _lastScrollTime;

        void Start()
        {
            _inputs = GetComponent<RewiredFirstPersonInputs>();

            if (_inputs == null) Debug.LogError("[HotbarInputHandler] RewiredFirstPersonInputs component not found!");

            if (fpHudHotbars == null)
            {
                fpHudHotbars = FindFirstObjectByType<FPHUDHotbars>();
                if (fpHudHotbars == null) Debug.LogError("[HotbarInputHandler] FPHUDHotbars not found in scene!");
            }
        }

        void Update()
        {
            if (_inputs == null || fpHudHotbars == null) return;

            // Handle mouse wheel tool cycling
            if (enableMouseWheelToolCycling) HandleMouseWheelCycling();

            // Check each hotbar key
            if (_inputs.hotbarFP1)
                fpHudHotbars.HandleHotbarKeyPress(1);
            else if (_inputs.hotbarFP2)
                fpHudHotbars.HandleHotbarKeyPress(2);
            else if (_inputs.hotbarFP3)
                fpHudHotbars.HandleHotbarKeyPress(3);
            else if (_inputs.hotbarFP4)
                fpHudHotbars.HandleHotbarKeyPress(4);
            else if (_inputs.hotbarFP5)
                fpHudHotbars.HandleHotbarKeyPress(5);
            else if (_inputs.hotbarFP6) fpHudHotbars.HandleHotbarKeyPress(6);
        }

        void HandleMouseWheelCycling()
        {
            var pauseManager = PauseManager.Instance;
            if (pauseManager != null && pauseManager.IsPaused()) return;
            var scrollDelta = _inputs.scrollBetweenTools;

            // Check if enough time has passed since last scroll and scroll amount is significant
            if (Mathf.Abs(scrollDelta) < scrollThreshold || Time.time - _lastScrollTime < scrollCooldown) return;

            _lastScrollTime = Time.time;

            if (scrollDelta > 0)
                // Scroll up - next tool
                fpHudHotbars.CycleToolHotbar(1);
            else if (scrollDelta < 0)
                // Scroll down - previous tool
                fpHudHotbars.CycleToolHotbar(-1);
        }
    }
}
