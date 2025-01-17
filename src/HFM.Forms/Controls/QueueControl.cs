﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using HFM.Core.Client;
using HFM.Core.WorkUnits;

namespace HFM.Forms.Controls
{
    // TODO: Rename to WorkUnitQueueControl
    public sealed partial class QueueControl : UserControl
    {
        // ReSharper disable UnusedMember.Local
        private enum QueueControlRows
        {
            IndexCombo = 0,
            Blank1,
            Status,
            WaitingOn,
            Attempts,
            NextAttempt,
            BaseCredit,
            BeginDate,
            WorkServer,
            CPU,
            OS,
            Memory,
            CPUThreads,
            MachineID
        }
        // ReSharper restore UnusedMember.Local

        public event EventHandler<QueueIndexChangedEventArgs> QueueIndexChanged;

        private WorkUnitQueue _workUnitQueue;
        private IProteinService _proteinService;

        private SlotType _slotType = SlotType.Unknown;

        private const int DefaultRowHeight = 23;

        public QueueControl()
        {
            InitializeComponent();
        }

        public void SetProteinService(IProteinService proteinService)
        {
            _proteinService = proteinService;
        }

        private void OnQueueIndexChanged(QueueIndexChangedEventArgs e)
        {
            QueueIndexChanged?.Invoke(this, e);
        }

        public void SetWorkUnitQueue(WorkUnitQueue workUnitQueue, SlotType slotType)
        {
            if (workUnitQueue != null)
            {
                _workUnitQueue = workUnitQueue;
                _slotType = slotType;

                cboQueueIndex.SelectedIndexChanged -= cboQueueIndex_SelectedIndexChanged;
                cboQueueIndex.DataSource = CreateEntryNameCollection(_workUnitQueue);
                cboQueueIndex.DisplayMember = nameof(ListItem.DisplayMember);
                cboQueueIndex.ValueMember = nameof(ListItem.ValueMember);
                cboQueueIndex.SelectedIndex = -1;
                cboQueueIndex.SelectedIndexChanged += cboQueueIndex_SelectedIndexChanged;

                cboQueueIndex.SelectedValue = _workUnitQueue.CurrentQueueID;
            }
            else
            {
                _workUnitQueue = null;
                _slotType = SlotType.Unknown;
                SetControlsVisible(false);
            }
        }

        private static ICollection<ListItem> CreateEntryNameCollection(WorkUnitQueue workUnitQueue)
        {
            return workUnitQueue
                .Select(x => new ListItem(FormatDisplay(x), x.ID))
                .ToList();

            string FormatDisplay(WorkUnitQueueItem workUnit)
            {
                return String.Format(CultureInfo.InvariantCulture, "{0:00} - {1}", workUnit.ID, workUnit.ToShortProjectString());
            }
        }

        private void cboQueueIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_workUnitQueue == null) return;

            if (cboQueueIndex.SelectedIndex > -1)
            {
                SetControlsVisible(true);

                WorkUnitQueueItem item = _workUnitQueue[(int)cboQueueIndex.SelectedValue];
                StatusTextBox.Text = item.State;
                WaitingOnTextBox.Text = String.IsNullOrEmpty(item.WaitingOn) ? "(No Action)" : item.WaitingOn;
                AttemptsTextBox.Text = item.Attempts.ToString();
                NextAttemptTextBox.Text = item.NextAttempt.ToString();
                var protein = _proteinService.Get(item.ProjectID);
                BaseCreditTextBox.Text = protein != null ? protein.Credit.ToString(CultureInfo.CurrentCulture) : "0";
                AssignedTextBox.Text = FormatAssignedDateTimeUtc(item.AssignedDateTimeUtc);
                WorkServerTextBox.Text = item.WorkServer;
                CPUTypeTextBox.Text = item.CPU;
                OSTextBox.Text = item.OperatingSystem;
                MemoryTextBox.Text = item.Memory.ToString(CultureInfo.CurrentCulture);
                CPUThreadsTextBox.Text = item.CPUThreads.ToString(CultureInfo.CurrentCulture);
                MachineIDTextBox.Text = item.SlotID.ToString(CultureInfo.CurrentCulture);

                OnQueueIndexChanged(new QueueIndexChangedEventArgs((int)cboQueueIndex.SelectedValue));
            }
            else
            {
                // hide controls and display queue not available message
                SetControlsVisible(false);

                OnQueueIndexChanged(new QueueIndexChangedEventArgs(-1));
            }
        }

        private static string FormatAssignedDateTimeUtc(DateTime value)
        {
            if (value == DateTime.MinValue)
            {
                return "(Unknown)";
            }

            var localTime = value.ToLocalTime();
            return $"{localTime.ToShortDateString()} {localTime.ToShortTimeString()}";
        }

        private void SetControlsVisible(bool visible)
        {
            if (visible == false)
            {
                cboQueueIndex.DataSource = null;
                cboQueueIndex.Items.Clear();
                cboQueueIndex.Items.Add("No Queue Data");
                cboQueueIndex.SelectedIndex = 0;
            }

            StatusTextBox.Visible = visible;
            WaitingOnTextBox.Visible = visible;
            AttemptsTextBox.Visible = visible;
            NextAttemptTextBox.Visible = visible;
            BaseCreditTextBox.Visible = visible;
            AssignedTextBox.Visible = visible;
            WorkServerTextBox.Visible = visible;
            CPUTypeTextBox.Visible = visible;
            OSTextBox.Visible = visible;
            MemoryTextBox.Visible = visible;
            CPUThreadsTextBox.Visible = visible;
            MachineIDTextBox.Visible = visible;

            if (visible == false)
            {
                tableLayoutPanel1.RowStyles[(int)QueueControlRows.CPUThreads].Height = 0;
            }
            else
            {
                switch (_slotType)
                {
                    case SlotType.Unknown:
                        CPULabel.Text = "CPU:";
                        CPUThreadsTextBox.Visible = false;
                        tableLayoutPanel1.RowStyles[(int)QueueControlRows.CPUThreads].Height = 0;
                        break;
                    case SlotType.GPU:
                        CPULabel.Text = "GPU:";
                        CPUThreadsTextBox.Visible = false;
                        tableLayoutPanel1.RowStyles[(int)QueueControlRows.CPUThreads].Height = 0;
                        break;
                    case SlotType.CPU:
                        CPULabel.Text = "CPU:";
                        CPUThreadsTextBox.Visible = true;
                        tableLayoutPanel1.RowStyles[(int)QueueControlRows.CPUThreads].Height = DefaultRowHeight;
                        break;
                }
            }
        }
    }

    public class QueueIndexChangedEventArgs : EventArgs
    {
        public int Index { get; }

        public QueueIndexChangedEventArgs(int index)
        {
            Index = index;
        }
    }
}
