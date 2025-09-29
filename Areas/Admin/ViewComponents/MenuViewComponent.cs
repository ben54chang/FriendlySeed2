using FriendlySeed.Services;
using Microsoft.AspNetCore.Mvc;

namespace FriendlySeed.Areas.Admin.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly IMenuService _menuService;

        public MenuViewComponent(IMenuService menuService)
        {
            _menuService = menuService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var menus = await _menuService.GetMenuTreeAsync();
            return View(menus);
        }
    }
}
