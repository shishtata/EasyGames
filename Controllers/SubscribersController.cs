using EasyGames.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyGames.Controllers
{
    // SubscribersController manages subscriber-related actions
    public class SubscribersController : Controller
    {
        private readonly AppDbContext _db;

        public SubscribersController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            // Retrieve all subscribers from the database
            var subs = await _db.Subscribers.ToListAsync();
            return View(subs); // Pass the list of subscribers to the view
        }

    }
}
