using System;
using System.Windows.Input;

namespace MyDriving.Helpers
{
    public class RelayCommand : ICommand
    {
        private readonly Action handler;
        private bool isEnabled;
        private readonly Func<bool> canExecute;

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
                    if (CanExecuteChanged != null)
                    {
                        CanExecuteChanged(this, EventArgs.Empty);
                    }
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
        /// Method used to raise the <see cref="CanExecuteChanged"/> event
        /// to indicate that the return value of the <see cref="CanExecute"/>
        /// method has changed.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> handler;
        private bool isEnabled = true;

        private readonly Func<T, bool> canExecute;

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
                    if (CanExecuteChanged != null)
                    {
                        CanExecuteChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            if (canExecute != null)
                IsEnabled = canExecute((T)parameter);

            return IsEnabled;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            handler((T)parameter);
        }

        /// <summary>
        /// Method used to raise the <see cref="CanExecuteChanged"/> event
        /// to indicate that the return value of the <see cref="CanExecute"/>
        /// method has changed.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
