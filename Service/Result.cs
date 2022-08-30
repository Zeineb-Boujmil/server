namespace MasterDataService
{
    public class Result<T>
    {
        private Result() { }

        public bool IsSuccessfull { get; private set; }
        public string ErrorMessage { get; private set; }
        public T Value { get; private set; }

        public static Result<T> CreateSuccesfullResult(T value)
        {
            return new Result<T>()
            {
                IsSuccessfull = true,
                Value = value
            };
        }

        public static Result<T> CreateUnsuccesfullResult(string errorMessage)
        {
            return new Result<T>()
            {
                IsSuccessfull = false,
                ErrorMessage = errorMessage
            };
        }
    }
}