using GameCode.Data;

namespace GameCode.Hud
{
    public interface IMenu
    {
        MenuType MenuType { get; }
        public void Open();
        
        public void Close();
    }
}
