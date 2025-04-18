using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using lab6.Models;

public class ImageAnalysisController : Controller
{
    private readonly ComputerVisionClient _computerVisionClient;

    public ImageAnalysisController(ComputerVisionClient computerVisionClient)
    {
        _computerVisionClient = computerVisionClient;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new ImageAnalysisResultModel());
    }

    [HttpPost]
    public async Task<IActionResult> Index(IFormFile file)
    {
        var model = new ImageAnalysisResultModel();
        if (file == null || file.Length == 0) return View(model);

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        var imageBytes = ms.ToArray();

        var uploadsPath = Path.Combine("wwwroot", "uploads");
        Directory.CreateDirectory(uploadsPath);

        var fileName = Path.GetFileName(file.FileName);
        var filePath = Path.Combine(uploadsPath, fileName);
        await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

        model.ImagePath = "/uploads/" + fileName;

        var features = new List<VisualFeatureTypes?>
        {
            VisualFeatureTypes.Description,
            VisualFeatureTypes.Categories,
            VisualFeatureTypes.Tags,
            VisualFeatureTypes.Objects
        };

        var analysis = await _computerVisionClient.AnalyzeImageInStreamAsync(new MemoryStream(imageBytes), features);

        model.Description = (analysis.Description?.Captions?.Any() == true)
            ? string.Join("; ", analysis.Description.Captions.Select(c => $"{c.Text} ({c.Confidence:P})"))
            : "No description available.";

        model.Categories = analysis.Categories?.Select(c => c.Name).ToList() ?? new();
        model.Tags = analysis.Tags?.Select(t => t.Name).ToList() ?? new();
        model.Objects = analysis.Objects?.Select(o => $"{o.ObjectProperty} ({o.Confidence:P})").ToList() ?? new();

        return View(model);
    }
}
