using TechSto.Core.Models;

namespace TechSto.Core.Interfaces
{
    public interface IBrakeTesterService : IDisposable
    {

        /// <summary>
        /// Состояние подключения к устройству
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Подключение к тормозному стенду
        /// </summary>
        /// <param name="portName">Имя COM порта</param>
        Task<bool> ConnectAsync(string portName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Отключение от устройства
        /// </summary>
        Task DisconnectAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Запуск непрерывного чтения данных
        /// </summary>
        Task StartContinuousReadingAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Остановка непрерывного чтения
        /// </summary>
        void StopContinuousReading();

        /// <summary>
        /// Запрос одного измерения
        /// </summary>
        Task<bool> RequestMeasurementAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Запрос калибровочных данных
        /// </summary>
        Task<CalibrationData> RequestCalibrationDataAsync(CancellationToken cancellationToken = default);


        /// <summary>
        /// Быстрая проверка подключения
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> CheckAvailabilityAsync(string portName, CancellationToken cancellationToken = default);


        /// <summary>
        /// Событие изменения состояния подключения
        /// </summary>
        event EventHandler<DeviceConnectionEventArgs> ConnectionStateChanged;

        /// <summary>
        /// Событие получения измерения
        /// </summary>
        event EventHandler<MeasurementReceivedEventArgs> MeasurementReceived;

        /// <summary>
        /// Событие ошибки устройства
        /// </summary>
        event EventHandler<DeviceErrorEventArgs> ErrorOccurred;
    }
}