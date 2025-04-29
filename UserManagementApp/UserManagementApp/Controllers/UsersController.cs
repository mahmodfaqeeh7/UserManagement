using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using UserManagementApp.Data;
using UserManagementApp.Models;

namespace UserManagementApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManagementAppContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public UsersController(UserManagementAppContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment; // Injected here

        }

        // GET: Users
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 15)
        {
            // Calculate total number of users and pages
            var totalUsers = await _context.User.CountAsync();
            var totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

            // Retrieve the paginated users, ordered by the newest first
            var users = await _context.User
                                      .OrderByDescending(u => u.Id)// If you have a CreatedAt field
                                                                   //.OrderByDescending(u => u.Id) // Alternatively, order by ID if no CreatedAt
                                      .Skip((pageNumber - 1) * pageSize)
                                      .Take(pageSize)
                                      .ToListAsync();

            // Create the ViewModel with users and pagination info
            var model = new UserViewModel
            {
                Users = users,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                PageSize = pageSize
            };

            return View(model);
        }





        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,MobileNumber,Password,PhotoFile")] User user)
        {

            
                Console.WriteLine("test2");

                if (user.PhotoFile != null && user.PhotoFile.Length > 0) // Check if a file is uploaded
                {
                    Console.WriteLine("test3 - found photo");

                    // 1- Get the root path
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string path = Path.Combine(wwwRootPath + "/Images/");

                    // Ensure the directory exists
                    if (!Directory.Exists(path))
                    {
                        Console.WriteLine("test4 - no dic");

                        Directory.CreateDirectory(path);
                    }

                    // 2- Generate a unique file name
                    string fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(user.PhotoFile.FileName);
                    string fullPath = Path.Combine(path, fileName);

                    // 3- Upload the image to the specified folder
                    try
                    {
                        using (var fileStream = new FileStream(fullPath, FileMode.Create))
                        {
                            Console.WriteLine("test5 - uploading");

                            await user.PhotoFile.CopyToAsync(fileStream);
                        }

                        // Save the file name in the 'PhotoPath' property of the user
                        user.Photo= fileName;

                        // Log the success
                        Console.WriteLine($"File uploaded successfully: {fullPath}");
                    }
                    catch (Exception ex)
                    {
                        // Log the error
                        Console.WriteLine($"Error uploading file: {ex.Message}");
                        ModelState.AddModelError("", "Unable to upload the file. Please try again.");
                        return View(user);
                    }
                }
                else
                {
                    ModelState.AddModelError("PhotoFile", "Please select a file to upload.");
                }

                // Add the user to the database
                _context.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            

           
        }




        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,MobileNumber,Password,PhotoFile")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user != null)
            {
                _context.User.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login([Bind("Email,Password")] User model)
        {
            Console.WriteLine(model.Email);
            
                var user = _context.User
                    .FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);

                if (user != null)
                {
                    // Login successful, redirect to User List
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Invalid username or password.");
            
            return View(model);
        }


        public ActionResult Import()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Import(IFormFile file)
        {
            if (file != null && file.FileName.EndsWith(".xlsx"))
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                        var worksheet = package.Workbook.Worksheets.First();
                        var rowCount = worksheet.Dimension.Rows;

                        var users = new List<User>();
                        var invalidUsers = new List<string>(); // To keep track of invalid users

                        // Loop through each row sequentially (single-threaded)
                        for (int row = 2; row <= rowCount; row++)
                        {
                            var name = worksheet.Cells[row, 2].Text;
                            var email = worksheet.Cells[row, 3].Text;
                            var mobileNumber = worksheet.Cells[row, 4].Text;

                            // Validate Email
                            if (!IsValidEmail(email))
                            {
                                invalidUsers.Add($"Row {row}: Invalid email '{email}'.");
                                continue; 
                            }

                            // Validate Mobile Number
                            if (!IsValidMobileNumber(mobileNumber))
                            {
                                invalidUsers.Add($"Row {row}: Invalid mobile number '{mobileNumber}'.");
                                continue; 
                            }

                            var user = new User
                            {
                                Name = name,
                                Email = email,
                                MobileNumber = mobileNumber,
                                Password = "pass" + name,
                                Photo = ""
                            };

                            users.Add(user);
                        }

                        const int batchSize = 10000;
                        for (int i = 0; i < users.Count; i += batchSize)
                        {
                            var batch = users.Skip(i).Take(batchSize).ToList();
                            _context.User.AddRange(batch);
                            await _context.SaveChangesAsync();
                        }

                        if (invalidUsers.Count > 0)
                        {
                            foreach (var error in invalidUsers)
                            {
                                Console.WriteLine(error);
                            }
                        }
                    }
                }
            }
            return RedirectToAction("Index");
        }

        private bool IsValidEmail(string email)
        {
            return !string.IsNullOrEmpty(email) &&
                   Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
        }

        private bool IsValidMobileNumber(string mobileNumber)
        {
            return !string.IsNullOrEmpty(mobileNumber) &&
                   mobileNumber.All(char.IsDigit) &&
                   mobileNumber.Length >= 7 && mobileNumber.Length <= 15; 
        }




    }
}
