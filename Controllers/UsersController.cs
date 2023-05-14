using Autotest.Mvc.Models;
using Autotest.Mvc.Services;
using Microsoft.AspNetCore.Mvc;

namespace Autotest.Mvc.Controllers;

public class UsersController : Controller
{
    private readonly UsersService _usersService;
    private readonly QuestionService _questionService;
    public UsersController(UsersService usersService, QuestionService questionService)
    {
        _usersService = usersService;
        _questionService = questionService;
    }
    [HttpGet]
    public IActionResult SignUp()
    {
        return View();
    }

    [HttpPost]
    public IActionResult SignUp(CreateUserModel createUser)
    {
        if (!ModelState.IsValid)
        {
            return View(createUser);
        }

        _usersService.Register(createUser, HttpContext);

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult SignIn()
    {
        return View();
    }

    [HttpPost]
    public IActionResult SignIn(SignInUserModel signInUserModel)
    {
        if (!ModelState.IsValid)
        {
            return View(signInUserModel);
        }

        var isLogin = _usersService.LogIn(signInUserModel, HttpContext);

        if (!isLogin)
        {
            ModelState.AddModelError("Username", "Username or Password is incorrect");
            return View();
        }

        return RedirectToAction("Profile");
    }

    public IActionResult Profile()
    {
        var user = _usersService.GetCurrentUser(HttpContext);

        if (user == null)
        {
            return RedirectToAction("SignUp");
        }

        return View(user);
    }

    public IActionResult LogOut()
    {
        _usersService.LogOut(HttpContext);

        return RedirectToAction("SignIn");
    }

    public IActionResult ChangeLanguage(string language)
    {
        _questionService.LoadJson(language);

        return RedirectToAction("Index", "Home");
    }
}