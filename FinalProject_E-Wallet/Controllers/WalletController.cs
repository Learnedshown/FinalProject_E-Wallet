using FinalProject_E_Wallet.Data;
using Microsoft.AspNetCore.Mvc;
using FinalProject_E_Wallet.Models;

namespace FinalProject_E_Wallet.Controllers
{
    public class WalletController : Controller
    {
        private readonly AppDbContext _context;
        public WalletController(AppDbContext context)
        {
            _context = context;
        }
        private User GetCurrentUser()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId.HasValue)
            {
                return _context.Users.FirstOrDefault(u => u.Id == userId.Value);
            }
            TempData["Error"] = "Please Login First!";
            return null;
        }
        public ActionResult Dashboard()
        {
            var user = GetCurrentUser();
            if (user == null)
            {
                TempData["Error"] = "Please Login First!";
                return RedirectToAction("Login", "Account");
            }
            return View(user);
        }
        [HttpPost]
        public IActionResult CashIn(decimal amount)
        {
            var user = GetCurrentUser();
            if (amount <= 0)
            {
                ViewBag.Error = "Invalid Amount!";
                TempData["Error"] = "Invalid Amount!";
                return RedirectToAction("Dashboard");

            }
            user.Balance += amount;
            _context.Transactions.Add(new Transaction
            {
                SenderId = user.Id,
                ReceiverId = user.Id,
                Amount = amount,
                Type = "Cash In",
                Date = DateTime.Now
            });
            _context.SaveChanges();
            TempData["Success"] = "Cash In Successful!";
            return RedirectToAction("Dashboard");
        }
        [HttpPost]
        public IActionResult Send(string receiverEmail, decimal amount)
        {
            var sender = GetCurrentUser();
            var receiver = _context.Users.FirstOrDefault(u => u.Email == receiverEmail);
            if (receiver == null)
            {
                ViewBag.Error = "User not found!";
                TempData["Error"] = "User not found!";
                return RedirectToAction("Dashboard");
            }
            if (receiver.Id == sender.Id)
            {
                ViewBag.Error = "Cannot send money to yourself!";
                TempData["Error"] = "Cannot send money to yourself!";
                return RedirectToAction("Dashboard");
            }
            if (amount <= 0)
            {
                ViewBag.Error = "Invalid Amount!";
                TempData["Error"] = "Invalid Amount!";
                return RedirectToAction("Dashboard");
            }
            if (sender.Balance < amount)
            {
                ViewBag.Error = "Insufficient Balance!";
                TempData["Error"] = "Insufficient Balance!";
                return RedirectToAction("Dashboard");
            }

            sender.Balance -= amount;
            receiver.Balance += amount;

            _context.Transactions.Add(new Transaction
            {
                SenderId = sender.Id,
                ReceiverId = receiver.Id,
                Amount = amount,
                Type = "Send",
                Date = DateTime.Now
            });
            _context.SaveChanges();
            TempData["Success"] = "Sent Successfully!";
            return RedirectToAction("Dashboard");
        }
        public IActionResult History()
        {
            var user = GetCurrentUser();
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var transactions = _context.Transactions
                .Where(t => t.SenderId == user.Id || t.ReceiverId == user.Id)
                .OrderByDescending(t => t.Date)
                .ToList();

            // Get all users
            var users = _context.Users.ToList();

            // Pass users to view
            ViewBag.Users = users;

            return View(transactions);
        }
    }
}
