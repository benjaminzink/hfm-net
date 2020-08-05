﻿/*
 * HFM.NET - Work Unit History Query UI Form
 * Copyright (C) 2009-2015 Ryan Harlamert (harlam357)
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

using HFM.Core.Data;
using HFM.Forms.Controls;

namespace HFM.Forms
{
   public interface IQueryView : IWin32Window
   {
      WorkUnitQuery Query { get; set; }
      
      bool Visible { get; set; }

      DialogResult ShowDialog(IWin32Window owner);

      void Close();
   }

   public partial class QueryDialog : Form, IQueryView
   {
      private WorkUnitQuery _workUnitQuery;
      private BindingList<WorkUnitQueryParameter> _parametersList;

      public QueryDialog()
      {
         InitializeComponent();
         SetupDataGridViewColumns();
         dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;

         Query = new WorkUnitQuery();
      }

      #region IQueryView Members

      public WorkUnitQuery Query
      {
         get { return _workUnitQuery; }
         set
         {
            _workUnitQuery = value;
            _parametersList = new BindingList<WorkUnitQueryParameter>(_workUnitQuery.Parameters);
            BindNameTextBox(_workUnitQuery);
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = _parametersList;
         }
      }

      #endregion

      public void BindNameTextBox(WorkUnitQuery parameters)
      {
         txtName.DataBindings.Clear();
         txtName.DataBindings.Add("Text", parameters, "Name", false, DataSourceUpdateMode.OnPropertyChanged);
      }

      private void SetupDataGridViewColumns()
      {
         dataGridView1.AutoGenerateColumns = false;

         var queryColumn = new DataGridViewComboBoxColumn();
         var columnChoices = GetQueryFieldChoices();
         queryColumn.Name = "Name";
         queryColumn.HeaderText = "Name";
         queryColumn.DataSource = columnChoices;
         queryColumn.DisplayMember = nameof(ListItem.DisplayMember);
         queryColumn.ValueMember = nameof(ListItem.ValueMember);
         queryColumn.DataPropertyName = nameof(WorkUnitQueryParameter.Column);
         queryColumn.Width = 150;
         dataGridView1.Columns.Add(queryColumn);

         queryColumn = new DataGridViewComboBoxColumn();
         columnChoices = GetOperatorFieldChoices();
         queryColumn.Name = "Operator";
         queryColumn.HeaderText = "Operator";
         queryColumn.DataSource = columnChoices;
         queryColumn.DisplayMember = nameof(ListItem.DisplayMember);
         queryColumn.ValueMember = nameof(ListItem.ValueMember);
         queryColumn.DataPropertyName = nameof(WorkUnitQueryParameter.Operator);
         queryColumn.Width = 175;
         dataGridView1.Columns.Add(queryColumn);

         var valueColumn = new DataGridViewQueryValueColumn();
         valueColumn.Name = "Value";
         valueColumn.HeaderText = "Value";
         valueColumn.DataPropertyName = nameof(WorkUnitQueryParameter.Value);
         valueColumn.DefaultCellStyle.DataSourceNullValue = null;
         //valueColumn.Width = 200;
         valueColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
         dataGridView1.Columns.Add(valueColumn);
      }

      private static List<ListItem> GetQueryFieldChoices()
      {
         var columnChoices = new List<ListItem>();
         
         // On Mono the ComboBox choices must exactly match the enumeration name - LAME!!!
         if (Core.Application.IsRunningOnMono)
         {
            columnChoices.Add(new ListItem(WorkUnitRowColumn.ProjectID.ToString(), WorkUnitRowColumn.ProjectID));
            columnChoices.Add(new ListItem(WorkUnitRowColumn.WorkUnitName.ToString(), WorkUnitRowColumn.WorkUnitName));
            columnChoices.Add(new ListItem(WorkUnitRowColumn.Name.ToString(), WorkUnitRowColumn.Name));
            columnChoices.Add(new ListItem(WorkUnitRowColumn.Path.ToString(), WorkUnitRowColumn.Path));
            columnChoices.Add(new ListItem(WorkUnitRowColumn.Username.ToString(), WorkUnitRowColumn.Username));
            columnChoices.Add(new ListItem(WorkUnitRowColumn.Team.ToString(), WorkUnitRowColumn.Team));
            columnChoices.Add(new ListItem(WorkUnitRowColumn.SlotType.ToString(), WorkUnitRowColumn.SlotType));
            columnChoices.Add(new ListItem(WorkUnitRowColumn.Core.ToString(), WorkUnitRowColumn.Core));
            columnChoices.Add(new ListItem(WorkUnitRowColumn.CoreVersion.ToString(), WorkUnitRowColumn.CoreVersion));
            columnChoices.Add(new ListItem(WorkUnitRowColumn.FrameTime.ToString(), WorkUnitRowColumn.FrameTime));
            columnChoices.Add(new ListItem(WorkUnitRowColumn.KFactor.ToString(), WorkUnitRowColumn.KFactor));
            columnChoices.Add(new ListItem(WorkUnitRowColumn.PPD.ToString(), WorkUnitRowColumn.PPD));
            columnChoices.Add(new ListItem(WorkUnitRowColumn.Assigned.ToString(), WorkUnitRowColumn.Assigned));
            columnChoices.Add(new ListItem(WorkUnitRowColumn.Finished.ToString(), WorkUnitRowColumn.Finished));
            columnChoices.Add(new ListItem(WorkUnitRowColumn.Credit.ToString(), WorkUnitRowColumn.Credit));
            columnChoices.Add(new ListItem(WorkUnitRowColumn.Frames.ToString(), WorkUnitRowColumn.Frames));
            columnChoices.Add(new ListItem(WorkUnitRowColumn.FramesCompleted.ToString(), WorkUnitRowColumn.FramesCompleted));
            columnChoices.Add(new ListItem(WorkUnitRowColumn.Result.ToString(), WorkUnitRowColumn.Result));
            columnChoices.Add(new ListItem(WorkUnitRowColumn.Atoms.ToString(), WorkUnitRowColumn.Atoms));
            columnChoices.Add(new ListItem(WorkUnitRowColumn.ProjectRun.ToString(), WorkUnitRowColumn.ProjectRun));
            columnChoices.Add(new ListItem(WorkUnitRowColumn.ProjectClone.ToString(), WorkUnitRowColumn.ProjectClone));
            columnChoices.Add(new ListItem(WorkUnitRowColumn.ProjectGen.ToString(), WorkUnitRowColumn.ProjectGen));
         }
         else
         {
            string[] names = WorkUnitQueryParameter.GetColumnNames();
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.ProjectID], WorkUnitRowColumn.ProjectID));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.WorkUnitName], WorkUnitRowColumn.WorkUnitName));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.Name], WorkUnitRowColumn.Name));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.Path], WorkUnitRowColumn.Path));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.Username], WorkUnitRowColumn.Username));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.Team], WorkUnitRowColumn.Team));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.SlotType], WorkUnitRowColumn.SlotType));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.Core], WorkUnitRowColumn.Core));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.CoreVersion], WorkUnitRowColumn.CoreVersion));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.FrameTime], WorkUnitRowColumn.FrameTime));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.KFactor], WorkUnitRowColumn.KFactor));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.PPD], WorkUnitRowColumn.PPD));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.Assigned], WorkUnitRowColumn.Assigned));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.Finished], WorkUnitRowColumn.Finished));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.Credit], WorkUnitRowColumn.Credit));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.Frames], WorkUnitRowColumn.Frames));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.FramesCompleted], WorkUnitRowColumn.FramesCompleted));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.Result], WorkUnitRowColumn.Result));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.Atoms], WorkUnitRowColumn.Atoms));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.ProjectRun], WorkUnitRowColumn.ProjectRun));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.ProjectClone], WorkUnitRowColumn.ProjectClone));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.ProjectGen], WorkUnitRowColumn.ProjectGen));
         }

         return columnChoices;
      }
      
      private static List<ListItem> GetOperatorFieldChoices()
      {
         var columnChoices = new List<ListItem>();
         if (Core.Application.IsRunningOnMono)
         {
            columnChoices.Add(new ListItem(WorkUnitQueryOperator.Equal.ToString(), WorkUnitQueryOperator.Equal));
            columnChoices.Add(new ListItem(WorkUnitQueryOperator.NotEqual.ToString(), WorkUnitQueryOperator.NotEqual));
            columnChoices.Add(new ListItem(WorkUnitQueryOperator.GreaterThan.ToString(), WorkUnitQueryOperator.GreaterThan));
            columnChoices.Add(new ListItem(WorkUnitQueryOperator.GreaterThanOrEqual.ToString(), WorkUnitQueryOperator.GreaterThanOrEqual));
            columnChoices.Add(new ListItem(WorkUnitQueryOperator.LessThan.ToString(), WorkUnitQueryOperator.LessThan));
            columnChoices.Add(new ListItem(WorkUnitQueryOperator.LessThanOrEqual.ToString(), WorkUnitQueryOperator.LessThanOrEqual));
            columnChoices.Add(new ListItem(WorkUnitQueryOperator.Like.ToString(), WorkUnitQueryOperator.Like));
            columnChoices.Add(new ListItem(WorkUnitQueryOperator.NotLike.ToString(), WorkUnitQueryOperator.NotLike));
         }
         else
         {
            columnChoices.Add(new ListItem("Equal", WorkUnitQueryOperator.Equal));
            columnChoices.Add(new ListItem("Not Equal", WorkUnitQueryOperator.NotEqual));
            columnChoices.Add(new ListItem("Greater Than", WorkUnitQueryOperator.GreaterThan));
            columnChoices.Add(new ListItem("Greater Than Or Equal", WorkUnitQueryOperator.GreaterThanOrEqual));
            columnChoices.Add(new ListItem("Less Than", WorkUnitQueryOperator.LessThan));
            columnChoices.Add(new ListItem("Less Than Or Equal", WorkUnitQueryOperator.LessThanOrEqual));
            columnChoices.Add(new ListItem("Like", WorkUnitQueryOperator.Like));
            columnChoices.Add(new ListItem("Not Like", WorkUnitQueryOperator.NotLike));
         }

         return columnChoices;
      }

      private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
      {
         // column is query field
         if (e.ColumnIndex == 0)
         {
            // clear the value cell (this works for .NET and Mono)
            dataGridView1["Value", e.RowIndex].Value = null;
         }
      }

      private void btnAdd_Click(object sender, EventArgs e)
      {
         _workUnitQuery.Parameters.Add(new WorkUnitQueryParameter());
         RefreshDisplay();
      }

      private void btnRemove_Click(object sender, EventArgs e)
      {
         Debug.Assert(dataGridView1.SelectedCells.Count == 1);
         foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
         {
            _workUnitQuery.Parameters.RemoveAt(cell.OwningRow.Index);
         }
         RefreshDisplay();
      }

      private void RefreshDisplay()
      {
         _parametersList.ResetBindings();
      }

      private void btnOK_Click(object sender, EventArgs e)
      {
         DialogResult = DialogResult.OK;
         Close();
      }

      private void btnCancel_Click(object sender, EventArgs e)
      {
         DialogResult = DialogResult.Cancel;
         Close();
      }
   }
}