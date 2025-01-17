﻿
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using HFM.Core.WorkUnits;
using HFM.Log;
using HFM.Preferences;

namespace HFM.Core.Client
{
    // TODO: TimeFormatting should be a function of SlotModel, not view grid painting
    public enum TimeFormatting
    {
        None,
        Format1
    }

    public enum UnitTotalsType
    {
        All,
        ClientStart
    }

    public class SlotModel
    {
        #region IPreferenceSet

        public IPreferences Prefs => Client.Preferences;

        private PPDCalculation PPDCalculation => Prefs.Get<PPDCalculation>(Preference.PPDCalculation);

        private BonusCalculation BonusCalculation => Prefs.Get<BonusCalculation>(Preference.BonusCalculation);

        private bool ShowVersions => Prefs.Get<bool>(Preference.DisplayVersions);

        private int DecimalPlaces => Prefs.Get<int>(Preference.DecimalPlaces);

        internal bool ShowETADate => Prefs.Get<bool>(Preference.DisplayEtaAsDate);

        #endregion

        public WorkUnitModel WorkUnitModel { get; set; }

        public ClientSettings Settings => Client.Settings;

        public IClient Client { get; }

        public SlotModel(IClient client)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
            WorkUnitModel = new WorkUnitModel(this);

            Initialize();
        }

        private const int DefaultMachineID = 0;

        public void Initialize()
        {
            Arguments = String.Empty;
            MachineID = DefaultMachineID;
            // Status = 
            ClientVersion = String.Empty;
            TotalRunCompletedUnits = 0;
            TotalCompletedUnits = 0;
            TotalRunFailedUnits = 0;
        }

        #region Display Meta Data

        /// <summary>
        /// Client Startup Arguments
        /// </summary>
        public string Arguments { get; set; }

        private int _machineId;
        /// <summary>
        /// Machine ID associated with this client
        /// </summary>
        public int MachineID
        {
            // if SlotId is populated by a v7 client then also use it for the MachineId value
            get => SlotID > -1 ? SlotID : _machineId;
            set => _machineId = value;
        }

        #endregion

        #region Grid Properties

        /// <summary>
        /// Status of this client
        /// </summary>
        public SlotStatus Status { get; set; }

        public float Progress => ((float)PercentComplete) / 100;

        /// <summary>
        /// Current progress (percentage) of the unit
        /// </summary>
        public int PercentComplete => Status.IsRunning() || Status == SlotStatus.Paused ? WorkUnitModel.PercentComplete : 0;

        public SlotIdentifier SlotIdentifier => new SlotIdentifier(Settings.ClientIdentifier, SlotID);

        public string Name => SlotIdentifier.Name;

        public int SlotID { get; set; } = SlotIdentifier.NoSlotID;

        public SlotType SlotType { get; set; }

        public int? SlotThreads { get; set; }

        public string ClientVersion { get; set; }

        public string SlotTypeString
        {
            get
            {
                if (SlotType == SlotType.Unknown)
                {
                    return String.Empty;
                }

                var sb = new StringBuilder(SlotType.ToString());
                if (SlotThreads.HasValue)
                {
                    sb.AppendFormat(CultureInfo.InvariantCulture, ":{0}", SlotThreads);
                }
                if (ShowVersions && !String.IsNullOrEmpty(ClientVersion))
                {
                    sb.Append($" ({ClientVersion})");
                }
                return sb.ToString();
            }
        }

        public string SlotProcessor { get; set; }

        public bool IsUsingBenchmarkFrameTime => Status.IsRunning() && WorkUnitModel.IsUsingBenchmarkFrameTime(PPDCalculation);

        /// <summary>
        /// Time per frame (TPF) of the unit
        /// </summary>
        public TimeSpan TPF => Status.IsRunning() ? WorkUnitModel.GetFrameTime(PPDCalculation) : TimeSpan.Zero;

        /// <summary>
        /// Points per day (PPD) rating for this instance
        /// </summary>
        public double PPD => Status.IsRunning() ? Math.Round(WorkUnitModel.GetPPD(Status, PPDCalculation, BonusCalculation), DecimalPlaces) : 0;

        /// <summary>
        /// Units per day (UPD) rating for this instance
        /// </summary>
        public double UPD => Status.IsRunning() ? Math.Round(WorkUnitModel.GetUPD(PPDCalculation), 3) : 0;

        /// <summary>
        /// Estimated time of arrival (ETA) for this protein
        /// </summary>
        public TimeSpan ETA => Status.IsRunning() ? WorkUnitModel.GetEta(PPDCalculation) : TimeSpan.Zero;

