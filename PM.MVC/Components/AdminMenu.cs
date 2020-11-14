using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace PM.MVC.Components
{
    public class AdminMenu : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var menuItems = new List<AdminMenuItem>()
            {
                new AdminMenuItem { DisplayName = "User management", ActionValue = "UserManagement" },
                new AdminMenuItem { DisplayName = "Role management", ActionValue = "RoleManagement" },
            };

            return View(menuItems);
        }
    }

    public class AdminMenuItem
    {
        public string DisplayName { get; set; }

        public string ActionValue { get; set; }
    }
}