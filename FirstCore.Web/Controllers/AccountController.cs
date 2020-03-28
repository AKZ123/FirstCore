using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirstCore.Web.Models;
using FirstCore.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FirstCore.Web.Controllers
{
    //Part: 66.2
    public class AccountController : Controller
    {
        //Part: 67.1,  77.2.3
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        //Part: 75.1
        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsEmailIsUse(string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {email} is already use");
            }
        }

        //Part: 67.2
        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                //var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                //Part:        77.2.1,                                                       77.4.3
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, City=model.City };
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

        //Part: 70.4,  72
        [HttpPost]
        public async Task<IActionResult> Login(UserLoginViewModel model, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password,model.RememberMe, false); //(RememberMe) isPersistent: false for Session Cooce(Sign in Lost after browser close) & (RememberMe)isPersistent: true for Permanent Cooce(Sign in also stay after browser close)

                if (result.Succeeded)
                {
                    //Part: 72,  73
                    if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                        //return LocalRedirect(ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("index", "Home");
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");                
            }
            return View(model);
        }

        //Part: 83.2
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
