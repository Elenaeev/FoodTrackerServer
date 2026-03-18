

using FoodTrackerServer;

namespace FoodTrackerServer
{
    public class RecipeFoodModel
    {
        public int Id { get; set; }
        public int FoodId { get; set; }
        public int RecipeId { get; set; }
        public FoodDetailModel? Food { get; set; }
        public float Amount { get; set; }

    }
}
