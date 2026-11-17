using FimCore.Models;

namespace FimCore.Interfaces
{
    public interface IAuditLogger
    {
        void LogEvent(IntegrityEvent integrityEvent);
    }
}