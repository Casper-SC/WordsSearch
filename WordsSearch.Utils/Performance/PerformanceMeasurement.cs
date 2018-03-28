using System;
using System.Diagnostics;
using WordsSearch.Utils.Performance.Formatting;
using WordsSearch.Utils.Performance.Formatting.Interfaces;

namespace WordsSearch.Utils.Performance
{
    /// <summary>
    /// Класс для замера времени, которое проходит между вызовами методов <see cref="StartNew"/> и <see cref="End"/>.
    /// Предполагается использование одного экземпляра для последовательного замера в одном потоке. 
    /// После каждого вызова <see cref="StartNew"/> необходимо вызывать <see cref="End"/>.
    /// 
    /// Примеры использования.
    /// Вариант 1:
    /// <code>
    /// _performance.StartNew("-----------");
    /// try
    /// {
    ///     // Код, время выполнения которого нужно замерить.
    /// }
    /// finally
    /// {
    ///     _performance.End("Замер такой-то");
    /// }
    /// </code>
    /// 
    /// Вариант 2:
    /// <code>
    /// _performance.Measure((someParam) =>
    /// {
    ///     // Код, время выполнения которого нужно замерить.
    /// },
    /// someParameter, startMessage: "-----------", endMessage: "Замер такой-то");
    /// </code>
    /// 
    /// </summary>
    public class PerformanceMeasurement : IPerformanceMeasurement
    {
        private readonly Stopwatch _stopwatch;
        private bool _isEnabled;
        private readonly IResultFormatter _resultFormatter;

        public PerformanceMeasurement(IResultFormatter resultFormatter = null)
        {
            _resultFormatter = resultFormatter ?? new DefaultResultFormatter();

            _stopwatch = new Stopwatch();
            _isEnabled = true;
        }

        /// <summary>
        /// Возвращает или задаёт активен ли замер затрачиваемого времени между вызовами методов <see cref="StartNew"/> и <see cref="End"/>.
        /// </summary>
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }

        /// <summary>
        /// Обработчик, который будет вызываться при срабатывании метода. <see cref="StartNew"/>. 
        /// </summary>
        public event LogHandler LogStartNew;

        /// <summary>
        /// Обработчик, в который будет передаваться сформированное сообщение о затраченном времени
        /// между вызовами методов <see cref="StartNew"/> и <see cref="End"/> во время вызова метода <see cref="End"/>. 
        /// </summary>
        public event LogHandler LogEnd;

        public void Measure<TParam>(Action<TParam> action, TParam parameter, string startMessage = "", string endMessage = "")
        {
            StartNew(startMessage);

            try
            {
                action(parameter);
            }
            finally
            {
                End(endMessage);
            }
        }

        public void Measure(Action action, string startMessage = "", string endMessage = "")
        {
            StartNew(startMessage);

            try
            {
                action();
            }
            finally
            {
                End(endMessage);
            }
        }

        public void StartNew(string message = "")
        {
            if (!_isEnabled)
            {
                return;
            }

            OnLogStartNew(message);

            if (_stopwatch.IsRunning)
            {
                throw new Exception("Stopwatch is already running");
            }

            _stopwatch.Reset();
            _stopwatch.Start();
        }

        public void End(string headerOfMessage)
        {
            if (!_isEnabled)
            {
                return;
            }

            if (!_stopwatch.IsRunning)
            {
                return;
            }

            _stopwatch.Stop();

            OnLogEnd(_resultFormatter.Format(_stopwatch.Elapsed, _stopwatch.ElapsedMilliseconds, _stopwatch.ElapsedTicks, headerOfMessage));
        }

        public void Reset()
        {
            _stopwatch.Stop();
        }

        protected void OnLogEnd(string message)
        {
            var handler = LogEnd;
            if (handler != null)
            {
                handler(this, message);
            }
        }

        protected void OnLogStartNew(string message)
        {
            var handler = LogStartNew;
            if (handler != null)
            {
                handler(this, message);
            }
        }
    }
}