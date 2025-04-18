using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using lab6.Models;

public class FaceController : Controller
{
    private readonly FaceClient _faceClient;

    public FaceController(FaceClient faceClient)
    {
        _faceClient = faceClient;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new FaceDetectionResultModel());
    }

    [HttpPost]
    public async Task<IActionResult> Index(IFormFile file)
    {
        var model = new FaceDetectionResultModel ();
        if (file == null || file.Length == 0)
        {
            model.Message = "File not loaded";
            return View(model);
        }

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        var imageBytes = ms.ToArray();

        var faceAttributes = new List<FaceAttributeType>
        {
            FaceAttributeType.Glasses,
            FaceAttributeType.HeadPose,
            FaceAttributeType.Accessories,
            FaceAttributeType.Blur,
            FaceAttributeType.Exposure,
            FaceAttributeType.Noise
        };

        var faces = await _faceClient.Face.DetectWithStreamAsync(
            image: new MemoryStream(imageBytes),
            returnFaceId: false,
            returnFaceLandmarks: false,
            returnFaceAttributes: faceAttributes,
            recognitionModel: "recognition_04",
            detectionModel: "detection_01"
        );

        if (faces == null || !faces.Any())
        {
            model.Message = "Face not found";
            return View(model);
        }

        using var image = Image.Load(imageBytes);
        int faceIndex = 1;

        foreach (var face in faces)
        {
            var rect = face.FaceRectangle;
            var cropped = image.Clone(ctx => ctx.Crop(new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height)));
            var faceFileName = $"face_{faceIndex}.png";
            var facePath = Path.Combine("wwwroot/images/faces", faceFileName);
            Directory.CreateDirectory(Path.GetDirectoryName(facePath));
            cropped.Save(facePath, new PngEncoder());

            model.Faces.Add(new FaceInfo
            {
                Glasses = face.FaceAttributes.Glasses.ToString(),
                Pitch = face.FaceAttributes.HeadPose.Pitch,
                Roll = face.FaceAttributes.HeadPose.Roll,
                Yaw = face.FaceAttributes.HeadPose.Yaw,
                Accessories = face.FaceAttributes.Accessories?.Select(a => a.Type.ToString()).ToList() ?? new(),
                BlurLevel = face.FaceAttributes.Blur.BlurLevel.ToString(),
                BlurValue = face.FaceAttributes.Blur.Value,
                ExposureLevel = face.FaceAttributes.Exposure.ExposureLevel.ToString(),
                ExposureValue = face.FaceAttributes.Exposure.Value,
                NoiseLevel = face.FaceAttributes.Noise.NoiseLevel.ToString(),
                NoiseValue = face.FaceAttributes.Noise.Value,
                FaceImageUrl = $"/images/faces/{faceFileName}"
            });

            faceIndex++;
        }
        return View(model);
    }
}
