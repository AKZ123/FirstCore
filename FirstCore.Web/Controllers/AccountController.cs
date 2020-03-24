using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirstCore.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FirstCore.Web.Controllers
{
    //Part: 66.2
    public class AccountController : Controller
    {
        //Part: 67.1
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        //Part: 67.2
        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false); //isPersistent: false for Session Cooce(Sign in Lost after browser close) & isPersistent: true for Permanent Cooce(Sign in also stay after browser close)
                    return RedirectToAction("index", "Home");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        //Part: 69.4
        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "Home");
        }

        //Part: 70.2
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        //Part: 70.4
        [HttpPost]
        public async Task<IActionResult> Login(UserLoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password,model.RememberMe, false); //(RememberMe) isPersistent: false for Session Cooce(Sign in Lost after browser close) & (RememberMe)isPersistent: true for Permanent Cooce(Sign in also stay after browser close)

                if (result.Succeeded)
                {
                    return RedirectToAction("index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");                
            }
            return View(model);
        }


    }
}
