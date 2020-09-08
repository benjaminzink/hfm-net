﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using HFM.Core.WorkUnits;
using HFM.Forms.Internal;
using HFM.Preferences;

namespace HFM.Forms.Models
{
    public class MainModel : ViewModelBase
    {
        public IPreferences Preferences { get; }

        public MainModel(IPreferences preferences)
        {
            Preferences = preferences ?? new InMemoryPreferencesProvider();
        }

        public override void Load()
        {
            FormLocation = Preferences.Get<Point>(Preference.FormLocation);
            FormSize = Preferences.Get<Size>(Preference.FormSize);
            FormLogWindowVisible = Preferences.Get<bool>(Preference.FormLogWindowVisible);
            FormLogWindowHeight = Preferences.Get<int>(Preference.FormLogWindowHeight);
            FormSplitterLocation = Preferences.Get<int>(Preference.FormSplitterLocation);
            QueueWindowVisible = Preferences.Get<bool>(Preference.QueueWindowVisible);
            FollowLog = Preferences.Get<bool>(Preference.FollowLog);
            FormColumns.Reset(Preferences.Get<ICollection<string>>(Preference.FormColumns));

            // changed by preferences dialog
            MinimizeTo = Preferences.Get<MinimizeToOption>(Preference.MinimizeTo);
            ColorLogFile = Preferences.Get<bool>(Preference.ColorLogFile);

            Preferences.PreferenceChanged += (s, e) =>
            {
                string toolTip;
                switch (e.Preference)
                {
                    case Preference.MinimizeTo:
                        MinimizeTo = Preferences.Get<MinimizeToOption>(e.Preference);
                        break;
                    case Preference.ColorLogFile:
                        ColorLogFile = Preferences.Get<bool>(e.Preference);
                        break;
                    case Preference.BonusCalculation:
                        toolTip = ClientsModel.BonusCalculationList
                            .FirstOrDefault(x => x.GetValue<BonusCalculation>() == Preferences.Get<BonusCalculation>(Preference.BonusCalculation)).DisplayMember;
                        NotifyToolTip = toolTip;
                        break;
                    case Preference.PPDCalculation:
                        toolTip = ClientsModel.PPDCalculationList
                            .FirstOrDefault(x => x.GetValue<PPDCalculation>() == Preferences.Get<PPDCalculation>(Preference.PPDCalculation)).DisplayMember;
                        NotifyToolTip = toolTip;
                        break;
                }
            };
        }

        public override void Save()
        {
            Preferences.Set(Preference.FormLocation, FormLocation);
            Preferences.Set(Preference.FormSize, FormSize);
            Preferences.Set(Preference.FormLogWindowVisible, FormLogWindowVisible);
            Preferences.Set(Preference.FormLogWindowHeight, FormLogWindowHeight);
            Preferences.Set(Preference.FormSplitterLocation, FormSplitterLocation);
            Preferences.Set(Preference.QueueWindowVisible, QueueWindowVisible);
            Preferences.Set(Preference.FollowLog, FollowLog);
            Preferences.Set(Preference.FormColumns, FormColumns);

            Preferences.Save();
        }

        public void GridModelSelectedSlotChanged(object sender, EventArgs e)
        {
            var selectedSlot = (sender as MainGridModel)?.SelectedSlot;
            ClientDetails = selectedSlot?.SlotIdentifier.ClientIdentifier.ToServerPortString();
        }

        /// <summary>
        /// Gets the previous state of the window.
        /// </summary>
        public FormWindowState OriginalWindowState { get; private set; }

        private FormWindowState _windowState;

        public FormWindowState WindowState
        {
            get => _windowState;
            set
            {
                if (_windowState != value)
                {
                    OriginalWindowState = _windowState;
                    _windowState = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ShowInTaskbar));
                }
            }
        }

        public Point FormLocation { get; set; }

        public Size FormSize { get; set; }

        private bool _formLogWindowVisible;

        public bool FormLogWindowVisible
        {
            get => _formLogWindowVisible;
            set
            {
                if (_formLogWindowVisible != value)
                {
                    _formLogWindowVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        public int FormLogWindowHeight { get; set; }

        public int FormSplitterLocation { get; set; }

        private bool _queueWindowVisible;

        public bool QueueWindowVisible
        {
            get => _queueWindowVisible;
            set
            {
                if (_queueWindowVisible != value)
                {
                    _queueWindowVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICollection<string> FormColumns { get; } = new List<string>();

        private bool _followLog;

        public bool FollowLog
        {
            get => _followLog;
            set
            {
                if (_followLog != value)
                {
                    _followLog = value;
                    OnPropertyChanged();
                }
            }
        }

        private MinimizeToOption _minimizeTo;

        public MinimizeToOption MinimizeTo
        {
            get => _minimizeTo;
            set
            {
                if (_minimizeTo != value)
                {
                    _minimizeTo = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(NotifyIconVisible));
                    OnPropertyChanged(nameof(ShowInTaskbar));
                }
            }
        }

        private bool _colorLogFile;

        public bool ColorLogFile
        {
            get => _colorLogFile;
            set
            {
                if (_colorLogFile != value)
                {
                    _colorLogFile = value;
                    OnPropertyChanged();
                }
            }
        }

        public int DecimalPlaces => Preferences.Get<int>(Preference.DecimalPlaces);

        public bool NotifyIconVisible => MinimizeTo == MinimizeToOption.SystemTray || MinimizeTo == MinimizeToOption.Both;

        public bool ShowInTaskbar
        {
            get
            {
                if (WindowState != FormWindowState.Minimized) return true;
                return MinimizeTo == MinimizeToOption.TaskBar || MinimizeTo == MinimizeToOption.Both;
            }
        }

        private string _clientDetails;

        public string ClientDetails
        {
            get => _clientDetails;
            set
            {
                if (_clientDetails != value)
                {
                    _clientDetails = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _notifyToolTip;

        public string NotifyToolTip
        {
            get => _notifyToolTip;
            set
            {
                _notifyToolTip = value;
                OnPropertyChanged();
            }
        }
    }
}
