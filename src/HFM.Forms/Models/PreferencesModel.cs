﻿
using System;
using System.Collections.Generic;
using System.Linq;

using HFM.Preferences;

namespace HFM.Forms.Models
{
    public class PreferencesModel : ViewModelBase
    {
        public PreferencesModel(IPreferenceSet preferences, IAutoRunConfiguration autoRunConfiguration)
        {
            Preferences = preferences;
            ScheduledTasksModel = new ScheduledTasksModel(preferences);
            StartupAndExternalModel = new StartupAndExternalModel(preferences, autoRunConfiguration);
            OptionsModel = new OptionsModel(preferences);
            ReportingModel = new ReportingModel(preferences);
            WebSettingsModel = new WebSettingsModel(preferences);
            WebVisualStylesModel = new WebVisualStylesModel(preferences);
        }

        public IPreferenceSet Preferences { get; }
        public ScheduledTasksModel ScheduledTasksModel { get; set; }
        public StartupAndExternalModel StartupAndExternalModel { get; set; }
        public OptionsModel OptionsModel { get; set; }
        public ReportingModel ReportingModel { get; set; }
        public WebSettingsModel WebSettingsModel { get; set; }
        public WebVisualStylesModel WebVisualStylesModel { get; set; }

        public override string Error
        {
            get
            {
                var errors = EnumerateErrors().Where(x => !String.IsNullOrEmpty(x));
                return String.Join(Environment.NewLine, errors);
            }
        }

        private IEnumerable<string> EnumerateErrors()
        {
            yield return ScheduledTasksModel.Error;
            yield return StartupAndExternalModel.Error;
            yield return OptionsModel.Error;
            yield return ReportingModel.Error;
            yield return WebSettingsModel.Error;
            yield return WebVisualStylesModel.Error;
        }

        public override bool ValidateAcceptance()
        {
            OnPropertyChanged(String.Empty);
            ScheduledTasksModel.ValidateAcceptance();
            StartupAndExternalModel.ValidateAcceptance();
            OptionsModel.ValidateAcceptance();
            ReportingModel.ValidateAcceptance();
            WebSettingsModel.ValidateAcceptance();
            WebVisualStylesModel.ValidateAcceptance();
            return !HasError;
        }

        public override void Load()
        {
            ScheduledTasksModel.Load();
            StartupAndExternalModel.Load();
            OptionsModel.Load();
            ReportingModel.Load();
            WebSettingsModel.Load();
            WebVisualStylesModel.Load();
        }

        public override void Save()
        {
            ScheduledTasksModel.Save();
            StartupAndExternalModel.Save();
            OptionsModel.Save();
            ReportingModel.Save();
            WebSettingsModel.Save();
            WebVisualStylesModel.Save();
            
            Preferences.Save();
            StartupAndExternalModel.SaveAutoRunConfiguration();
        }
    }
}