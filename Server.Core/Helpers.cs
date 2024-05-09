using gRPC.Common;
using gRPC.Common.Protos;
using DateTime = gRPC.Common.Protos.DateTime;

namespace Server.Core
{
    public static class Helpers
    {
        public static BaseResponse GetBaseResponseError(string code, string message)
        {
            return new BaseResponse()
            {
                Error = new CommonError()
                {
                    ErrorCode = code, 
                    ErrorText = message
                },
                TaskSubmitted = false
            };
        }

        public static BaseResponse GetBaseResponseSuccess()
        {
            return new BaseResponse() { Error = null, TaskSubmitted = true };
        }

        public static DateTimeValue[] GenerateValues(Date dEnd)
        {
            var dBeg = dEnd.AddDays(-62);
            var result = new List<DateTimeValue>()
            {
                new DateTimeValue()
                {
                    DateTime = new DateTime() {Date = dBeg, Time = new Time() {Hour = 0, Minute = 0, Second = 0}},
                    Value = 2
                }
            };
            var maxListItems = 744;
            

            var count = 1;

            do
            {
                var lastObj = result.LastOrDefault();
                result.Add(new DateTimeValue()
                {
                    Value = lastObj!.Value + 1,
                    DateTime = lastObj!.DateTime.AddHours(1)
                });
                count++;
            } while (count < maxListItems);

            return result.ToArray();
        }
    }
}
