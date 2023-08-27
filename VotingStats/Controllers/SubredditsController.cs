using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VotingStats.Controllers;

public class SubredditsController : Controller
{
    // GET: SubredditsController
    public ActionResult Index()
    {
        return View();
    }

    // GET: SubredditsController/Details/5
    public ActionResult Details(int id)
    {
        return View();
    }
}
