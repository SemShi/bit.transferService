using Google.Protobuf.Collections;
using gRPC.Common.Protos;
using Microsoft.Extensions.Logging;
using Guid = System.Guid;

namespace Server.Core.Services
{
    public class NormalizeDataService : INormalizeDataService
    {
        private readonly ILogger<NormalizeDataService> _logger;

        public NormalizeDataService(
            ILogger<NormalizeDataService> logger)
        {
            _logger = logger;
        }


        public async Task<Result<RequestСoefficientMinMax>> GetMinMaxDictionary(RepeatedField<DateTimeValue> data)
        {
            _logger.LogInformation("Извлекаем коэффициенты из набора входных данных..");
            if (!data.Any())
            {
                _logger.LogWarning("Массив данных оказался пуст.");
                return await Task.FromResult(new ErrorResult<RequestСoefficientMinMax>("Массив входных данных не содержит данных."));
            }

            var min = data.Select(x => x.Value).ToArray().Min();
            var max = data.Select(x => x.Value).ToArray().Max();

            _logger.LogInformation("Данные извлечены. Min: {0}; Max: {1}", min, max);
            return await Task.FromResult(new SuccessResult<RequestСoefficientMinMax>(new RequestСoefficientMinMax()
            {
                Min = min,
                Max = max,
            }));
        }

        public async Task<Result<RepeatedField<DateTimeValue>>> Normalize(RepeatedField<DateTimeValue> data, RequestСoefficientMinMax parameters)
        {
            _logger.LogInformation("Пытаемся выполнить нормализацию входных данных..");
            if (!data.Any())
            {
                _logger.LogWarning("Массив данных оказался пуст.");
                return new ErrorResult<RepeatedField<DateTimeValue>>("Массив входных данных не содержит данных.");
            }

            var resultArray = new RepeatedField<DateTimeValue>();
            foreach (var dateTimeValue in data)
            {
                if (dateTimeValue.Value == 0)
                {
                    resultArray.Add(dateTimeValue);
                    continue;
                }

                dateTimeValue.Value = (dateTimeValue.Value - parameters.Min) / (parameters.Max - parameters.Min);
                resultArray.Add(dateTimeValue);
            }

            _logger.LogInformation("Нормализация выполнена.");
            return await Task.FromResult(new SuccessResult<RepeatedField<DateTimeValue>>(resultArray));
        }

        public async Task<Result<RepeatedField<DateTimeValue>>> Denormalize(RepeatedField<DateTimeValue> data, RequestСoefficientMinMax parameters)
        {
            _logger.LogInformation("Пытаемся выполнить денормализацию входных данных..");
            if (!data.Any())
            {
                _logger.LogWarning("Массив данных оказался пуст.");
                return new ErrorResult<RepeatedField<DateTimeValue>>("Массив входных данных не содержит данных.");
            }

            if(!parameters.ValidateModel())
                return new ErrorResult<RepeatedField<DateTimeValue>>("Отсутствуют коэффициенты для процесса денормализации данных.");

            var resultArray = new RepeatedField<DateTimeValue>();

            foreach (var dateTimeValue in data)
            {
                dateTimeValue.Value = dateTimeValue.Value * (parameters.Max - parameters.Min) + parameters.Min;
                resultArray.Add(dateTimeValue);
            }

            _logger.LogInformation("Денормализация выполнена.");
            return await Task.FromResult(new SuccessResult<RepeatedField<DateTimeValue>>(resultArray));
        }
    }
}
