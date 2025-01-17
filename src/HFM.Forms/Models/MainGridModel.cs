﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

using HFM.Core.Client;
using HFM.Forms.Internal;
using HFM.Preferences;

namespace HFM.Forms.Models
{
    public sealed class MainGridModel : ViewModelBase
    {
        private readonly ISynchronizeInvoke _synchronizeInvoke;
        private readonly SlotModelSortableBindingList _slotList;

        public IPreferences Preferences { get; }

        public ClientConfiguration ClientConfiguration { get; }

        public BindingSource BindingSource { get; }

        public MainGridModel(ISynchronizeInvoke synchronizeInvoke, IPreferences preferences, ClientConfiguration clientConfiguration)
        {
            _synchronizeInvoke = synchronizeInvoke;
            Preferences = preferences ?? new InMemoryPreferencesProvider();
            ClientConfiguration = clientConfiguration;

            _slotList = new SlotModelSortableBindingList();
            _slotList.RaiseListChangedEvents = false;
            BindingSource = new BindingSource();
        }

        public override void Load()
        {
            SortColumnName = Preferences.Get<string>(Preference.FormSortColumn);
            SortColumnOrder = Preferences.Get<ListSortDirection>(Preference.FormSortOrder);

            _slotList.OfflineClientsLast = Preferences.Get<bool>(Preference.OfflineLast);
            _slotList.Sorted += (sender, e) =>
            {
                SortColumnName = e.Name;
                Preferences.Set(Preference.FormSortColumn, SortColumnName);
                SortColumnOrder = e.Direction;
                Preferences.Set(Preference.FormSortOrder, SortColumnOrder);
            };
            BindingSource.DataSource = _slotList;
            BindingSource.CurrentItemChanged += (sender, args) => SelectedSlot = (SlotModel)BindingSource.Current;

            // subscribe to services raising events that require a view action
            Preferences.PreferenceChanged += (s, e) => OnPreferenceChanged(e);
            ClientConfiguration.ClientConfigurationChanged += (s, e) => ResetBindings();
        }

        private string _sortColumnName;

        public string SortColumnName
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_sortColumnName))
                {
                    _sortColumnName = DefaultSortColumnName;
                }
                return _sortColumnName;
            }
            set
            {
                if (_sortColumnName != value)
                {
                    _sortColumnName = String.IsNullOrWhiteSpace(value)
                        ? DefaultSortColumnName
                        : ValidateSortColumnNameOrDefault(value);
                }
            }
        }

        // SortColumnName stores the actual SlotModel property name
        // this method guards against SlotModel property name changes
        private string ValidateSortColumnNameOrDefault(string name)
        {
            var properties = TypeDescriptor.GetProperties(typeof(SlotModel));
            var property = properties.Find(name, true);
            return property is null ? DefaultSortColumnName : name;
        }

        private string DefaultSortColumnName { get; } = nameof(SlotModel.Name);

        public ListSortDirection SortColumnOrder { get; set; }

        private SlotModel _selectedSlot;

        public SlotModel SelectedSlot
        {
            get => _selectedSlot;
            set
            {
                if (!ReferenceEquals(_selectedSlot, value))
                {
                    _selectedSlot = value;
                    OnPropertyChanged();
                }
            }
        }

        private void OnPreferenceChanged(PreferenceChangedEventArgs e)
        {
            switch (e.Preference)
            {
                case Preference.OfflineLast:
                    _slotList.OfflineClientsLast = Preferences.Get<bool>(Preference.OfflineLast);
                    Sort();
                    break;
                case Preference.PPDCalculation:
                case Preference.DecimalPlaces:
                case Preference.BonusCalculation:
                    ResetBindings();
                    break;
            }
        }

        private readonly object _resetBindingsLock = new object();

        private void ResetBindings()
        {
            if (!Monitor.TryEnter(_resetBindingsLock))
            {
                Debug.WriteLine("Reset already in progress...");
                return;
            }
            try
            {
                ResetBindingsInternal();
            }
            finally
            {
                Monitor.Exit(_resetBindingsLock);
            }
        }

        private readonly object _slotsListLock = new object();

        private SlotTotals _slotTotals;

        public SlotTotals SlotTotals
        {
            get => _slotTotals;
            private set
            {
                _slotTotals = value;
                OnPropertyChanged();
            }
        }

        private void ResetBindingsInternal()
        {
            if (_synchronizeInvoke is Control control && control.IsDisposed)
            {
                return;
            }
            if (_synchronizeInvoke.InvokeRequired)
            {
                _synchronizeInvoke.Invoke(new Action(ResetBindingsInternal), null);
                return;
            }

            lock (_slotsListLock)
            {
                // get slots from the dictionary
                var slots = ClientConfiguration.GetSlots();

                // refresh the underlying binding list
                BindingSource.Clear();
                foreach (var slot in slots)
                {
                    BindingSource.Add(slot);
                }
                Debug.WriteLine("Number of slots: {0}", BindingSource.Count);

                SortInternal();
                ResetSelectedSlot();
                SlotModel.FindDuplicateProjects(slots);
                SlotTotals = SlotTotals.Create(slots);

                BindingSource.ResetBindings(false);
            }
            OnAfterResetBindings(EventArgs.Empty);
        }

        private void Sort()
        {
            lock (_slotsListLock)
            {
                SortInternal();
            }
        }

        private void SortInternal()
        {
            BindingSource.Sort = $"{SortColumnName} {SortColumnOrder.ToBindingSourceSortString()}";
            if (_slotList is IBindingList bindingList)
            {
                bindingList.ApplySort(bindingList.SortProperty, bindingList.SortDirection);
            }
        }

        public void ResetSelectedSlot()
        {
            if (SelectedSlot == null) return;

            int row = BindingSource.Find(nameof(SlotModel.Name), SelectedSlot.Name);
            if (row > -1)
            {
                BindingSource.Position = row;
            }
        }

        public event EventHandler AfterResetBindings;

        private void OnAfterResetBindings(EventArgs e) => AfterResetBindings?.Invoke(this, e);
    }
}
