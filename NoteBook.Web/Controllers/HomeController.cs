using Microsoft.AspNetCore.Mvc;

namespace NoteBook.Web.Controllers;

/// <summary>
/// Home controller - entry point for the application
/// </summary>
public class HomeController : Controller
{
    /// <summary>
    /// Index action - displays the home page
    /// </summary>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Privacy action - displays the privacy policy
    /// </summary>
    public IActionResult Privacy()
    {
        ViewData["PageTitle"] = "Privacy Policy";
        return View();
    }
}
