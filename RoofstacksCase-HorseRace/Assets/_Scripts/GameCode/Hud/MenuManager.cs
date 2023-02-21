using System.Collections.Generic;
using GameCode.Data;
using UnityEngine;

namespace GameCode.Hud
{
    public class MenuManager : MonoBehaviour, IMenuManager
    {
        private List<IMenu> _menus = new();

        public void OpenMenu(MenuType menuType)
        {
            if (_menus == null || _menus.Count == 0)
                CreateMenus();

            if (_menus == null) 
                return;
            
            foreach (var menu in _menus)
            {
                if (menu.MenuType == menuType)
                {
                    menu.Open();
                    continue;
                }

                menu.Close();
            }
        }

        private void CreateMenus()
        {
            _menus.Clear();
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent<IMenu>(out var menu))
                {
                    _menus.Add(menu);
                }
            }
        }
    }
}
