using FinTracker.Models;

namespace FinTracker.Services
{
    public class NotificationService
    {
        public List<NotificationViewModel> GetNotifications()
        {
            var today = DateTime.Today;
            var notifications = new List<NotificationViewModel>();

            var events = new List<(DateTime Date, string Message)>
            {
                (new DateTime(today.Year, today.Month, 5), "💰 Выплата аванса"),
                (new DateTime(today.Year, today.Month, 6), "💸 Уплата НДФЛ за аванс"),
                (new DateTime(today.Year, today.Month, 20), "💰 Выплата зарплаты"),
                (new DateTime(today.Year, today.Month, 21), "💸 Уплата НДФЛ за зарплату"),
                (new DateTime(today.Year, today.Month, 15), "📑 Начать подготовку окончательного расчёта по зарплате"),
                (new DateTime(today.Year, today.Month, 18), "📑 Завершить подготовку расчёта по зарплате"),
                (new DateTime(today.Year, today.Month, 1), "⏰ Начать формирование аванса и проверку табеля"),
                (new DateTime(today.Year, today.Month, 4), "⏰ Завершить формирование аванса и проверку табеля"),
                (GetNextQuarterEnd(today), "📊 Конец квартала: рассчитать УСН/НДС/прибыль"),
                (new DateTime(today.Year, today.Month, 15), "🛡 Уплата страховых взносов за прошлый месяц")
            };

            foreach (var evt in events)
            {
                int daysLeft = (evt.Date - today).Days;
                
                if (daysLeft < -1 || daysLeft > 7) continue; 
                

                string prefix = daysLeft switch
                {
                    > 1 => $"⏳ Через {daysLeft} д. : ",
                    1 => $"⏳ Завтра: ",
                    0 => $"📅 Сегодня: ",
                    -1 => "❗ Вчера: ",
                    _ => ""
                };

                notifications.Add(new NotificationViewModel
                {
                    Date = evt.Date,
                    Message = prefix + evt.Message
                });
            }

            return notifications;
        }

        private DateTime GetNextQuarterEnd(DateTime today)
        {
            int month = today.Month;
            int year = today.Year;

            return month switch
            {
                <= 3 => new DateTime(year, 3, 31),
                <= 6 => new DateTime(year, 6, 30),
                <= 9 => new DateTime(year, 9, 30),
                _ => new DateTime(year, 12, 31)
            };
        }
    }
}
