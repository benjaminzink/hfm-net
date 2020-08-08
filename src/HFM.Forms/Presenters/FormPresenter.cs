﻿using System;

using HFM.Forms.Views;

namespace HFM.Forms.Presenters
{
    public abstract class FormPresenter : IFormPresenter
    {
        public IWin32Form Form { get; protected set; }

        public virtual void Show()
        {
            var form = OnCreateForm();
            form.Closed += OnClosed;
            Form = form;
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
}