using TechSto.Core.DTOs;
using TechSto.Core.Entities;

namespace TechSto.Core.Interfaces
{
    public interface ICheckService
    {
        /// <summary>
        /// Возвращает список всех проверок автообиля для DTO 
        /// </summary>
        /// <param name="carId">ID автомобиля</param>
        /// <returns></returns>
        List<CheckDto> GetChecksDtoByCarId(int carId);

        /// <summary>
        /// Добавляет запись о проверке с текущей датой для указанного автомобиля
        /// </summary>
        /// <param name="carId">ID автомобиля</param>
        /// <param name="carMileage">Пробег автомобиля на момент проверки</param>
        /// <param name="checkResult">Результат проверки (опционально)</param>
        /// <returns>Созданная запись Check</returns>
        Check AddCheck(int carId, int carMileage, CheckResultType? checkResult = null);

        /// <summary>
        /// Удаляет запись о проверке по её ID
        /// </summary>
        /// <param name="checkId">ID записи проверки</param>
        /// <returns>true - если удаление успешно, false - если запись не найдена</returns>
        bool DeleteCheck(int checkId);

        /// <summary>
        /// Удаляет все записи о проверках для указанного автомобиля
        /// </summary>
        /// <param name="carId">ID автомобиля</param>
        /// <returns>Количество удалённых записей</returns>
        int DeleteAllChecksByCarId(int carId);

        /// <summary>
        /// Получает все проверки для указанного автомобиля
        /// </summary>
        /// <param name="carId">ID автомобиля</param>
        /// <returns>Список проверок, отсортированный по дате (сначала новые)</returns>
        List<Check> GetChecksByCarId(int carId);

        /// <summary>
        /// Получает проверку по ID
        /// </summary>
        /// <param name="checkId">ID проверки</param>
        /// <returns>Запись проверки или null</returns>
        Check? GetCheckById(int checkId);
    }
}