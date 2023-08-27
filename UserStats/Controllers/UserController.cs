using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UserStats.Controllers;

public class UserController : Controller
{
    // GET: UserController
    public ActionResult Index()
    {
        return View();
    }

    // GET: UserController/Details/5
    public ActionResult Details(int id)
    {
        return View();
    }
}
