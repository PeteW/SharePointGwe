namespace Oxbow.Gwe.Core.WebServiceModels
{
    public class BaseWebServiceRequestModel
    {
        public string Id { get; set; }
    }

    public class BaseWebServiceResponseModel
    {
        public ExecutionOutcome ExecutionOutcome { get; set; }
    }

    public class ExecutionOutcome
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }

        public static ExecutionOutcome Success()
        {
            return new ExecutionOutcome { IsSuccessful = true };
        }

        public static ExecutionOutcome Success(string message)
        {
            return new ExecutionOutcome { IsSuccessful = true, Message = message };
        }

        public static ExecutionOutcome Fail(string message)
        {
            return new ExecutionOutcome { IsSuccessful = false, Message = message };
        }
    }
}