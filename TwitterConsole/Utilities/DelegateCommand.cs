using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TwitterConsole.Utilities
{
    /// <summary>
    /// Class I have used in other projects
    /// </summary>
    public class DelegateCommand : ICommand
    {
        private bool _canExecuteCache;
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public DelegateCommand(Func<object, bool> canExecute, Action<object> execute)
        {
            _canExecute = canExecute;
            _execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null) return true;

            var toReturn = _canExecute(parameter);
            if (toReturn != _canExecuteCache)
            {
                _canExecuteCache = toReturn;
                if (CanExecuteChanged != null) CanExecuteChanged(this, new EventArgs());
            }
            return toReturn;
        }

        public void Execute(object parameter)
        {
            if (_execute != null) _execute(parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}
