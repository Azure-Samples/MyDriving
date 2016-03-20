// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Windows.Input;

namespace MyDriving.Helpers
{
    public class RelayCommand : ICommand
    {
        private readonly Func<bool> canExecute;
        private readonly Action handler;
        private bool isEnabled;

        public RelayCommand(Action handler, Func<bool> canExecute = null)
        {
            this.handler = handler;
            this.canExecute = canExecute;
            if (canExecute == null)
                isEnabled = true;
        }

        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                if (value != isEnabled)
                {
                    isEnabled = value;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            if (canExecute != null)
                IsEnabled = canExecute();

            return IsEnabled;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            handler();
        }

        /// <summary>
        ///     Method used to raise the <see cref="CanExecuteChanged" /> event
        ///     to indicate that the return value of the <see cref="CanExecute" />
        ///     method has changed.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            handler?.Invoke(this, EventArgs.Empty);
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Func<T, bool> canExecute;
        private readonly Action<T> handler;
        private bool isEnabled = true;

        public RelayCommand(Action<T> handler, Func<T, bool> canExecute = null)
        {
            this.handler = handler;
            this.canExecute = canExecute;
            if (canExecute == null)
                isEnabled = true;
        }

        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                if (value != isEnabled)
                {
                    isEnabled = value;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            if (canExecute != null)
                IsEnabled = canExecute((T) parameter);

            return IsEnabled;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            handler((T) parameter);
        }

        /// <summary>
        ///     Method used to raise the <see cref="CanExecuteChanged" /> event
        ///     to indicate that the return value of the <see cref="CanExecute" />
        ///     method has changed.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            handler?.Invoke(this, EventArgs.Empty);
        }
    }
}