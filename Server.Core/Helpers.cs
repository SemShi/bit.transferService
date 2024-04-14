using gRPC.Common.Protos;

namespace Server.Core
{
    public static class Helpers
    {
        public static BaseResponse GetBaseResponseError(string code, string message)
        {
            return new BaseResponse()
            {
                Error = new CommonError()
                    { ErrorCode = code, ErrorText = message },
                TaskSubmitted = false
            };
        }

        public static BaseResponse GetBaseResponseSuccess()
        {
            return new BaseResponse() { Error = null, TaskSubmitted = true };
        }
    }
}
