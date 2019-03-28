using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            // Get celestial object
            var result = _context.CelestialObjects.SingleOrDefault(co => co.Id == id);

            // Return if not found
            if (result == null)
            {
                return NotFound();
            }

            // Manually set the satellites for the returned object
            // TODO: Refactor
            result.Satellites = _context.CelestialObjects
                .Where(s => s.OrbitedObjectId == id).ToList();

            return Ok(result);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var results = _context.CelestialObjects.Where(co => co.Name.ToLower() == name.ToLower()).ToList();

            if (!results.Any())
                return NotFound();

            // Manually set the satellites property for each returned object
            // TODO: Refactor
            foreach (var result in results)
            {
                result.Satellites = _context.CelestialObjects.Where(s => s.OrbitedObjectId == result.Id).ToList();
            }

            return Ok(results);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var results = _context.CelestialObjects.ToList();

            if (!results.Any())
                return NotFound();

            // Manually set the satellites property for each returned object
            // TODO: Refactor
            foreach (var result in results)
            {
                result.Satellites = _context.CelestialObjects.Where(s => s.OrbitedObjectId == result.Id).ToList();
            }

            return Ok(results);
        }

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            // TODO: Object sanitization

            var result = _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();

            // TODO: Review use of celestialObject vs result.Entity
            return CreatedAtRoute(nameof(GetById), new { id = result.Entity.Id }, result.Entity);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {

            // Option A
            // if (id != celestialObject.Id)
            // {
            //     return BadRequest();
            // }
            // _context.Entry(co).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            // _context.SaveChanges();

            // Option B - required by Iris
            var existingObject = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);

            if (existingObject == null)
            {
                return NotFound();
            }

            existingObject.Name = celestialObject.Name;
            existingObject.OrbitalPeriod = celestialObject.OrbitalPeriod;
            existingObject.OrbitedObjectId = celestialObject.OrbitedObjectId;

            _context.CelestialObjects.Update(existingObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var existingObject = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);

            if (existingObject == null)
            {
                return NotFound();
            }

            if (name.Length < 1)
            {
                return BadRequest();
            }

            existingObject.Name = name;
            _context.CelestialObjects.Update(existingObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {

            throw new NotImplementedException();
        }
    }
}
