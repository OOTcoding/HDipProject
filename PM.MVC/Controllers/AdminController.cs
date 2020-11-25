using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PM.MVC.Models.EF;
using PM.MVC.ViewModels;

namespace PM.MVC.Controllers
{
    [Authorize(Roles = "Manager")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityResource> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<IdentityResource> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult UserManagement()
        {
            var users = _userManager.Users;
            return View(users);
        }

        public IActionResult AddUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(AddUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new IdentityResource { UserName = model.UserName, Email = model.Email };
            IdentityResult result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("UserManagement", _userManager.Users);
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }

            return View(model);
        }

        public async Task<IActionResult> EditUser(string userId)
        {
            IdentityResource user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return RedirectToAction("UserManagement", _userManager.Users);
            }

            var model = new EditUserViewModel { Id = user.Id, Email = user.Email, UserName = user.UserName };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            IdentityResource user = await _userManager.FindByIdAsync(model.Id);

            if (user != null)
            {
                user.UserName = model.UserName;
                user.Email = model.Email;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("UserManagement", _userManager.Users);
                }

                ModelState.AddModelError("", "User not updated, something went wrong");

                return View(model);
            }

            return RedirectToAction("UserManagement", _userManager.Users);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            IdentityResource user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("UserManagement", _userManager.Users);
                }

                ModelState.AddModelError("", "Something went wrong while deleting this user.");
            }
            else
            {
                ModelState.AddModelError("", "This user can't be found.");
            }

            return View("UserManagement", _userManager.Users);
        }

        public IActionResult RoleManagement()
        {
            IQueryable<IdentityRole> roles = _roleManager.Roles;
            return View(roles);
        }
        public IActionResult AddNewRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddNewRole(AddRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var role = new IdentityRole(model.RoleName);
            IdentityResult result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                return RedirectToAction("RoleManagement", _roleManager.Roles);
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }

            return View(model);
        }

        public async Task<IActionResult> EditRole(string id)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return RedirectToAction("RoleManagement", _roleManager.Roles);
            }

            var editRoleViewModel = new EditRoleViewModel { Id = role.Id, RoleName = role.Name, Users = new List<string>() };

            foreach (IdentityResource user in _userManager.Users)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    editRoleViewModel.Users.Add(user.UserName);
                }
            }

            return View(editRoleViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(model.Id);

            if (role != null)
            {
                role.Name = model.RoleName;

                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("RoleManagement", _roleManager.Roles);
                }

                ModelState.AddModelError("", "Role not updated, something went wrong.");
                return View(model);
            }

            return RedirectToAction("RoleManagement", _roleManager.Roles);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(id);

            if (role != null)
            {
                var result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("RoleManagement", _roleManager.Roles);
                }

                ModelState.AddModelError("", "Something went wrong while deleting the role.");
            }
            else
            {
                ModelState.AddModelError("", "This role can't be found.");
            }

            return RedirectToAction("RoleManagement", _roleManager.Roles);
        }

        public async Task<IActionResult> AddUserToRole(string roleId)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                return RedirectToAction("RoleManagement", _roleManager.Roles);
            }

            UserRoleViewModel model = await GetUserRoleViewModelNotInRole(role);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddUserToRole(UserRoleViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            var role = await _roleManager.FindByIdAsync(model.RoleId);

            var result = await _userManager.AddToRoleAsync(user, role.Name);

            if (result.Succeeded)
            {
                return RedirectToAction("RoleManagement", _roleManager.Roles);
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }

            return View(model);
        }

        public async Task<IActionResult> DeleteUserFromRole(string roleId)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                return RedirectToAction("RoleManagement", _roleManager.Roles);
            }

            UserRoleViewModel model = await GetUserRoleViewModel(role);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUserFromRole(UserRoleViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            var role = await _roleManager.FindByIdAsync(model.RoleId);

            var result = await _userManager.RemoveFromRoleAsync(user, role.Name);

            if (result.Succeeded)
            {
                return RedirectToAction("RoleManagement", _roleManager.Roles);
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }

            return View(model);
        }

        private async Task<UserRoleViewModel> GetUserRoleViewModel(IdentityRole role)
        {
            var model = new UserRoleViewModel { RoleId = role.Id };

            foreach (var user in _userManager.Users)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user);
                }
            }

            return model;
        }

        private async Task<UserRoleViewModel> GetUserRoleViewModelNotInRole(IdentityRole role)
        {
            var model = new UserRoleViewModel { RoleId = role.Id };

            foreach (IdentityResource user in _userManager.Users)
            {
                if (!await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user);
                }
            }

            return model;
        }
    }
}