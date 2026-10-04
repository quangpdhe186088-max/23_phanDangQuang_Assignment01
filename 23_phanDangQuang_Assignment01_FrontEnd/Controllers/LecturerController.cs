using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using _23_phanDangQuang_Assignment01_FrontEnd.Models;

namespace _23_phanDangQuang_Assignment01_FrontEnd.Controllers;

[Authorize(Roles = AppRoles.Lecturer)]
public sealed class LecturerController : AuthenticatedController
{
    [HttpGet]
    public IActionResult Index() => View();
}
