namespace TechSto.Core.DTOs
{
    public class CheckDto
    {
        public int Id { get; set; }
        public int Number { get; set; } // Порядковый номер
        public DateTime Date { get; set; }
        public string Result { get; set; } // "Пройдено" или "Не пройдено"
        public bool IsPassed { get; set; } // true - пройдено, false - не пройдено
    }
}
