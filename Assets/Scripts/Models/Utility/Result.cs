namespace TapMatch.Models.Utility
{
    public enum ResultType
    {
        None = 0,
        Succeeded = 1,
        ValidFailure = 2,
        GenericError = 3,
    }

    // Result Wrapper with error handling, copied over from personal project
    public class Result<T>
    {
        private readonly ResultType ResultType;
        private readonly string DeveloperErrorMessage;
        private readonly T Model;

        public bool IsError(out string errorMessage)
        {
            errorMessage = DeveloperErrorMessage;
            return ResultType >= ResultType.GenericError;
        }

        public bool Succeeded => ResultType == ResultType.Succeeded;

        public bool TryGetModel(out T result)
        {
            result = Model;

            return ResultType switch
            {
                ResultType.Succeeded => true,
                _ => false
            };
        }

        private Result(T model)
        {
            ResultType = ResultType.Succeeded;
            DeveloperErrorMessage = "";
            Model = model;
        }

        private Result(bool validFailure = false)
        {
            if (validFailure)
            {
                ResultType = ResultType.ValidFailure;
                DeveloperErrorMessage = "";
            }
            else
            {
                ResultType = ResultType.GenericError;
                DeveloperErrorMessage = $"Created Empty Result of return type {typeof(T).Name}";
            }

            Model = default;
        }

        private Result(ResultType resultType, string developerErrorMessage)
        {
            ResultType = resultType;
            DeveloperErrorMessage = developerErrorMessage;
            Model = default;
        }

        public static Result<T> GenericError(string developerErrorMessage) =>
            new(ResultType.GenericError, developerErrorMessage);

        public static Result<T> ValidFailure() => new(true);
        public static Result<T> Create(T model) => new(model);
    }

    public static class ResultExtensions
    {
        public static Result<T> ToResult<T>(this T obj) => Result<T>.Create(obj);
    }
}