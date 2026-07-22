namespace FiyiPOS.Services.FiyiPOS
{
    public class SystemStatusService
    {
        private bool _isOnline = true;
        private string _message = "En línea";
        private DateTime _lastCheckedAt = DateTime.Now;

        public bool IsOnline => _isOnline;
        public string Message => _message;
        public DateTime LastCheckedAt => _lastCheckedAt;

        public event Action? StatusChanged;

        public void SetOnline()
        {
            bool changed = !_isOnline;
            _isOnline = true;
            _message = "En línea";
            _lastCheckedAt = DateTime.Now;
            if (changed) StatusChanged?.Invoke();
        }

        public void SetOffline(string reason = "Sin conexión a la base de datos")
        {
            bool changed = _isOnline;
            _isOnline = false;
            _message = reason;
            _lastCheckedAt = DateTime.Now;
            if (changed) StatusChanged?.Invoke();
        }
    }
}
