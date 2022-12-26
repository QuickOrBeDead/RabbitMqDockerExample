namespace AuditLogWorker.Infrastructure.Service
{
    public interface IAuditService
    {
        void Log(string message);
    }

    public sealed class AuditLogService : IAuditService
    {
        private static readonly IList<string> _logs = new List<string>();

        public void Log(string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            _logs.Add(message);
        }
    }
}
