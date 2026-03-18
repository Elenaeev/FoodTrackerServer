

namespace FoodTrackerServer
{
    public class GenerateRecipeRequest
    {
        public List<FoodDetailModel> Ingredients { get; set; }
        public Macros TargetMacros { get; set; }
        public string Type { get; set; }
    }
}
