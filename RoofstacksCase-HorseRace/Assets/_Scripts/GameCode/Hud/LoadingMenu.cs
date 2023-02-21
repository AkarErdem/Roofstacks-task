namespace GameCode.Hud
{
    public class LoadingMenu : Menu
    {
        public override void Open() => gameObject.SetActive(true);
        
        public override void Close() => gameObject.SetActive(false);
    }
}
