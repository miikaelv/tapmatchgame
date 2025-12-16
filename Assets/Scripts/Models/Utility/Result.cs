using UnityEngine;

namespace TapMatch.Models.Utility
{
    public enum ResultType
    {
        None = 0,
        Succeeded = 1,
        Failed = 2,
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
        
        public bool TryGetResultAndLogOnError(out T model)
        {
            model = default;
            
            if (IsError(out var errorMessage))
            {
                Debug.LogError($"{errorMessage}");
                return false;
            }

            model = Model;
            return true;
        }

        private Result(T model)
        {
            ResultType = ResultType.Succeeded;
            DeveloperErrorMessage = "";
            Model = model;
        }

        private Result()
        {
            ResultType = ResultType.Failed;
            DeveloperErrorMessage = "";
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

        public static Result<T> Failed() => new();
        public static Result<T> Create(T model) => new(model);
    }

    public static class ResultExtensions
    {
        public static Result<T> ToResult<T>(this T obj) => Result<T>.Create(obj);
    }
}