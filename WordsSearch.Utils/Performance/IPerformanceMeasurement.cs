using System;

namespace WordsSearch.Utils.Performance
{
    public interface IPerformanceMeasurement
    {
        void Measure<TParam>(Action<TParam> action, TParam parameter, string startMessage = "", string endMessage = "");

        void Measure(Action action, string startMessage = "", string endMessage = "");

        void StartNew(string message = "");

        void End(string message);

        void Reset();

        /// <summary>
        /// Возвращает или задаёт активен ли замер затрачиваемого времени между вызовами методов <see cref="StartNew"/> и <see cref="End"/>.
        /// </summary>
        bool IsEnabled { get; set; }

        event LogHandler LogStartNew;

        event LogHandler LogEnd;
    }

    public delegate void LogHandler(IPerformanceMeasurement sender, string message);
}