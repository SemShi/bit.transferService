namespace Server.Core.Services
{
    public interface ISqliteService
    {
        Task<Result> AddCoefficients(RequestСoefficientMinMax model);
        Task<Result<RequestСoefficientMinMax>> GetCoefficientsByRequestId(string requestId);
    }
}
