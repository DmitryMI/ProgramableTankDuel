using UnityEngine;

namespace Assets.Scripts
{
    public class LoggerWrapper
    {
        //private readonly Logger _logger;
        private readonly TextPrinter _logger;

        public LoggerWrapper(TextPrinter logger)
        {
            _logger = logger;
        }

        public void Print(string message, Color color, bool bold, bool italic)
        {
            _logger.Print(message, color, bold, italic);
        }

        public void PrintRaw(string message)
        {
            _logger.Print(message);
        }

        public void Endl()
        {
            _logger.Endl();
        }
    }
}
