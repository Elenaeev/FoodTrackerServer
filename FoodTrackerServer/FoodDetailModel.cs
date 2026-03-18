

namespace FoodTrackerServer
{
    public class FoodDetailModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Kcal { get; set; }
        public float Carbs { get; set; }
        public float Protein { get; set; }
        public float Fat { get; set; }
        public string Unit { get; set; } = "g";
        public float InventoryAmount { get; set; } = 0f;
        public string InventoryAmountWithUnit => $"{InventoryAmount} {Unit}";

    }
}