        /// <summary>
        /// Estimated time of arrival (ETA) for this protein
        /// </summary>
        public DateTime ETADate => Status.IsRunning() ? WorkUnitModel.GetEtaDate(PPDCalculation) : DateTime.MinValue;

        public string Core
        {
            get
            {
                if (ShowVersions && Math.Abs(WorkUnitModel.WorkUnit.CoreVersion) > Single.Epsilon)
                {
                    return String.Format(CultureInfo.InvariantCulture, "{0} ({1:0.##})", WorkUnitModel.CurrentProtein.Core, WorkUnitModel.WorkUnit.CoreVersion);
                }
                return WorkUnitModel.CurrentProtein.Core;
            }
        }

        public string CoreID => WorkUnitModel.WorkUnit.CoreID;

        public string ProjectRunCloneGen => WorkUnitModel.WorkUnit.ToShortProjectString();

        public double Credit => Status.IsRunning() ? Math.Round(WorkUnitModel.GetCredit(Status, PPDCalculation, BonusCalculation), DecimalPlaces) : WorkUnitModel.CurrentProtein.Credit;

        public int Completed =>
           Prefs.Get<UnitTotalsType>(Preference.UnitTotals) == UnitTotalsType.All
              ? TotalCompletedUnits
              : TotalRunCompletedUnits;

        public int Failed =>
           Prefs.Get<UnitTotalsType>(Preference.UnitTotals) == UnitTotalsType.All
              ? TotalFailedUnits
              : TotalRunFailedUnits;

        /// <summary>
        /// Gets or sets the number of completed units since the last client start.
        /// </summary>
        public int TotalRunCompletedUnits { get; set; }

        /// <summary>
        /// Gets or sets the total number of completed units.
        /// </summary>
        public int TotalCompletedUnits { get; set; }

        /// <summary>
        /// Gets or sets the number of failed units since the last client start.
        /// </summary>
        public int TotalRunFailedUnits { get; set; }

        /// <summary>
        /// Gets or sets the total number of failed units.
        /// </summary>
        public int TotalFailedUnits { get; set; }

        /// <summary>
        /// Combined Folding ID and Team String
        /// </summary>
        public string Username =>
            String.IsNullOrWhiteSpace(WorkUnitModel.WorkUnit.FoldingID)
                ? String.Empty
                : String.Format(CultureInfo.InvariantCulture, "{0} ({1})", WorkUnitModel.WorkUnit.FoldingID, WorkUnitModel.WorkUnit.Team);

        public DateTime Assigned => WorkUnitModel.Assigned;

        public DateTime PreferredDeadline => WorkUnitModel.PreferredDeadline;

        #endregion

        public IList<LogLine> CurrentLogLines { get; set; }

        public WorkUnitQueue WorkUnitQueue { get; set; }

        #region Grid Data Warnings

        /// <summary>
        /// Project (R/C/G) is a Duplicate of another Client's Project (R/C/G)
        /// </summary>
        public bool ProjectIsDuplicate { get; set; }

        public bool UsernameOk
        {
            get
            {
                // if these are the default assigned values, don't check the prefs and just return true
                if ((String.IsNullOrWhiteSpace(WorkUnitModel.WorkUnit.FoldingID) || WorkUnitModel.WorkUnit.FoldingID == Unknown.Value) && WorkUnitModel.WorkUnit.Team == default)
                {
                    return true;
                }
                // if the slot is unknown or offline, don't check the prefs and just return true
                if (Status == SlotStatus.Unknown || Status == SlotStatus.Offline)
                {
                    return true;
                }
                return WorkUnitModel.WorkUnit.FoldingID == Prefs.Get<string>(Preference.StanfordId) &&
                       WorkUnitModel.WorkUnit.Team == Prefs.Get<int>(Preference.TeamId);
            }
        }

        #endregion

        /// <summary>
        /// Find slots with duplicate projects.
        /// </summary>
        public static void FindDuplicateProjects(ICollection<SlotModel> slots)
        {
            var duplicates = slots.GroupBy(x => x.WorkUnitModel.WorkUnit.ToShortProjectString())
                .Where(g => g.Count() > 1 && g.First().WorkUnitModel.WorkUnit.HasProject())
                .Select(g => g.Key)
                .ToList();

            foreach (var slot in slots)
            {
                slot.ProjectIsDuplicate = duplicates.Contains(slot.WorkUnitModel.WorkUnit.ToShortProjectString());
            }
        }
    }
}
