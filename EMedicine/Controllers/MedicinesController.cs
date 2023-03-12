using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EMedicine.Models;

namespace EMedicine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicinesController : ControllerBase
    {
        private readonly EMedicineContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public MedicinesController(EMedicineContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            this._hostingEnvironment = hostingEnvironment;
        }

        // GET: api/Medicines
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Medicines>>> GetMedicines()
        {
          if (_context.Medicines == null)
          {
              return NotFound();
          }
            return await _context.Medicines
                .Select(x => new Medicines()
                {
                    Id= x.Id,
                    Name= x.Name,
                    Discount= x.Discount,
                    ExpDate= x.ExpDate,
                    ImageUrl= x.ImageUrl,    
                    Manufacturer= x.Manufacturer,
                    Status= x.Status,
                    UnitPrice = x.UnitPrice,
                    ImageSrc = String.Format("{0}://{1}{2}/Images/{3}", Request.Scheme, Request.Host, Request.PathBase, x.ImageUrl),
                })
                .ToListAsync();
        }

        // GET: api/Medicines/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Medicines>> GetMedicines(int id)
        {
          if (_context.Medicines == null)
          {
              return NotFound();
          }
            var medicines = await _context.Medicines.FindAsync(id);
            medicines.ImageSrc = String.Format("{0}://{1}{2}/Images/{3}", Request.Scheme, Request.Host, Request.PathBase, medicines.ImageUrl);

            if (medicines == null)
            {
                return NotFound();
            }

            return medicines;
        }

        // PUT: api/Medicines/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMedicines(int id, [FromForm] Medicines medicines)
        {
            if (id != medicines.Id)
            {
                return BadRequest();
            }

            if(medicines.ImageFile != null)
            {
                var existingMedicine = await _context.Medicines.FindAsync(id);
                DeleteImage(existingMedicine.ImageUrl);
                _context.Entry(existingMedicine).State = EntityState.Detached;
                medicines.ImageUrl = await SaveImage(medicines.ImageFile);
            }

            _context.Entry(medicines).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MedicinesExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Medicines
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Medicines>> PostMedicines([FromForm]Medicines medicines)
        {
          if (_context.Medicines == null)
          {
              return Problem("Entity set 'EMedicineContext.Medicines'  is null.");
          }
            medicines.ImageUrl = await SaveImage(medicines.ImageFile);
            _context.Medicines.Add(medicines);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMedicines", new { id = medicines.Id }, medicines);
        }


        // DELETE: api/Medicines/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicines(int id)
        {
            if (_context.Medicines == null)
            {
                return NotFound();
            }
            var medicines = await _context.Medicines.FindAsync(id);
            if (medicines == null)
            {
                return NotFound();
            }
            DeleteImage(medicines.ImageUrl);
            _context.Medicines.Remove(medicines);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MedicinesExists(int id)
        {
            return (_context.Medicines?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [NonAction]
        public async Task<string> SaveImage(IFormFile imageFile)
        {
            string imageName = new string(Path.GetFileNameWithoutExtension(imageFile.FileName).Take(10).ToArray()).Replace(' ', '-');
            imageName = imageName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(imageFile.FileName);
            var imagePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Images", imageName);
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }
            return imageName;
        }

        [NonAction]
        public void DeleteImage(string imageName)
        {

            var imagePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Images", imageName);
            if(System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
        }
    }
}
