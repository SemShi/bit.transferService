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
            _logger.LogInformation("Пытаемся сохранить коэффициенты в базу..");
            try
            {
                await _context.Сoefficients.AddAsync(model);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                UndetouchEntities(ex);
                return new ErrorResult("Ошибка при сохранинии в базху данных");
            }
            _logger.LogInformation("Данные сохранены!");
            return new SuccessResult();
        }

        public async Task<Result> SavePrediction(SaveHourlyConsumptionRequest request)
        {
            _logger.LogInformation("Пытаемся сохранить спрогнозированные данные..");
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
            catch (Exception ex)
            {
                UndetouchEntities(ex);
                return new ErrorResult("Ошибка при сохранинии в базху данных");
            }
            _logger.LogInformation("Данные сохранены!");
            return new SuccessResult();
        }

        private void UndetouchEntities(Exception ex)
        {
            _logger.LogError(ex, "Ошибка при сохранении в базу данных");
            var undetachedEntriesCopy = _context.ChangeTracker.Entries()
                .Where(e => e.State != EntityState.Detached)
                .ToList();

            foreach (var entry in undetachedEntriesCopy)
                entry.State = EntityState.Detached;
        }

        public async Task<Result<RequestСoefficientMinMax>> GetCoefficientsByRequestId(string requestId)
        {
            _logger.LogInformation("Пытаемся получить данные по коэффициентам. RequestId: {0}", requestId);
            if(requestId == Guid.Empty.ToString()) return new ErrorResult<RequestСoefficientMinMax>("");
            var response = 
                await _context.Сoefficients
                .FindAsync(requestId);
            if(response != null)
                _logger.LogInformation("Данные получены!");
            else
                _logger.LogWarning("Не удалось найти данные по коэффициентам. RequestId: {0}", requestId);

            return response is null
                ? new ErrorResult<RequestСoefficientMinMax>("")
                : new SuccessResult<RequestСoefficientMinMax>(response);
        }

        public async Task<Result> SaveClientData(CentralServerRequest request)
        {
            _logger.LogInformation("Пытаемся сохранить набор данных от клиентского сервиса..");
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
                UndetouchEntities(ex);
                return new ErrorResult(ex.Message);
            }
            _logger.LogInformation("Данные сохранены!");
            return new SuccessResult();
        }

        public async Task<Result<RepeatedField<DateTimeValue>>> GetClientData(HourlyConsumptionRequest request)
        {
            // Нужен request_id
            _logger.LogInformation("Пытаемся получить набор данных от клиентского сервиса..");
            var dbResponse = await _context.ClientDataRows.Where(x => x.RequestId == "").ToArrayAsync();

            if (!dbResponse.Any())
            {
                _logger.LogWarning("Не удалось найти данные.");
                return new ErrorResult<RepeatedField<DateTimeValue>>("Не удалось найти данные.");
            }
                

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
    }
}
