using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;

namespace Danciu_Lavinia_Lab4.Controllers
{
    public class PredictionController : Controller
    {
        public IActionResult Price(PricePredictionModel.ModelInput input)
        {
            MLContext mlContext = new MLContext();

            ITransformer mlModel =
                mlContext.Model.Load(@"PricePredictionModel.mlnet", out var modelInputSchema);

            var predEngine = mlContext.Model.CreatePredictionEngine<
                Danciu_Lavinia_Lab4.PricePredictionModel.ModelInput,
                Danciu_Lavinia_Lab4.PricePredictionModel.ModelOutput>(mlModel);

            var result = predEngine.Predict(input);

            ViewBag.Price = result.Score;
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
