using FinalProject_E_Wallet.Data;
using Microsoft.AspNetCore.Mvc;
using FinalProject_E_Wallet.Models.ViewModels;
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
        public IActionResult Dashboard()
        {
            var user = GetCurrentUser();

            if (user == null)
                return RedirectToAction("Login", "Account");

            var recentTransactions = _context.Transactions
                .Where(t => t.SenderId == user.Id || t.ReceiverId == user.Id)
                .OrderByDescending(t => t.Date)
                .Take(5)
                .ToList();

            ViewBag.RecentTransactions = recentTransactions;

            return View(user);
        }
        public IActionResult CashIn()
        {
            var user = GetCurrentUser();

            if (user == null)
            {
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
        public IActionResult Send()
        {
            var currentUser = GetCurrentUser();

            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // EXCLUDE CURRENT USER
            var users = _context.Users
                .Where(u => u.Id != currentUser.Id)
                .ToList();

            return View(users);
        }
        [HttpPost]
        public IActionResult Send(string receiverEmail, decimal amount)
        {
            var sender = GetCurrentUser();

            if (sender == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var receiver = _context.Users
                .FirstOrDefault(u => u.Email == receiverEmail);

            if (receiver == null)
            {
                TempData["Error"] = "User not found!";
                return RedirectToAction("Send");
            }

            if (receiver.Id == sender.Id)
            {
                TempData["Error"] = "Cannot send money to yourself!";
                return RedirectToAction("Send");
            }

            if (amount <= 0)
            {
                TempData["Error"] = "Invalid Amount!";
                return RedirectToAction("Send");
            }

            if (sender.Balance < amount)
            {
                TempData["Error"] = "Insufficient Balance!";
                return RedirectToAction("Send");
            }

            // UPDATE BALANCES
            sender.Balance -= amount;
            receiver.Balance += amount;

            // CREATE TRANSACTION
            var transaction = new Transaction
            {
                SenderId = sender.Id,
                ReceiverId = receiver.Id,
                Amount = amount,
                Type = "Send",
                Date = DateTime.Now
            };

            _context.Transactions.Add(transaction);

            _context.SaveChanges();

            // REDIRECT TO RECEIPT
            return RedirectToAction("Receipt", new { id = transaction.Id });
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
        public IActionResult Receipt(int id)
        {
            var transaction = _context.Transactions
                .FirstOrDefault(t => t.Id == id);

            if (transaction == null)
            {
                TempData["Error"] = "Transaction not found!";
                return RedirectToAction("Dashboard");
            }

            var sender = _context.Users
                .FirstOrDefault(u => u.Id == transaction.SenderId);

            var receiver = _context.Users
                .FirstOrDefault(u => u.Id == transaction.ReceiverId);

            ViewBag.Sender = sender;
            ViewBag.Receiver = receiver;

            return View(transaction);
        }

        [HttpGet]
        public IActionResult EditProfile()
        {
            var user = GetCurrentUser();

            if (user == null)
                return RedirectToAction("Login", "Account");

            var model = new EditProfileViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                ProfilePicturePath = user.ProfilePicturePath
            };

            return View(model); // ✅ MUST BE VIEWMODEL
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model, IFormFile? profileImage)
        {
            var user = GetCurrentUser();

            if (user == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                var errors = ModelState
        .Values
        .SelectMany(v => v.Errors)
        .Select(e => e.ErrorMessage)
        .ToList();

                Console.WriteLine(string.Join("\n", errors));
                return View(model);
            }

            // UPDATE BASIC INFO
            user.FirstName = model.FirstName;
            user.MiddleName = model.MiddleName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;

            Console.WriteLine("Password: " + model.NewPassword);

            // PASSWORD UPDATE (ONLY IF USER ENTERS NEW ONE)
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                Console.WriteLine("Password: " + model.NewPassword);
                if (model.NewPassword != model.ConfirmPassword)
                {
                    ModelState.AddModelError("", "Passwords do not match!");
                    TempData["Error"] = "Password does not match!";
                    Console.WriteLine("Password does not match!");
                    return View(model);
                }

                user.Password = model.NewPassword; // (you should hash this later)
            }

            // IMAGE UPLOAD
            if (profileImage != null && profileImage.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string fileName = $"{user.Id}{Path.GetExtension(profileImage.FileName)}";
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profileImage.CopyToAsync(stream);
                }

                user.ProfilePicturePath = $"/uploads/{fileName}";
                Console.WriteLine($"/uploads/{fileName}");

            }

            _context.SaveChanges();

            return RedirectToAction("Dashboard");
        }
    }
}
