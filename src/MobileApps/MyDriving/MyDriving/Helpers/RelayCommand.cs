// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Windows.Input;

namespace MyDriving.Helpers
{
    public class RelayCommand : ICommand
    {
        private readonly Func<bool> _canExecute;
        private readonly Action _handler;
        private bool _isEnabled;

        public RelayCommand(Action handler, Func<bool> canExecute = null)
        {
            _handler = handler;
            _canExecute = canExecute;
            if (canExecute == null)
                _isEnabled = true;
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (value != _isEnabled)
                {
                    _isEnabled = value;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
                IsEnabled = _canExecute();

            return IsEnabled;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _handler();
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
        private readonly Func<T, bool> _canExecute;
        private readonly Action<T> _handler;
        private bool _isEnabled = true;

        public RelayCommand(Action<T> handler, Func<T, bool> canExecute = null)
        {
            _handler = handler;
            _canExecute = canExecute;
            if (canExecute == null)
                _isEnabled = true;
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (value != _isEnabled)
                {
                    _isEnabled = value;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
                IsEnabled = _canExecute((T) parameter);

            return IsEnabled;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _handler((T) parameter);
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