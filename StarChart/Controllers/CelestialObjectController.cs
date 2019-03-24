using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;

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
    }
}
