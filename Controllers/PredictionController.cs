
using Danciu_Lavinia_Lab4.Data;
using Microsoft.AspNetCore.Mvc;
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
        /*[HttpGet]
        public async Task<IActionResult> History()
        {
            var history = await _context.PredictionHistories
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            return View(history);
        }*/
        [HttpGet]
        public async Task<IActionResult> History(string? paymentType,float? minPrice,float? maxPrice, string? sortOrder, DateTime? startDate,
    DateTime? endDate)
        {

            var query = _context.PredictionHistories.AsQueryable();
           

            if (!string.IsNullOrEmpty(paymentType))
            {
                query = query.Where(p => p.PaymentType == paymentType);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.PredictedPrice >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.PredictedPrice <= maxPrice.Value);


            }

            if (startDate.HasValue)
                query = query.Where(p => p.CreatedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(p => p.CreatedAt <= endDate.Value);
        
            query = sortOrder switch
            {
                ////////////////////////SORTARE DUPA PRET
                "price_asc" => query.OrderBy(p => p.PredictedPrice),
                "price_desc" => query.OrderByDescending(p => p.PredictedPrice),
                ////////////////////////SORTARE DUPA data
                "date_asc" => query.OrderBy(p => p.CreatedAt),
                "date_desc" => query.OrderByDescending(p => p.CreatedAt),

                _ => query.OrderByDescending(p => p.CreatedAt) // default: cele mai noi primele
            };



            /////////////////SORTARE DUPA DATA
            ///filtrare dupa interval de date (CreatedAt)


            //sortare dupa data
            ViewBag.CurrentPaymentType = paymentType;
            ViewBag.CurrentMinPrice = minPrice;
            ViewBag.CurrentMaxPrice = maxPrice;
            ViewBag.CurrentSortOrder = sortOrder;
            /////FORMAT DATA
            ViewBag.CurrentStartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.CurrentEndDate = endDate?.ToString("yyyy-MM-dd");

            var result = await query.ToListAsync();
            return View(result);
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
