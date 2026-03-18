

namespace FoodTrackerServer
{
    public class RecipeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public List<RecipeFoodModel>? Foods { get; set; } 

    }
}
