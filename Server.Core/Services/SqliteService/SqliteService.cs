using gRPC.Common.Protos;
using Microsoft.Extensions.Logging;
using Guid = System.Guid;

namespace Server.Core.Services
{
    public class SqliteService : ISqliteService
    {
        private readonly ILogger<SqliteService> _logger;
        private readonly ApplicationContext _context = new();
        public SqliteService(ILogger<SqliteService> logger, INormalizeDataService normalizeDataService)
        {
            _logger = logger;
        }

        public async Task<Result> AddCoefficients(RequestСoefficientMinMax model)
        {
            if (!model.ValidateModel()) return new ErrorResult("В модели не хватает данных");
            await _context.Сoefficients.AddAsync(model);
            await _context.SaveChangesAsync();
            return new SuccessResult();
        }

        public async Task<Result<RequestСoefficientMinMax>> GetCoefficientsByRequestId(string requestId)
        {
            if(requestId == Guid.Empty.ToString()) return new ErrorResult<RequestСoefficientMinMax>("");
            var response = 
                await _context.Сoefficients
                .FindAsync(requestId);

            return response is null
                ? new ErrorResult<RequestСoefficientMinMax>("")
                : new SuccessResult<RequestСoefficientMinMax>(response);
        }

        public async Task<Result> SaveClientData(CentralServerRequest request)
        {
            var rows = request.DateTimeValue
                    .Select(row => new PredictionRow(row, request.RequestGuid))
                    .ToList();

            try
            {
                await _context.PredictionRows.AddRangeAsync(rows);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex.Message);
            }

            return new SuccessResult();
        }

        public Task<Result<PredictionRow[]>> GetClientData(HourlyConsumptionRequest request)
        {
            //request.
            throw new NotImplementedException();
        }

        public async Task<Result<PredictionRow[]>> GetClientData(string requestId)
        {
            if(requestId == Guid.Empty.ToString()) return new ErrorResult<PredictionRow[]>("");
            var response = _context.PredictionRows.Where(row => row.Id == requestId).ToArray();

            return await Task.FromResult(new SuccessResult<PredictionRow[]>(response));
        }
    }
}
