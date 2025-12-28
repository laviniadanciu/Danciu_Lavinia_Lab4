
using Microsoft.AspNetCore.Mvc;
using Danciu_Lavinia_Lab4.Data;
using Microsoft.EntityFrameworkCore;   // <- important 

namespace Danciu_Lavinia_Lab4.Controllers
{
    public class PredictionController : Controller
    {
        private readonly AppDbContext _context;

        public PredictionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Price()
        {
            return View(new PricePredictionModel.ModelInput());
        }
        [HttpGet]
        public async Task<IActionResult> History()
        {
            var history = await _context.PredictionHistories
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            return View(history);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Price(PricePredictionModel.ModelInput input)
        {
            if (!ModelState.IsValid)
                return View(input);

            var result = PricePredictionModel.Predict(input);

            ViewBag.Price = result.Score;

            var history = new PredictionHistory
            {
                PassengerCount = input.Passenger_count,
                TripTimeInSecs = input.Trip_time_in_secs,
                TripDistance = input.Trip_distance,
                PaymentType = input.Payment_type,
                PredictedPrice = result.Score,
                CreatedAt = DateTime.Now
            };

            _context.PredictionHistories.Add(history);
            await _context.SaveChangesAsync();

            return View(input);
        }

        public IActionResult Duration(TimePredictionModel.ModelInput input)
        {

            input.Vendor_id = input.Vendor_id ?? string.Empty;
            var result = TimePredictionModel.Predict(input);

            ViewBag.Duration = result.Score;
            return View(input);
        }
    }
}
