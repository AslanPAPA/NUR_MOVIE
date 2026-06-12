using NUR.Data;
using NUR.Models;

namespace NUR.Services
{
    public static class HistorySyncService
    {
        // 🔥 событие для мгновенного обновления UI
        public static event Action? HistoryChanged;

        // =========================
        // SYNC SERVER → LOCAL DB
        // =========================
        public static async Task SyncAsync()
        {
            try
            {
                if (!await InternetHelper.HasInternet())
                    return;

                var serverHistory = await HistoryService.GetHistory();

                if (serverHistory == null)
                    return;

                foreach (var movie in serverHistory)
                {
                    DatabaseService.SaveHistory(movie);
                }

                // 🔥 уведомляем UI
                HistoryChanged?.Invoke();
            }
            catch
            {
                // можно логировать, но не ломаем приложение
            }
        }

        // =========================
        // ADD SINGLE ITEM (FAST UPDATE)
        // =========================
        public static void AddLocal(Movie movie)
        {
            if (movie == null) return;

            DatabaseService.SaveHistory(movie);

            // 🔥 мгновенное обновление UI
            HistoryChanged?.Invoke();
        }
    }
}