using System.Collections;
using Helpers.Events;
using Manager.Settings;
using Michsky.MUIP;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SharedUI
{
    public class GlobalSettingsUIController : MonoBehaviour, MMEventListener<MyUIEvent>
    {
        [Header("UI Elements")] [SerializeField]
        CustomDropdown resolutionDropdown;
        [FormerlySerializedAs("mouseSensitivitySliderComponent")] [SerializeField]
        Slider mouseXSensitivitySliderComponent;
        [SerializeField] Slider mouseYSensitivitySliderComponent;
        [SerializeField] float initialMaxMouseSensitivity = 2.0f;

        [SerializeField] MMFeedbacks onOpenFeedbacks;
        [SerializeField] MMFeedbacks onCloseFeedbacks;

        [SerializeField] CustomDropdown tutorialOnDropdown;


        [Header("Features Toggles")] [SerializeField]
        bool ditheringCanBeToggled;
        [SerializeField] ButtonManager ditherToggleButton;

        [SerializeField] Toggle autoSaveAtCheckpointToggle;

        CanvasGroup _canvasGroup;

        bool _isDitheringOn;

        void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();

            // Initialize to invisible
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        void Start()
        {
            SetupResolutionDropdown();
            SetupDitheringButton();
            SetupMouseSensitivitySlider();
            SetupGameplaySettings();
        }

        void OnEnable()
        {
            this.MMEventStartListening();
            mouseXSensitivitySliderComponent.onValueChanged.AddListener(OnMouseXSensitivitySliderChanged);
            mouseYSensitivitySliderComponent.onValueChanged.AddListener(OnMouseYSensitivitySliderChanged);
        }

        void OnDisable()
        {
            this.MMEventStopListening();
            mouseXSensitivitySliderComponent.onValueChanged.RemoveListener(OnMouseXSensitivitySliderChanged);
            mouseYSensitivitySliderComponent.onValueChanged.RemoveListener(OnMouseYSensitivitySliderChanged);
        }
        public void OnMMEvent(MyUIEvent eventType)
        {
            if (eventType.uiType == UIType.GlobalSettingsPanel)
            {
                if (eventType.uiActionType == UIActionType.Open)
                {
                    _canvasGroup.alpha = 1f;
                    _canvasGroup.interactable = true;
                    _canvasGroup.blocksRaycasts = true;
                }
                else if (eventType.uiActionType == UIActionType.Close)
                {
                    _canvasGroup.alpha = 0f;
                    _canvasGroup.interactable = false;
                    _canvasGroup.blocksRaycasts = false;
                }
            }
            else if (eventType.uiActionType == UIActionType.Close) // Close any open panels on other UI types
            {
                _canvasGroup.alpha = 0f;
                _canvasGroup.interactable = false;
                _canvasGroup.blocksRaycasts = false;
            }
        }

        void SetupDitheringButton()
        {
            if (ditheringCanBeToggled)
            {
                ditherToggleButton.gameObject.SetActive(true);
                var gsm = GlobalSettingsManager.Instance;
                _isDitheringOn = gsm.DitheringEnabled;

                ditherToggleButton.onClick.AddListener(OnDitheringToggleButtonPressed);
            }
            else
            {
                ditherToggleButton.gameObject.SetActive(false);
            }
        }

        void SetupMouseSensitivitySlider()
        {
            var gsm = GlobalSettingsManager.Instance;
            if (gsm == null || mouseXSensitivitySliderComponent == null)
            {
                Debug.LogWarning("GlobalSettingsManager or mouseSensitivitySliderComponent missing!");
                return;
            }

            // FIX: Set minValue to 0.1f (not 0.0f) to match valid range
            mouseXSensitivitySliderComponent.minValue = 0.1f;
            mouseXSensitivitySliderComponent.maxValue = initialMaxMouseSensitivity;
            mouseXSensitivitySliderComponent.value = gsm.MouseXSensitivity;

            mouseYSensitivitySliderComponent.minValue = 0.1f;
            mouseYSensitivitySliderComponent.maxValue = initialMaxMouseSensitivity;
            mouseYSensitivitySliderComponent.value = gsm.MouseYSensitivity;
        }


        void SetupResolutionDropdown()
        {
            var gsm = GlobalSettingsManager.Instance;
            if (gsm == null || resolutionDropdown == null)
            {
                Debug.LogWarning("GlobalSettingsManager or resolutionDropdown missing!");
                return;
            }

            // Clear any existing items in the dropdown
            resolutionDropdown.items.Clear();

            // Populate dropdown with resolutions
            for (var i = 0; i < gsm.chooseableResolutions.Count; i++)
            {
                var res = gsm.chooseableResolutions[i];
                var label = $"{res.width} x {res.height}";
                resolutionDropdown.CreateNewItem(label);
            }

            // Refresh UI to apply changes
            resolutionDropdown.SetupDropdown();

            // Handle value change event
            resolutionDropdown.onValueChanged.RemoveAllListeners();
            resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);

            // Optionally set current resolution
            var current = Screen.currentResolution;
            var index = gsm.chooseableResolutions.FindIndex(r =>
                r.width == current.width && r.height == current.height);

            if (index >= 0) resolutionDropdown.SetDropdownIndex(index);
        }

        // Setup CustomDropdown with only two options: On and Off
        void SetupGameplaySettings()
        {
            var gsm = GlobalSettingsManager.Instance;
            if (gsm == null || resolutionDropdown == null)
            {
                Debug.LogWarning("GlobalSettingsManager or resolutionDropdown missing!");
                return;
            }

            // Tutorial On/Off
            if (!gsm.IsTutorialOn)
                tutorialOnDropdown.SetDropdownIndex(1); // Off
            else
                tutorialOnDropdown.SetDropdownIndex(0); // On


            tutorialOnDropdown.onValueChanged.RemoveAllListeners();
            tutorialOnDropdown.onValueChanged.AddListener(index =>
            {
                GlobalSettingsEvent.Trigger(GlobalSettingsEventType.TutorialOnChanged, index);
            });

            // Auto Save at Checkpoint Toggle
            if (autoSaveAtCheckpointToggle != null)
            {
                autoSaveAtCheckpointToggle.isOn = gsm.AutoSaveAtCheckpoints;
                autoSaveAtCheckpointToggle.onValueChanged.RemoveAllListeners();
                autoSaveAtCheckpointToggle.onValueChanged.AddListener(isOn =>
                {
                    GlobalSettingsEvent.Trigger(GlobalSettingsEventType.AutoSaveAtCheckpointsChanged, isOn ? 0 : 1);
                });
            }
        }


        void OnResolutionChanged(int index)
        {
            GlobalSettingsEvent.Trigger(GlobalSettingsEventType.ResolutionChanged, index);
        }

        void OnMouseXSensitivitySliderChanged(float newValue)
        {
            GlobalSettingsEvent.Trigger(GlobalSettingsEventType.MouseXSensitivityChanged, newValue);
        }

        void OnMouseYSensitivitySliderChanged(float newValue)
        {
            GlobalSettingsEvent.Trigger(GlobalSettingsEventType.MouseYSensitivityChanged, newValue);
        }

        public void OnExitSettingsButtonPressed()
        {
            onCloseFeedbacks?.PlayFeedbacks();
            // MyUIEvent.Trigger(UIType.GlobalSettingsPanel, UIActionType.Close);
            // PauseEvent.Trigger(PauseEventType.PauseOff);
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        IEnumerator MakeCursorVisibleAndUnlocked()
        {
            // Wait for end of frame to ensure UI has closed
            // wait 1 second to ensure any transitions are complete
            yield return new WaitForSeconds(1.0f);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        void OnDitheringToggleButtonPressed()
        {
            _isDitheringOn = !_isDitheringOn;
            GlobalSettingsEvent.Trigger(GlobalSettingsEventType.DitheringToggled, _isDitheringOn ? 1 : 0);
        }
    }
}
