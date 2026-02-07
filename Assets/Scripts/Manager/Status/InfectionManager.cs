using System;
using System.Collections.Generic;
using System.Linq;
using Helpers.Events;
using Helpers.Events.Status;
using Helpers.Interfaces;
using Helpers.ScriptableObjects;
using Manager.Status.Scriptable;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Manager.Status
{
    public class InfectionManager : MonoBehaviour, ICoreGameService, MMEventListener<StatsStatusEvent>,
        MMEventListener<InGameTimeUpdateEvent>
    {
        [Title("Base Infection Sites")] [TableList(ShowIndexLabels = true)]
        public List<InfectionObject> infectionSites = new()
        {
            new InfectionObject { site = InfectionSite.Skin01, baseProbability = 0.6f },
            new InfectionObject { site = InfectionSite.Heart01, baseProbability = 0.05f },
            new InfectionObject { site = InfectionSite.Lungs01, baseProbability = 0.1f },
            new InfectionObject { site = InfectionSite.Brain01, baseProbability = 0.05f },
            new InfectionObject { site = InfectionSite.Eyes01, baseProbability = 0.2f }
        };
        public bool autosave;


        [SerializeField] int initialMinutesUntilNextInfection = 50;
        public StatusEffectIconRepository iconRepository;


        bool _dirty;

        bool _wasContaminationMaxed;

        int _lastKnownMinutesElapsed;
        int _minutesPerInfection;

        int _minutesUntilNextInfection;

        string _savePath;

        public List<OngoingInfection> OngoingInfections { get; private set; } = new();

        public static InfectionManager Instance { get; private set; }


        [Title("Current Configuration")]
        [ShowInInspector]
        [ReadOnly]
        [TableList(ShowIndexLabels = true)]
        List<InfectionObject> CurrentProbabilities
        {
            get
            {
                var display = new List<InfectionObject>();
                foreach (var site in infectionSites)
                    display.Add(
                        new InfectionObject
                        {
                            baseProbability = site.currentProbability
                        });

                return display;
            }
        }


        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }


        void Start()
        {
            if (_minutesPerInfection == 0)
                _minutesPerInfection = initialMinutesUntilNextInfection;


            _minutesUntilNextInfection = _minutesPerInfection;
        }

        void OnEnable()
        {
            this.MMEventStartListening<StatsStatusEvent>();
            this.MMEventStartListening<InGameTimeUpdateEvent>();
        }

        void OnDisable()
        {
            this.MMEventStopListening<StatsStatusEvent>();
            this.MMEventStopListening<InGameTimeUpdateEvent>();
        }
        public void Save()
        {
            _savePath = GetSaveFilePath();
            ES3.Save("OngoingInfections", OngoingInfections, _savePath);
            ES3.Save("IsContaminationMaxed", _wasContaminationMaxed, _savePath);
            ES3.Save("MinutesUntilNextInfection", _minutesUntilNextInfection, _savePath);
            ES3.Save("MinutesPerInfection", _minutesPerInfection, _savePath);
            ES3.Save("LastKnownMinutesElapsed", _lastKnownMinutesElapsed, _savePath);
        }
        public void Load()
        {
            _savePath = GetSaveFilePath();
            if (ES3.KeyExists("OngoingInfections", _savePath))
                OngoingInfections = ES3.Load<List<OngoingInfection>>("OngoingInfections", _savePath);

            if (ES3.KeyExists("IsContaminationMaxed", _savePath))
                _wasContaminationMaxed = ES3.Load<bool>("IsContaminationMaxed", _savePath);

            if (ES3.KeyExists("MinutesUntilNextInfection", _savePath))
                _minutesUntilNextInfection = ES3.Load<int>("MinutesUntilNextInfection", _savePath);

            if (ES3.KeyExists("MinutesPerInfection", _savePath))
                _minutesPerInfection = ES3.Load<int>("MinutesPerInfection", _savePath);

            if (ES3.KeyExists("LastKnownMinutesElapsed", _savePath))
                _lastKnownMinutesElapsed = ES3.Load<int>("LastKnownMinutesElapsed", _savePath);

            ReconstructProbabilities();
        }
        public void Reset()
        {
            OngoingInfections.Clear();
            _wasContaminationMaxed = false;
            _minutesUntilNextInfection = initialMinutesUntilNextInfection;
            _minutesPerInfection = initialMinutesUntilNextInfection;
            _lastKnownMinutesElapsed = 0;

            InitializeProbabilities();

            Save();
        }
        public void ConditionalSave()
        {
            if (autosave && _dirty)
            {
                Save();
                _dirty = false;
            }
        }
        public void MarkDirty()
        {
            _dirty = true;
        }
        public string GetSaveFilePath()
        {
            return SaveManager.Instance.GetGlobalSaveFilePath(GlobalManagerType.InfectionManagerSave);
        }
        public void CommitCheckpointSave()
        {
            if (_dirty)
            {
                Save();
                _dirty = false;
            }
        }
        public bool HasSavedData()
        {
            return ES3.FileExists(_savePath ?? GetSaveFilePath());
        }
        public void OnMMEvent(InGameTimeUpdateEvent eventType)
        {
            // _isContaminationMaxed = PlayerMutableStatsManager.Instance.IsContaminationMaxed();
            if (_wasContaminationMaxed)
            {
                OngoingInfection newInfection;
                int minutesPassed;
                if (_lastKnownMinutesElapsed == 0) // First update after load
                    minutesPassed = 0;
                else if (eventType.MinutesElapsed != _lastKnownMinutesElapsed)
                    minutesPassed = eventType.MinutesElapsed - _lastKnownMinutesElapsed;
                else
                    minutesPassed = 0;

                _lastKnownMinutesElapsed = eventType.MinutesElapsed;

                _minutesUntilNextInfection -= minutesPassed; // actual time passed
                if (_minutesUntilNextInfection <= 0)
                {
                    newInfection = RollForNewInfection();
                    InfectionUIEvent.Trigger(
                        _minutesUntilNextInfection, _minutesPerInfection, newInfection);
                    // _wasContaminationMaxed = PlayerMutableStatsManager.Instance.IsContaminationMaxed();
                }
                else
                {
                    InfectionUIEvent.Trigger(
                        _minutesUntilNextInfection, _minutesPerInfection);
                }
            }
        }
        public void OnMMEvent(StatsStatusEvent eventType)
        {
            if (eventType.StatType == StatsStatusEvent.StatsStatusType.Contamination &&
                eventType.Status == StatsStatusEvent.StatsStatus.IsMax)
            {
                _wasContaminationMaxed = eventType.Enabled;
            }
            else if (eventType.StatType == StatsStatusEvent.StatsStatusType.Contamination &&
                     eventType.Status == StatsStatusEvent.StatsStatus.IsMin)
            {
                _wasContaminationMaxed = false;
                Decontaminate();
            }
        }

        void Decontaminate()
        {
            foreach (var infection in OngoingInfections.ToList())
                if (infection.canBeRemovedViaDecontamination)
                {
                    OngoingInfections.Remove(infection);

                    InfectionUIEvent.Trigger(0, _minutesPerInfection, infection, false);

                    var site = infectionSites.Find(s => s.infectionSiteID == infection.infectionSiteID);

                    PlayerStatusEffectEvent.Trigger(
                        PlayerStatusEffectEvent.StatusEffectEventType.Remove,
                        site.statusEffectOfInfection.effectID, null,
                        PlayerStatusEffectEvent.DirectionOfEvent.Inbound,
                        StatusEffect.StatusEffectKind.MinorInfections);
                }

            ReconstructProbabilities();
            MarkDirty();
        }
        void ReconstructProbabilities()
        {
            InitializeProbabilities();

            var infectedSites = new HashSet<string>();
            foreach (var infection in OngoingInfections) infectedSites.Add(infection.infectionSiteID);

            foreach (var site in infectionSites)
                if (infectedSites.Contains(site.infectionSiteID))
                    site.currentProbability = 0f;


            NormalizeRemainingProbabilities();
        }

        void InitializeProbabilities()
        {
            foreach (var site in infectionSites) site.currentProbability = site.baseProbability;
        }

        OngoingInfection RollForNewInfection()
        {
            OngoingInfection newInfection = null;
            Debug.Log("Rolling for new infection");

            var eligibleSites = GetEligibleInfectionSites();

            if (eligibleSites.Count == 0)
            {
                Debug.Log("All infection sites are already infected!");
                _minutesUntilNextInfection = _minutesPerInfection;
                return null;
            }


            var selectedSite = SetWeightedRandomSite(eligibleSites);

            if (selectedSite != null)
            {
                newInfection = CreateNewInfection(selectedSite);
                selectedSite.currentProbability = 0f; // Set probability to 0 to prevent re-infection

                NormalizeRemainingProbabilities();
            }


            _minutesUntilNextInfection = _minutesPerInfection;
            return newInfection;
        }

        List<InfectionObject> GetEligibleInfectionSites()
        {
            // Only return sites that:
            // 1. Haven't been infected yet (currentProbability > 0)
            // 2. Don't already have an ongoing infection

            var alreadyInfectedSites = new HashSet<string>();
            foreach (var infection in OngoingInfections) alreadyInfectedSites.Add(infection.infectionSiteID);

            var eligible = new List<InfectionObject>();
            foreach (var site in infectionSites)
                if (site.currentProbability > 0f && !alreadyInfectedSites.Contains(site.site.ToString()))
                    eligible.Add(site);

            return eligible;
        }

        InfectionObject SetWeightedRandomSite(List<InfectionObject> eligibleSites)
        {
            var totalWeight = 0f;
            foreach (var site in eligibleSites) totalWeight += site.currentProbability;

            if (totalWeight <= 0f)
            {
                Debug.LogWarning("Infection probability is 0 or less");
                return null;
            }

            var randomValue = Random.Range(0f, totalWeight);

            var cumulativeWeight = 0f;
            foreach (var site in eligibleSites)
            {
                cumulativeWeight += site.currentProbability;
                if (randomValue <= cumulativeWeight) return site;
            }

            return eligibleSites[eligibleSites.Count - 1];
        }

        void NormalizeRemainingProbabilities()
        {
            var totalCurrentProbability = 0f;
            foreach (var site in infectionSites) totalCurrentProbability += site.currentProbability;

            if (totalCurrentProbability > 0f)
                foreach (var site in infectionSites)
                    if (site.currentProbability > 0f)
                        site.currentProbability = site.currentProbability / totalCurrentProbability;
        }

        public OngoingInfection CreateNewInfection(InfectionObject infectionObject)
        {
            var newInfection = new OngoingInfection
            {
                infectionID = Guid.NewGuid().ToString(),
                infectionSiteID = infectionObject.infectionSiteID,
                progressionTowardSupplantation = 0f,
                infectionName = infectionObject.infectionName,
                canBeRemovedViaDecontamination = infectionObject.canBeRemovedViaDecontamination
            };

            OngoingInfections.Add(newInfection);
            MarkDirty();

            // Apply Status Effect

            if (infectionObject.statusEffectOfInfection != null)
                PlayerStatusEffectEvent.Trigger(
                    PlayerStatusEffectEvent.StatusEffectEventType.Apply,
                    infectionObject.statusEffectOfInfection.effectID, null,
                    PlayerStatusEffectEvent.DirectionOfEvent.Inbound, StatusEffect.StatusEffectKind.None);

            // TODO Notify UI
            InfectionUIEvent.Trigger(
                _minutesUntilNextInfection, _minutesPerInfection, newInfection);

            Debug.Log($"New infection created: {newInfection.infectionSiteID}");
            return newInfection;
        }

        [Serializable]
        public class OngoingInfection
        {
            public string infectionName;
            public string infectionID;
            public string infectionSiteID;
            [Range(0f, 1f)] public float progressionTowardSupplantation;
            public bool canBeRemovedViaDecontamination;
        }

        [Serializable]
        public class InfectionInfo
        {
            public string infectionSiteID;
            public InfectionObject site;
        }
    }
}
