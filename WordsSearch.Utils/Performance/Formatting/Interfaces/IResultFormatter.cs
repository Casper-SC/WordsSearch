using System;

namespace WordsSearch.Utils.Performance.Formatting.Interfaces
{
    /// <summary>
    /// Интерфейс, который предоставляет методы для форматирования затраченного времени на какую-либо операцию.
    /// </summary>
    public interface IResultFormatter
    {
        /// <summary>
        /// Форматировать вывод информации о затраченном времени. 
        /// Stopwatch не передаётся в метод, чтобы не было возможности манипулировать его работой в реализации этого метода.
        /// </summary>
        /// <param name="stopwatchElapsed"> Общее затраченное время в тактах таймера, измеренное экземпляром <code>Stopwatch</code>.</param>
        /// <param name="stopwatchElapsedMs"> Общее затраченное время в миллисекундах, измеренное экземпляром <code>Stopwatch</code>.</param>
        /// <param name="stopwatchElapsedTicks"> Общее затраченное время в тактах таймера, измеренное экземпляром <code>Stopwatch</code>.</param>
        /// <param name="message"> Сообщение. </param>
        /// <returns></returns>
        string Format(TimeSpan stopwatchElapsed, long stopwatchElapsedMs, long stopwatchElapsedTicks, string message);
    }
}