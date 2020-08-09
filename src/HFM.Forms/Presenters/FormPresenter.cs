﻿using System;

using HFM.Forms.Models;
using HFM.Forms.Views;

namespace HFM.Forms.Presenters
{
    public abstract class FormPresenter : IFormPresenter
    {
        public IWin32Form Form { get; protected set; }

        public virtual void Show()
        {
            Form = OnCreateForm();
            Form.Closed += OnClosed;
            Form.Show();
        }

        protected abstract IWin32Form OnCreateForm();

        public virtual void Close()
        {
            Form?.Close();
        }

        public event EventHandler Closed;

        protected virtual void OnClosed(object sender, EventArgs e)
        {
            Closed?.Invoke(this, e);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Form?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public abstract class FormPresenter<TViewModel> : FormPresenter where TViewModel : ViewModelBase
    {
        protected ViewModelBase ModelBase { get; }

        protected FormPresenter(TViewModel model)
        {
            ModelBase = model;
        }

        public override void Show()
        {
            ModelBase.Load();

            Form = OnCreateForm();
            Form.Closed += OnClosed;
            Form.Show();
        }

        protected override void OnClosed(object sender, EventArgs e)
        {
            ModelBase.Save();
            base.OnClosed(sender, e);
        }
    }
}
