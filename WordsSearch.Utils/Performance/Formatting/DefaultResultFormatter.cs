using System;
using WordsSearch.Utils.Performance.Formatting.Interfaces;

namespace WordsSearch.Utils.Performance.Formatting
{
    /// <summary>
    /// Класс, который реализует методы для форматирования затраченного времени на какую-либо операцию.
    /// </summary>
    public class DefaultResultFormatter : IResultFormatter
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
        public string Format(TimeSpan stopwatchElapsed, long stopwatchElapsedMs, long stopwatchElapsedTicks, string message)
        {
            return string.Format("{0}: {1} ms, {2} ticks", message, stopwatchElapsedMs, stopwatchElapsedTicks);
        }
    }
}
