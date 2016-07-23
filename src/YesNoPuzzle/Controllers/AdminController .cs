using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YesNoPuzzle.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YesNoPuzzle.Models;
using YesNoPuzzle.Models.GameViewModels;

namespace YesNoPuzzle.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;

        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> DeleteGame(int? id)
        {
            if (id == null)
                return NotFound();

            var game = _db.Games.First(g => g.Id == id);

            var questions = _db.Questions.Where(q => q.Game == game).ToList();

            foreach (var q in questions)
            {
                _db.Questions.Remove(q);
            }

            _db.Games.Remove(game);

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index), "Admin");
        }

        public async Task<IActionResult> GameOver(int? id)
        {
            if (id == null)
                return NotFound();

            var game = _db.Games.First(g => g.Id == id);

            game.GameState = false;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index), "Admin");
        }


        public async Task<IActionResult> CreateNewGame()
        {
            return View();
        }

        public async Task<IActionResult> AddNewGame(GameViewModel model)
        {
            _db.Games.Add(new Game()
            {
                GameName = model.GameName,
                GameCondition = model.GameCondition,
                GameState = true,
                User = await _userManager.GetUserAsync(HttpContext.User)
            });
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index), "Admin");
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
            {
                var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var games = await _db.Games.Where(u => u.User.Id == userId).ToListAsync();

                return View(games);
            }
            return View();    
        }

        public async Task<IActionResult> AnswerYes(int? id)
        {
            if (id == null)
                return NotFound();

            Question question = _db.Questions.First(q => q.Id == id);

            question.State = 1;

            await _db.SaveChangesAsync();

            return RedirectToAction("Question", "Admin");
        }

        public async Task<IActionResult> AnswerNo(int? id)
        {
            if (id == null)
                return NotFound();

            Question question = _db.Questions.First(q => q.Id == id);

            question.State = 2;

            await _db.SaveChangesAsync();

            return RedirectToAction("Question", "Admin");
        }

        public async Task<IActionResult> AnswerNoMatter(int? id)
        {
            if (id == null)
                return NotFound();

            Question question = _db.Questions.First(q => q.Id == id);

            question.State = 3;

            await _db.SaveChangesAsync();

            return RedirectToAction("Question", "Admin");
        }

        public async Task<IActionResult> Question()
        {           
                var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var games = await _db.Games
                    .Include(g => g.Questions)
                    .Where(g => g.User.Id == userId && g.GameState)
                    .ToListAsync();


                foreach (var t in games)
                    t.Questions = t.Questions.Where(q => q.State == 0).ToList();

                return View(games);            
        }

        public IActionResult Rules()
        {
            ViewData["Message"] = "YesNo Puzzle.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Our contact page.";

            return View();
        }       
    }
}
