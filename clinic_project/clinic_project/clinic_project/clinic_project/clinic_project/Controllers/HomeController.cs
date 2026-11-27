using AspNetCoreGeneratedDocument;
using clinic_project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using NuGet.Protocol.Plugins;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Security.Claims;

namespace clinic_project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        ClinicDbContext db = new ClinicDbContext();

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //add madicine 

        public IActionResult addmadi()
        {
            return View();
        }

        [HttpPost]

        public IActionResult addmadi(Medicine M, IFormFile imgfile)

        {

            var imgname = Path.GetFileName(imgfile.FileName);

            string imgPath = Path.Combine(HttpContext.Request.PathBase.Value, "wwwroot/upload/");

            string imgvalue = Path.Combine(imgPath, imgname);

            using (var stream = new FileStream(imgvalue, FileMode.Create))
            {
                imgfile.CopyTo(stream);
            }


            var dbimg = Path.Combine("/upload/", imgname);
            M.Image = dbimg;

            db.Medicines.Add(M);
            db.SaveChanges();

            return RedirectToAction("fetchmadi");
        }

        //fetch madi
        public IActionResult fetchmadi()
        {

            return View(db.Medicines.ToList());

        }

        //delete  Madi 
       public IActionResult deletmedi(int id)
        {
            var deleteu = db.Medicines.FirstOrDefault(m => m.MedicineId == id);
            if (deleteu != null)
            {
                db.Medicines.Remove(deleteu);
                db.SaveChanges();
            }


            return RedirectToAction("fetchmadi");

        }

        //Edit madi

        public IActionResult editpro(int id)
        {
            var dataitem = db.Medicines.FirstOrDefault(p => p.MedicineId == id);

            return View(dataitem);
        }


        [HttpPost]
        public IActionResult editpro(IFormFile file, Medicine m, int id)
        {


            var existingProduct = db.Medicines.Find(id);
            if (existingProduct == null)
            {
                return NotFound();
            }
            else
            {
                if (file != null)
                {
                    var imageName = Path.GetFileName(file.FileName);
                    string imagePath = Path.Combine(HttpContext.Request.PathBase.Value, "wwwroot/upload/");

                    string imagevalue = Path.Combine(imagePath, imageName);
                    using (var stream = new FileStream(imagevalue, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    existingProduct.Image = "/upload/" + imageName;
                }
                existingProduct.MedicineId = m.MedicineId;
                existingProduct.Name = m.Name;
                existingProduct.Type = m.Type;
                existingProduct.Code = m.Code;
                existingProduct.Price = m.Price;
                existingProduct.Stock = m.Stock;



                db.Medicines.Update(existingProduct);
                db.SaveChanges();


                return RedirectToAction("fetchmadi");

            }
        }

        public IActionResult adduser()
        {
            ViewBag.roledata = new SelectList(db.Roles.ToList(), "RoleId", "RoleName");
            {
                return View();
            }

        }


        [HttpPost]

        public IActionResult adduser(User u)
        {
            ViewBag.roledata = new SelectList(db.Roles.ToList(), "RoleId", "RoleName");
            db.Users.Add(u);
            db.SaveChanges();
            {
                return RedirectToAction("showuser");
            }

        }

        public IActionResult showuser()
        {
            return View(db.Users.Include(ua => ua.Role).ToList());
        }

        public IActionResult Feedback()
        {
            var feedbacks = db.Feedbacks
               .Include(f => f.User)
               .OrderByDescending(f => f.CreatedAt)
               .ToList();

            return View(feedbacks);

        }
        public IActionResult AddRole()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddRole(Role roles)
        {
            db.Roles.Add(roles);
            db.SaveChanges();
            return RedirectToAction("Rolelist");
        }


        public IActionResult Rolelist()
        {
            return View(db.Roles.ToList());
        }

        public IActionResult deleterole(int id)
        {
            var dataitem = db.Roles.FirstOrDefault(r => r.RoleId == id);
            if (dataitem != null)
            {
                db.Roles.Remove(dataitem);
                db.SaveChanges();
            }

            return RedirectToAction("Rolelist");
        }

        public IActionResult editrole(int id)
        {
            var dataitem = db.Roles.FirstOrDefault(r => r.RoleId == id);
            return View(dataitem);
        }

        [HttpPost]
        public IActionResult editrole(int id, Role rol)
        {
            var dataitem = db.Roles.Find(id);
            if (dataitem == null)
            {
                return NotFound();
            }
            dataitem.RoleName = rol.RoleName;
            db.Roles.Update(dataitem);
            db.SaveChanges();
            return RedirectToAction("Rolelist");
        }

        //Subject ADD

        public IActionResult Addsubject()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Addsubject(Subject s)
        {
            db.Subjects.Add(s);
            db.SaveChanges();
            return View();
        }

        public IActionResult fetchsubject()
        {
            return View(db.Subjects.ToList());
        }

        public IActionResult deletsubject(int id)
        {
            var dataitem = db.Subjects.FirstOrDefault(s => s.SubjectId == id);

            if (dataitem != null)
            {
                db.Subjects.Remove(dataitem);
                db.SaveChanges();
            }

            return RedirectToAction("fetchsubject");
        }

        public IActionResult editsubject(int id)
        {
            var dataitem = db.Subjects.FirstOrDefault(s => s.SubjectId == id);
            return View(dataitem);
        }


        [HttpPost]
        public IActionResult editsubject(int id, Subject s )
        {
            var dataitem = db.Subjects.Find(id);
            if (dataitem == null)
            {
                return NotFound();
            }
            dataitem.SubjectName = s.SubjectName;
            dataitem.Description = s.Description;

            db.Subjects.Update(dataitem);
            db.SaveChanges();
            return RedirectToAction("fetchsubject");
        }

        //equipments 

        public IActionResult AddEquipment()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddEquipment(Equipment equipment, IFormFile? ImageFile)
        {
            string imageName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);

            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/upload/");

            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }

            string imageValue = Path.Combine(imagePath, imageName);

            using (var stream = new FileStream(imageValue, FileMode.Create))
            {
                ImageFile.CopyTo(stream);
            }

            equipment.Image = "/upload/" + imageName;

            db.Equipment.Add(equipment);
            db.SaveChanges();
            return RedirectToAction("EquipmentList");
        }

        //fetch equipment
        public IActionResult EquipmentList()
        {
            return View(db.Equipment.ToList());
        }

        //delete  equipment
        public IActionResult DeleteEquipment(int id)
        {
            var item = db.Equipment.FirstOrDefault(e => e.EquipmentId == id);
            if (item != null)
            {
                db.Equipment.Remove(item);
                db.SaveChanges();
            }
            return RedirectToAction("EquipmentList");
        }

        //edit  equipment
        public IActionResult EditEquipment(int id)
        {
            var item = db.Equipment.FirstOrDefault(e => e.EquipmentId == id);
            return View(item);
        }
        [HttpPost]
        public IActionResult EditEquipment(IFormFile file, Equipment eq, int id)
        {
            var existingProduct = db.Equipment.Find(id);

            if (existingProduct == null)
            {
                return NotFound();
            }
            else
            {
                // agar new image upload ho
                if (file != null)
                {
                    var imageName = Path.GetFileName(file.FileName);

                    string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/upload/");

                    // folder create agar na ho
                    if (!Directory.Exists(imagePath))
                    {
                        Directory.CreateDirectory(imagePath);
                    }

                    string imageValue = Path.Combine(imagePath, imageName);

                    using (var stream = new FileStream(imageValue, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    existingProduct.Image = "/upload/" + imageName;
                }

           
                existingProduct.Name = eq.Name;
                existingProduct.Description = eq.Description;
                existingProduct.Price = eq.Price;
                existingProduct.Stock = eq.Stock;

                db.Equipment.Update(existingProduct);
                db.SaveChanges();

                return RedirectToAction("EquipmentList");
            }
        }

        // ADD
        public IActionResult AddSeminar()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddSeminar(Seminar s)
        {
            db.Seminars.Add(s);
            db.SaveChanges();

            return RedirectToAction("Seminar");
        }

        //FETCH SEMINAR 
        public IActionResult Seminar()
        {
            var data = db.Seminars.ToList();
            return View(data);
        }

        // DELETE
        public IActionResult DeleteSeminar(int id)
        {
            var row = db.Seminars.Find(id);

            db.Seminars.Remove(row);
            db.SaveChanges();

            return RedirectToAction("Seminar");
        }

        // EDIT
        public IActionResult EditSeminar(int id)
        {
            var row = db.Seminars.Find(id);
            return View(row);
        }
        [HttpPost]
        public IActionResult EditSeminar(Seminar s)
        {
            db.Seminars.Update(s);
            db.SaveChanges();

            return RedirectToAction("Seminar");
        }

        public IActionResult AddStaff()
        {
            ViewBag.Roles = db.Roles.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddStaff(Staff staff, string Password)
        {
            if (ModelState.IsValid)
            {
                db.Staff.Add(staff);
                db.SaveChanges();

                User user = new User()
                {
                    Name = staff.FullName,
                    Email = staff.Email,
                    Phone = staff.Phone,
                    RoleId = staff.RoleId,
                    PasswordHash = Password,
                    CreatedAt = DateTime.Now
                };

                db.Users.Add(user);
                db.SaveChanges();

                return RedirectToAction("staff");
            }

            ViewBag.Roles = db.Roles.ToList();
            return RedirectToAction("staff");
        }

        public IActionResult Staff()
        {
            var staffs = db.Staff.Include(s => s.Role).ToList();
            return View(staffs);
        }

        public IActionResult DeleteStaff(int id)
        {
            var staff = db.Staff.Find(id);

            if (staff != null)
            {
                db.Staff.Remove(staff);
                db.SaveChanges();
            }

            return RedirectToAction("Staff");
        }

        public IActionResult EditStaff(int? id)
        {
            if (id == null) return NotFound();

            var staff = db.Staff.Find(id);
            if (staff == null) return NotFound();

            ViewBag.Roles = db.Roles.ToList();
            return View(staff);
        }

        // POST: Staff/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditStaff(int id, Staff staff)
        {
            if (id != staff.StaffId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(staff);
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!db.Staff.Any(e => e.StaffId == id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction("Staff");
            }
            ViewBag.Roles = db.Roles.ToList();
            return View(staff);
        }


        public IActionResult PracticalList()
        {
            // =========================
            // PRACTICAL LIST
            // =========================
            var data = db.Practicals
                .Include(p => p.Subject)
                .Include(p => p.Staff)
                .ToList();

            return View(data);
        }


        // =========================
        // ADD PRACTICAL (GET)
        // =========================
        [HttpGet]
        public IActionResult AddPractical()
        {
            ViewBag.SubjectId = new SelectList(db.Subjects, "SubjectId", "SubjectName");
            ViewBag.StaffId = new SelectList(db.Staff, "StaffId", "StaffName");

            return View();
        }

        // =========================
        // ADD PRACTICAL (POST)
        // =========================
        [HttpPost]
        public IActionResult AddPractical(Practical row)
        {
            db.Practicals.Add(row);
            db.SaveChanges();

            return RedirectToAction("PracticalList");
        }

        // =========================
        // EDIT PRACTICAL (GET)
        // =========================
        [HttpGet]
        public IActionResult EditPractical(int id)
        {
            var row = db.Practicals.Find(id);

            ViewBag.SubjectId = new SelectList(db.Subjects, "SubjectId", "SubjectName", row.SubjectId);
            ViewBag.StaffId = new SelectList(db.Staff, "StaffId", "StaffName", row.StaffId);

            return View(row);
        }

        // =========================
        // EDIT PRACTICAL (POST)
        // =========================
        [HttpPost]
        public IActionResult EditPractical(Practical row)
        {
            db.Practicals.Update(row);
            db.SaveChanges();

            return RedirectToAction("PracticalList");
        }

        // =========================
        // DELETE PRACTICAL
        // =========================
        public IActionResult DeletePractical(int id)
        {
            var row = db.Practicals.Find(id);

            db.Practicals.Remove(row);
            db.SaveChanges();

            return RedirectToAction("PracticalList");
        }

        // =========================
        // LECTURE LIST
        // =========================
        public IActionResult LectureList()
        {
            var data = db.Lectures
                .Include(x => x.Subject)
                .Include(x => x.Staff)
                .ToList();

            return View(data);
        }

        // =========================
        // ADD LECTURE (GET)
        // =========================
        [HttpGet]
        public IActionResult AddLecture()
        {
            ViewBag.SubjectId = new SelectList(db.Subjects, "SubjectId", "SubjectName");
            ViewBag.StaffId = new SelectList(db.Staff, "StaffId", "StaffName");

            return View();
        }

        // =========================
        // ADD LECTURE (POST)
        // =========================
        [HttpPost]
        public IActionResult AddLecture(Lecture row)
        {
            db.Lectures.Add(row);
            db.SaveChanges();

            return RedirectToAction("LectureList");
        }

        // =========================
        // EDIT LECTURE (GET)
        // =========================
        [HttpGet]
        public IActionResult EditLecture(int id)
        {
            var row = db.Lectures.Find(id);

            ViewBag.SubjectId = new SelectList(db.Subjects, "SubjectId", "SubjectName", row.SubjectId);
            ViewBag.StaffId = new SelectList(db.Staff, "StaffId", "StaffName", row.StaffId);

            return View(row);
        }

        // =========================
        // EDIT LECTURE (POST)
        // =========================
        [HttpPost]
        public IActionResult EditLecture(Lecture row)
        {
            db.Lectures.Update(row);
            db.SaveChanges();

            return RedirectToAction("LectureList");
        }

        // =========================
        // DELETE LECTURE
        // =========================
        public IActionResult DeleteLecture(int id)
        {
            var row = db.Lectures.Find(id);

            db.Lectures.Remove(row);
            db.SaveChanges();

            return RedirectToAction("LectureList");
        }
        public IActionResult Orders()
        {
        

            var adminOrders = db.Orders
                      .OrderByDescending(o => o.OrderDate)
                      .ToList();

            return View(adminOrders);
        }

    }
}