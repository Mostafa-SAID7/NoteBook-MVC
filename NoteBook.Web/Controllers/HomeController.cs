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

    /// <summary>
    /// PageNotFound action - displays 404 page for missing routes
    /// </summary>
    public IActionResult PageNotFound()
    {
        Response.StatusCode = 404;
        ViewData["PageTitle"] = "Page Not Found";
        ViewData["RequestPath"] = Request.Path;
        return View("NotFound");
    }
}
