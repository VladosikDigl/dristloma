namespace FinTracker.Models
{
    public class HomeViewModel
    {
        public List<string> Reminders { get; set; } // Список напоминаний
        public Dictionary<string, string> Buttons { get; set; } // Кнопки (текст -> стиль)
        public ChartData ChartData { get; set; } // Данные для графика
    }

    public class ChartData
    {
        public List<string> Labels { get; set; }
        public List<int> Values { get; set; }
        public string BackgroundColor { get; set; } = "#36a2eb";
    }
}
