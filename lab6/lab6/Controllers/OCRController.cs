using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using lab6.Models;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

public class OCRController : Controller
{
    private readonly ComputerVisionClient _computerVisionClient;

    public OCRController(ComputerVisionClient computerVisionClient)
    {
        _computerVisionClient = computerVisionClient;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new OCRResultModel());
    }

    [HttpPost]
    public async Task<IActionResult> Index(IFormFile file)
    {
        var model = new OCRResultModel();

        if (file == null || !file.ContentType.StartsWith("image/"))
        {
            ModelState.AddModelError("", "File must be an image.");
            return View(model);
        }

        var uploadPath = Path.Combine("wwwroot", "uploads");
        Directory.CreateDirectory(uploadPath);
        var filePath = Path.Combine(uploadPath, file.FileName);

        await using (var fs = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fs);
        }

        model.ImagePath = "/uploads/" + file.FileName;

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        ms.Seek(0, SeekOrigin.Begin);

        var ocrResult = await _computerVisionClient.ReadInStreamAsync(ms);
        var operationId = ocrResult.OperationLocation.Split('/').Last();
        var result = await _computerVisionClient.GetReadResultAsync(Guid.Parse(operationId));

        while (result.Status == OperationStatusCodes.Running)
        {
            result = await _computerVisionClient.GetReadResultAsync(Guid.Parse(operationId));
        }

        var textList = new List<string>();

        foreach (var page in result.AnalyzeResult.ReadResults)
        {
            foreach (var line in page.Lines)
            {
                textList.Add(line.Text);
            }
        }

        model.FullText = string.Join(" ", textList);

        return View(model);
    }
}
