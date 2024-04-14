using Microsoft.Extensions.Logging;

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
    }
}
