using TapMatch.Models;
using TapMatch.Models.Utility;

namespace TapMatch.UnityServices.Actions
{
    // Atomized Game Actions that can be created anywhere,
    // but only run through ModelService which will decide in what context it can edit the Models
    public abstract class GameAction<T> where T : IGameActionResult
    {
        protected abstract Result<bool> CanExecute(GameState state);

        public Result<T> Execute(GameState state)
        {
            var canExecuteResult = CanExecute(state);

            return canExecuteResult.IsError(out var message) 
                ? Result<T>.GenericError(message) 
                : ExecuteInternal(state);
        }

        protected abstract Result<T> ExecuteInternal(GameState state);
    }
}