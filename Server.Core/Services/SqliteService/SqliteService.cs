using Google.Protobuf.Collections;
using gRPC.Common.Protos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DateTime = gRPC.Common.Protos.DateTime;
using Guid = System.Guid;

namespace Server.Core.Services
{
    public class SqliteService : ISqliteService
    {
        private readonly ILogger<SqliteService> _logger;
        private readonly ApplicationContext _context = new();
        public SqliteService(ILogger<SqliteService> logger)
        {
            _logger = logger;
        }

        public async Task<Result> AddCoefficients(RequestСoefficientMinMax model)
        {
            if (!model.ValidateModel()) return new ErrorResult("В модели не хватает данных");
            try
            {
                await _context.Сoefficients.AddAsync(model);
                await _context.SaveChangesAsync();
            }
            catch
            {
                UndetouchEntities();
                return new ErrorResult("Ошибка при сохранинии в базху данных");
            }
            return new SuccessResult();
        }

        public async Task<Result> SavePrediction(SaveHourlyConsumptionRequest request)
        {
            //#TODO нужен requestId
            if (!request.ConsumptionData.Any())
                return new ErrorResult("0");

            var dataToModels = 
                request.ConsumptionData.Select(x => new PredictedRow(x, ""));
            try
            {
                await _context.PredictedRows.AddRangeAsync(dataToModels);
                await _context.SaveChangesAsync();
            }
            catch
            {
                UndetouchEntities();
                return new ErrorResult("Ошибка при сохранинии в базху данных");
            }

            return new SuccessResult();
        }

        private void UndetouchEntities()
        {
            var undetachedEntriesCopy = _context.ChangeTracker.Entries()
                .Where(e => e.State != EntityState.Detached)
                .ToList();

            foreach (var entry in undetachedEntriesCopy)
                entry.State = EntityState.Detached;
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
                    .Select(row => new ClientData(row, request.RequestGuid))
                    .ToList();

            try
            {
                await _context.ClientDataRows.AddRangeAsync(rows);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                UndetouchEntities();
                return new ErrorResult(ex.Message);
            }

            return new SuccessResult();
        }

        public async Task<Result<RepeatedField<DateTimeValue>>> GetClientData(HourlyConsumptionRequest request)
        {
            // Нужен request_id

            var dbResponse = await _context.ClientDataRows.Where(x => x.RequestId == "").ToArrayAsync();

            var resultDb = new RepeatedField<DateTimeValue>();
            foreach (var row in dbResponse)
            {
                resultDb.Add(new DateTimeValue()
                {
                    DateTime = new DateTime()
                    {
                        Date = new Date()
                        {
                            Day = row.DateTime.Day,
                            Month = row.DateTime.Month,
                            Year = row.DateTime.Year,
                        },
                        Time = new Time()
                        {
                            Hour = row.DateTime.Hour,
                            Minute = row.DateTime.Minute,
                            Second = row.DateTime.Second,
                        }
                    },
                    Value = row.Value
                });
            }

            return new SuccessResult<RepeatedField<DateTimeValue>>(resultDb);
        }

        public async Task<Result<PredictedRow[]>> GetClientData(string requestId)
        {
            if(requestId == Guid.Empty.ToString()) return new ErrorResult<PredictedRow[]>("");
            var response = _context.PredictedRows.Where(row => row.RequestId == requestId).ToArray();

            return await Task.FromResult(new SuccessResult<PredictedRow[]>(response));
        }
    }
}
