namespace BloodyBoss.DB.Models
{
    internal class ItemEncounterModel
    {
        public string name { get; set; }
        public int ItemID { get; set; }
        public int Stack { get; set; }
        public int Chance { get; set; } = 1;
        public string Color { get; set; } = "#daa520";
    }
}
