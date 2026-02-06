using System;
using System.Globalization;
using FirstPersonPlayer.UI.InventoryListView;
using Helpers.Events;
using Helpers.Events.Gated;
using Helpers.Events.Progression;
using Manager;
using Manager.ProgressionMangers;
using Manager.UI;
using Michsky.MUIP;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using SharedUI.Progression;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace SharedUI.Interact
{
    public class GatedLevelingUIController : MonoBehaviour, MMEventListener<MyUIEvent>, MMEventListener<XPEvent>,
        MMEventListener<AttrPendingBuyEvent>, MMEventListener<OuterCoreXPEvent>
    {
        [Serializable]
        public enum MediStatType
        {
            Standard,
            Infected
        }

        [FormerlySerializedAs("innerCoresDisplay")] [SerializeField]
        OuterCoresDisplay outerCoresDisplay;

        [SerializeField] MediStatType mediStatType = MediStatType.Standard;

        [Header(" Attribute Setters ")] [SerializeField]
        AttributePointSetter dexteritySetter;
        [SerializeField] AttributePointSetter mentalToughnessSetter;
        [SerializeField] AttributePointSetter agilitySetter;
        [SerializeField] AttributePointSetter strengthSetter;
        [SerializeField] AttributePointSetter exobioticSetter;
        [Header("Individual UI Elements")] [SerializeField]
        TMP_Text totalUnusedXPText;
        [SerializeField] WaitWhileInteractingOverlay waitOverlay;

        [Header("Feedbacks and Buttons")] [SerializeField]
        MMFeedbacks openFeedbacks;
        [SerializeField] MMFeedbacks addXPFeedbacks;
        [SerializeField] MMFeedbacks commitChangesFeedbacks;


        [Header(" Buttons ")] [SerializeField] ButtonManager commitButton;
        [SerializeField] ButtonManager cancelButton;

        [SerializeField] TMP_Text maxHealth;
        [SerializeField] TMP_Text maxStamina;
        [SerializeField] TMP_Text contaminationResistance;

        CanvasGroup _canvasGroup;

        int _currentUnusedXP;

        int _initialAgility;
        float _initialContaminationResistance;
        int _initialDexterity;
        int _initialExobiotic;

        float _initialMaxHealth;
        float _initialMaxStamina;
        int _initialMentalToughness;
        int _initialStrength;

        //Attributes
        int _pendingNewAgility;
        float _pendingNewContaminationResistance;
        int _pendingNewDexterity;
        int _pendingNewExobiotic;

        // Float Stats
        float _pendingNewMaxHealth;
        float _pendingNewMaxStamina;
        int _pendingNewStrength;

        int _pendingNewUnusedXP;
        public int CurrentUnusedXP
        {
            get => _currentUnusedXP;
            set
            {
                _currentUnusedXP = value;
                totalUnusedXPText.text = _currentUnusedXP.ToString();
            }
        }
        void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            // hide
            Hide();
        }

        void Start()
        {
            Initialize();
        }
        void OnEnable()
        {
            this.MMEventStartListening<MyUIEvent>();
            this.MMEventStartListening<XPEvent>();
            this.MMEventStartListening<AttrPendingBuyEvent>();
            this.MMEventStartListening<OuterCoreXPEvent>();
        }
        void OnDisable()
        {
            this.MMEventStopListening<MyUIEvent>();
            this.MMEventStopListening<XPEvent>();
            this.MMEventStopListening<AttrPendingBuyEvent>();
            this.MMEventStopListening<OuterCoreXPEvent>();
        }
        public void OnMMEvent(AttrPendingBuyEvent eventType)
        {
            var attributeManager = AttributesManager.Instance;
            if (eventType.PendingBuyEventType == PendingBuyEventType.IncreasePendingAttribute)
            {
                var xpRequired = attributeManager.GetXpRequiredForLevel(eventType.AttrLevelTarget);
                if (_pendingNewUnusedXP -
                    xpRequired < 0)
                    return;

                _pendingNewUnusedXP = _pendingNewUnusedXP -
                                      xpRequired;

                totalUnusedXPText.text = _pendingNewUnusedXP.ToString();

                switch (eventType.AttributeType)
                {
                    case AttributeType.Dexterity:
                        _pendingNewDexterity = eventType.AttrLevelTarget;

                        dexteritySetter.Initialize(eventType.AttrLevelTarget, _pendingNewUnusedXP);
                        dexteritySetter.canDecrease = true;
                        break;

                    case AttributeType.Agility:
                        _pendingNewAgility = eventType.AttrLevelTarget;
                        _pendingNewMaxStamina += AttributesManager.Instance.GetStaminaPerAgilityIncrease();
                        UpdateFloatStatsUI();
                        agilitySetter.Initialize(eventType.AttrLevelTarget, _pendingNewUnusedXP);
                        agilitySetter.canDecrease = true;
                        break;
                    case AttributeType.Strength:
                        _pendingNewStrength = eventType.AttrLevelTarget;
                        _pendingNewMaxHealth += AttributesManager.Instance.GetHealthPerStrengthIncrease();
                        UpdateFloatStatsUI();
                        strengthSetter.Initialize(eventType.AttrLevelTarget, _pendingNewUnusedXP);
                        strengthSetter.canDecrease = true;
                        break;
                    case AttributeType.Exobiotic:
                        if (mediStatType == MediStatType.Infected)
                        {
                            _pendingNewExobiotic = eventType.AttrLevelTarget;
                            _pendingNewContaminationResistance +=
                                AttributesManager.Instance.GetContaminationResistPerExobioticIncrease();

                            UpdateFloatStatsUI();

                            exobioticSetter.Initialize(eventType.AttrLevelTarget, _pendingNewUnusedXP);
                            exobioticSetter.canDecrease = true;
                        }

                        break;
                }
            }
            else if (eventType.PendingBuyEventType == PendingBuyEventType.DecreasePendingAttribute)
            {
                switch (eventType.AttributeType)
                {
                    case AttributeType.Dexterity:
                        if (eventType.AttrLevelTarget < _initialDexterity) return;
                        break;
                    case AttributeType.MentalToughness:
                        if (eventType.AttrLevelTarget < _initialMentalToughness) return;
                        break;
                    case AttributeType.Agility:
                        if (eventType.AttrLevelTarget < _initialAgility) return;
                        break;
                    case AttributeType.Strength:
                        if (eventType.AttrLevelTarget < _initialStrength) return;
                        break;
                    case AttributeType.Exobiotic:
                        if (eventType.AttrLevelTarget < _initialExobiotic) return;
                        break;
                }

                _pendingNewUnusedXP += attributeManager.GetXpRequiredForLevel(eventType.AttrLevelTarget - 1);

                totalUnusedXPText.text = _pendingNewUnusedXP.ToString();


                switch (eventType.AttributeType)
                {
                    case AttributeType.Dexterity:
                        _pendingNewDexterity = eventType.AttrLevelTarget;
                        dexteritySetter.Initialize(eventType.AttrLevelTarget, _pendingNewUnusedXP);
                        dexteritySetter.canDecrease = eventType.AttrLevelTarget >= _initialDexterity;
                        break;

                    case AttributeType.Agility:
                        _pendingNewAgility = eventType.AttrLevelTarget;
                        agilitySetter.Initialize(eventType.AttrLevelTarget, _pendingNewUnusedXP);
                        _pendingNewMaxStamina -= AttributesManager.Instance.GetStaminaPerAgilityIncrease();
                        UpdateFloatStatsUI();
                        agilitySetter.canDecrease = eventType.AttrLevelTarget >= _initialAgility;

                        break;
                    case AttributeType.Strength:
                        _pendingNewStrength = eventType.AttrLevelTarget;
                        strengthSetter.Initialize(eventType.AttrLevelTarget, _pendingNewUnusedXP);
                        _pendingNewMaxHealth -= AttributesManager.Instance.GetHealthPerStrengthIncrease();
                        UpdateFloatStatsUI();
                        strengthSetter.canDecrease = eventType.AttrLevelTarget >= _initialStrength;
                        break;
                    case AttributeType.Exobiotic:
                        if (mediStatType == MediStatType.Infected)
                        {
                            _pendingNewExobiotic = eventType.AttrLevelTarget;
                            exobioticSetter.Initialize(eventType.AttrLevelTarget, _pendingNewUnusedXP);
                            _pendingNewContaminationResistance -=
                                AttributesManager.Instance.GetContaminationResistPerExobioticIncrease();

                            UpdateFloatStatsUI();

                            exobioticSetter.canDecrease = eventType.AttrLevelTarget >= _initialExobiotic;
                        }

                        break;
                }
            }
        }
        public void OnMMEvent(MyUIEvent eventType)
        {
            if (eventType.uiType == UIType.LevelingUIInfected && mediStatType == MediStatType.Infected)
            {
                if (eventType.uiActionType == UIActionType.Open)
                    Show();
                else if (eventType.uiActionType == UIActionType.Close) Hide();
            }

            if (eventType.uiType == UIType.LevelingUI && mediStatType == MediStatType.Standard)
            {
                if (eventType.uiActionType == UIActionType.Open)
                    Show();
                else if (eventType.uiActionType == UIActionType.Close) Hide();
            }
        }
        public void OnMMEvent(OuterCoreXPEvent eventType)
        {
            if (eventType.EventType == InnerCoreXPEventType.ConvertCoreToXP)
            {
                _currentUnusedXP = _currentUnusedXP +
                                   AttributesManager.Instance.GetXPGainedForCoreGrade(eventType.CoreGrade);

                _pendingNewUnusedXP = _currentUnusedXP;
                RefreshAttrSetters();
            }
        }
        public void OnMMEvent(XPEvent eventType)
        {
            if (eventType.EventType == XPEventType.SetUnusedXP)
            {
                if (CurrentUnusedXP > eventType.Amount)
                {
                    // spent XP
                }
                else
                {
                    // gained XP
                    addXPFeedbacks?.PlayFeedbacks();
                }

                CurrentUnusedXP = eventType.Amount;
                totalUnusedXPText.text = CurrentUnusedXP.ToString();
            }
        }
        void UpdateFloatStatsUI()
        {
            maxHealth.text = _pendingNewMaxHealth.ToString(CultureInfo.InvariantCulture);
            maxStamina.text = _pendingNewMaxStamina.ToString(CultureInfo.InvariantCulture);
            contaminationResistance.text = _pendingNewContaminationResistance.ToString(CultureInfo.InvariantCulture);
        }

        void Initialize()
        {
            var attributeManager = AttributesManager.Instance;
            var playerMutableStatsManager = PlayerMutableStatsManager.Instance;
            if (attributeManager == null)
            {
                Debug.LogError("AttributesManager instance not found!");
                return;
            }

            // Unused XP
            _currentUnusedXP = attributeManager.CurrentUnusedXP;
            _pendingNewUnusedXP = _currentUnusedXP;
            totalUnusedXPText.text = _pendingNewUnusedXP.ToString();

            // Inner Cores Display
            outerCoresDisplay.Refresh();

            RefreshAttrSetters();

            _initialAgility = attributeManager.Agility;
            _initialDexterity = attributeManager.Dexterity;
            _initialStrength = attributeManager.Strength;
            _initialExobiotic = attributeManager.Exobiotic;

            _initialMaxHealth = playerMutableStatsManager.CurrentMaxHealth;
            _initialMaxStamina = playerMutableStatsManager.CurrentMaxStamina;
            _initialContaminationResistance = playerMutableStatsManager.CurrentMaxContamination;

            _pendingNewAgility = _initialAgility;
            _pendingNewDexterity = _initialDexterity;
            _pendingNewStrength = _initialStrength;
            _pendingNewExobiotic = _initialExobiotic;

            _pendingNewMaxHealth = _initialMaxHealth;
            _pendingNewMaxStamina = _initialMaxStamina;
            _pendingNewContaminationResistance = _initialContaminationResistance;

            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(() => { CancelLeveling(); });
            commitButton.onClick.RemoveAllListeners();
            commitButton.onClick.AddListener(CommitChanges);
        }
        void RefreshAttrSetters()
        {
            var attributeManager = AttributesManager.Instance;
            // Attribute Setters
            dexteritySetter.Initialize(attributeManager.Dexterity, _pendingNewUnusedXP);
            agilitySetter.Initialize(attributeManager.Agility, _pendingNewUnusedXP);
            strengthSetter.Initialize(attributeManager.Strength, _pendingNewUnusedXP);
            if (exobioticSetter != null) exobioticSetter.Initialize(attributeManager.Exobiotic, _pendingNewUnusedXP);
        }

        void CancelLeveling()
        {
            // Logic to cancel attribute point changes
            Debug.Log("Canceled attribute point changes.");
            if (mediStatType == MediStatType.Infected)
                MyUIEvent.Trigger(UIType.LevelingUIInfected, UIActionType.Close);
            else if (mediStatType == MediStatType.Standard)
                MyUIEvent.Trigger(UIType.LevelingUI, UIActionType.Close);

            _currentUnusedXP = AttributesManager.Instance.CurrentUnusedXP;
            _pendingNewUnusedXP = _currentUnusedXP;
            _initialAgility = AttributesManager.Instance.Agility;
            _pendingNewAgility = _initialAgility;
            _initialDexterity = AttributesManager.Instance.Dexterity;
            _pendingNewDexterity = _initialDexterity;
            _initialStrength = AttributesManager.Instance.Strength;
            _pendingNewStrength = _initialStrength;
            _initialExobiotic = AttributesManager.Instance.Exobiotic;
            _pendingNewExobiotic = _initialExobiotic;

            GatedInteractionManager.Instance.isActiveGui = false;
        }

        void CommitChanges()
        {
            // Logic to commit attribute point changes
            if (_pendingNewDexterity == _initialDexterity &&
                _pendingNewAgility == _initialAgility &&
                _pendingNewStrength == _initialStrength && _pendingNewExobiotic == _initialExobiotic)

            {
                AlertEvent.Trigger(
                    AlertReason.Test, "No attribute changes to commit.", "Leveling");

                return;
            }

            Debug.Log("Committed attribute point changes.");

            if (mediStatType == MediStatType.Infected)
                MyUIEvent.Trigger(UIType.LevelingUIInfected, UIActionType.Close);
            else if (mediStatType == MediStatType.Standard)
                MyUIEvent.Trigger(UIType.LevelingUI, UIActionType.Close);

            // MyUIEvent.Trigger(UIType.WaitWhileInteracting, UIActionType.Open);
            // waitOverlay.Show("Applying Attribute Augments");
            AttributesManager.Instance.ApplyPendingAttributeChanges(
                _pendingNewDexterity,
                _pendingNewAgility,
                _pendingNewStrength,
                _pendingNewExobiotic);

            AttributesManager.Instance.ApplyPendingUnusedXP(_pendingNewUnusedXP);

            PlayerMutableStatsManager.Instance.ApplyPendingFloatStatChanges(
                _pendingNewMaxHealth,
                _pendingNewMaxStamina,
                _pendingNewContaminationResistance);

            commitChangesFeedbacks?.PlayFeedbacks();

            var newAttrValues = new NewAttributeValues
            {
                dexterity = _pendingNewDexterity,
                agility = _pendingNewAgility,
                strength = _pendingNewStrength,
                exobiotic = _pendingNewExobiotic
            };

            GatedLevelingEvent.Trigger(GatedInteractionEventType.CompleteInteraction, newAttrValues);
        }

        void Hide()
        {
            // hide
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
        void Show()
        {
            outerCoresDisplay.Refresh();
            Initialize();
            // show
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }
    }
}
