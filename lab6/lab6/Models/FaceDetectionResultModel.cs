using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System.Collections.Generic;

namespace lab6.Models
{
    public class FaceDetectionResultModel
    {
        public string Message { get; set; }

        public List<FaceInfo> Faces { get; set; } = new List<FaceInfo>();
    }

    public class FaceInfo
    {
        public string Glasses { get; set; }
        public double Pitch { get; set; }
        public double Roll { get; set; }
        public double Yaw { get; set; }
        public List<string> Accessories { get; set; } = new List<string>();
        public string BlurLevel { get; set; }
        public double BlurValue { get; set; }
        public string ExposureLevel { get; set; }
        public double ExposureValue { get; set; }
        public string NoiseLevel { get; set; }
        public double NoiseValue { get; set; }
        public string FaceImageUrl { get; set; }
    }
}
