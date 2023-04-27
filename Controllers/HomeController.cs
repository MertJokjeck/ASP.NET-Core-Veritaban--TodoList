using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _1.Models;
using _1.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace _1.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    MydatabaseContext db = new MydatabaseContext();

    public IActionResult Index()
    {
        return View();
    }

    [Route("/privacy")]
    public IActionResult Privacy()
    {
        return View();
    }

    [Route("/contact")]
    public IActionResult Contact()
    {
        return View();
    }

    [Authorize]
    [Route("/todolist")]
    public IActionResult Todolist()
    {
        var model = new TodoViewModel()
        {
            Todos = db.Todos!.OrderByDescending(x => x.Id).ToList(),
        };
        return View(model);
    }

    [Authorize]
    [HttpPost]
    [IgnoreAntiforgeryToken]
    [Route("/add-todo")]
    public IActionResult AddTodo(Todo postedData)
    {
        Todo toAdd = new Todo();
        toAdd.Title = postedData.Title;
        toAdd.IsComplated = false;
        db.Add(toAdd);
        db.SaveChanges();

        return Redirect("/todolist");
    }

    [Route("/delete-todo/{id}")]
    public IActionResult DeleteTodo(int id)
    {
        Todo toDelete = db.Todos!.Find(id)!;

        db.Remove(toDelete);
        db.SaveChanges();

        return Content("Kayıt başarıyla silindi.");
    }

    [Route("/update-todo/{id}")]
    public IActionResult UpdateTodo(int id)
    {
        Todo toUpdate = db.Todos!.Find(id)!;

        toUpdate.IsComplated = !toUpdate.IsComplated;
        db.Entry(toUpdate).CurrentValues.SetValues(toUpdate);
        db.SaveChanges();

        return Content(toUpdate.IsComplated.ToString());
    }

    [Route("/signin")]
    public IActionResult Signin()
    {
        return View();
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    [Route("/signin")]
    public async Task<IActionResult> Signin(User postedData)
    {
        User user = db.Users!.FirstOrDefault(
            x => x.Username == postedData.Username && x.Password == postedData.Password
        )!;

        if (user != null)
        {
            var claims = new List<Claim>()
            {
                new Claim("user", user.Id.ToString()),
                new Claim("role", "admin")
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            var claimsPrinciple = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(claimsPrinciple);

            return Redirect("/todolist");
        }
        else
        {
            TempData["Danger"] = "Hatalı kullanıcı adı ve ya şifre.";
            return Redirect("/signin");
        }
    }

    [Route("/signout")]
    public async Task<IActionResult> SignOut()
    {
        await HttpContext.SignOutAsync();
        TempData["Success"] = "çıkış yapıldı, tekrar gel.";
        return Redirect("/signin");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }
        );
    }
}
