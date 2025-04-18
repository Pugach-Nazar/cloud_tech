namespace lab6.Models
{
    public class ImageAnalysisResultModel
    {
        public string Description { get; set; }
        public List<string> Categories { get; set; }
        public List<string> Tags { get; set; }
        public List<string> Objects { get; set; } 
        public string ImagePath { get; set; }

        public ImageAnalysisResultModel()
        {
            Categories = new List<string>();
            Tags = new List<string>();
            Objects = new List<string>();
        }
    }

}
