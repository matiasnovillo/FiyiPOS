namespace FiyiPOS.Services.FiyiPOS
{
    public class HotkeyService
    {
        public event Action<string>? KeyPressed;

        public void Dispatch(string key) => KeyPressed?.Invoke(key);
    }
}
