﻿
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;

using HFM.Core;
using HFM.Core.Logging;
using HFM.Core.Net;
using HFM.Forms.Models;
using HFM.Preferences;

namespace HFM.Forms
{
    public class ApplicationUpdatePresenter : IDisposable
    {
        public ApplicationUpdateModel Model { get; }
        public ILogger Logger { get; }
        public IPreferenceSet Preferences { get; }
        public MessageBoxPresenter MessageBox { get; }

        public ApplicationUpdatePresenter(ApplicationUpdateModel model, ILogger logger, IPreferenceSet preferences, MessageBoxPresenter messageBox)
        {
            Model = model;
            Logger = logger ?? NullLogger.Instance;
            Preferences = preferences ?? new InMemoryPreferenceSet();
            MessageBox = messageBox ?? NullMessageBoxPresenter.Instance;
        }

        public IWin32Dialog Dialog { get; protected set; }

        public virtual DialogResult ShowDialog(IWin32Window owner)
        {
            Dialog = new ApplicationUpdateDialog(this);
            return Dialog.ShowDialog(owner);
        }

        public void Dispose()
        {
            Dialog?.Dispose();
        }

        public async Task DownloadClick(FileDialogPresenter saveFile)
        {
            if (!ShowSaveFileView(saveFile)) return;

            Model.DownloadInProgress = true;
            try
            {
                bool downloadResult = await Task.Run(PerformDownload).ConfigureAwait(true);
                if (downloadResult)
                {
                    Model.SelectedUpdateFileIsReadyToBeExecuted = Model.SelectedUpdateFile.UpdateType == (int)ApplicationUpdateFileType.Executable;

                    Dialog.DialogResult = DialogResult.OK;
                    Dialog.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                string message = String.Format(CultureInfo.CurrentCulture,
                    "Download failed with the following error:{0}{0}{1}", Environment.NewLine, ex.Message);
                MessageBox.ShowError(message, Core.Application.NameAndVersion);
            }
            finally
            {
                Model.DownloadInProgress = false;
            }
        }

        private bool ShowSaveFileView(FileDialogPresenter saveFile)
        {
            saveFile.FileName = Model.SelectedUpdateFile.Name;
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                Model.SelectedUpdateFileLocalFilePath = saveFile.FileName;
                return true;
            }

            return false;
        }

        private WebOperation _webOperation;

        private bool PerformDownload()
        {
            var selectedUpdateFile = Model.SelectedUpdateFile;
            var url = selectedUpdateFile.HttpAddress;
            var path = Model.SelectedUpdateFileLocalFilePath;

            _webOperation = WebOperation.Create(url);
            _webOperation.WebRequest.Proxy = WebProxyFactory.Create(Preferences);

            // execute the download
            _webOperation.Download(path);

            if (_webOperation.Result != WebOperationResult.Completed)
            {
                return false;
            }

            // verify, throws exception on error
            selectedUpdateFile.Verify(path);
            return true;
        }

        public void CancelClick()
        {
            if (_webOperation != null && _webOperation.State == WebOperationState.InProgress)
            {
                _webOperation.Cancel();
            }
            else
            {
                Dialog.DialogResult = DialogResult.Cancel;
                Dialog.Close();
            }
        }
    }
}