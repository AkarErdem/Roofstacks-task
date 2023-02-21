using GameCode.Data;
using UnityEngine;

namespace GameCode.Hud
{
    public class Menu : MonoBehaviour, IMenu
    {
        [SerializeField] private MenuType _menuType;
        
        public MenuType MenuType => _menuType;
        
        public virtual void Open() => gameObject.SetActive(true);
        
        public virtual void Close() => gameObject.SetActive(false);
        
    }
}
