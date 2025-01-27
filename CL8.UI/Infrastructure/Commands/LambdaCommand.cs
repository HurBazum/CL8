using CL8.UI.Infrastructure.Commands.Base;

namespace CL8.UI.Infrastructure.Commands
{
    public class LambdaCommand(Action<object> execute, Func<object, bool> canExecute) : Command
    {
        private readonly Action<object> _execute = execute;
        private readonly Func<object, bool> _canExecute = canExecute;

        public override void Execute(object? parameter) => _execute(parameter);
        public override bool CanExecute(object? parameter) => _canExecute.Invoke(parameter);
    }
}