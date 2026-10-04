using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _23_phanDangQuang_Assignment01_FrontEnd.Models;

namespace _23_phanDangQuang_Assignment01_FrontEnd.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (User.IsInRole(AppRoles.Lecturer)) return RedirectToAction("Index", "Lecturer");
        return RedirectToAction("Index", "News");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
