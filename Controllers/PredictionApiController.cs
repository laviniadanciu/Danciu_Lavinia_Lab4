using Danciu_Lavinia_Lab4.Data;
using Danciu_Lavinia_Lab4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Danciu_Lavinia_Lab4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PredictionApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PredictionApiController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/PredictionApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PredictionHistory>>> GetPredictions()
        {
            var predictions = await _context.PredictionHistories.ToListAsync();
            return Ok(predictions);
        }

        // DELETE: api/PredictionApi/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePrediction(int id)
        {
            var prediction = await _context.PredictionHistories.FindAsync(id);

            if (prediction == null)
            {
                return NotFound();
            }

            _context.PredictionHistories.Remove(prediction);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
