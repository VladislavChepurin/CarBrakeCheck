using System;

namespace TechSto.DataBase.Entity
{
    public class DataCheck
    {
        public int Id { get; set; }
        public string Data { get; set; }               // дата проверки (может быть и string, если нужно)

        // Внешний ключ к владельцу
        public int OwnerId { get; set; }
        public virtual Owner Owner { get; set; }          // навигационное свойство
    }
}