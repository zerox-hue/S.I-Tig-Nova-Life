using SQLite;
using UnityEngine;

namespace SITig
{
    public class PointPatern
    {
        [AutoIncrement][PrimaryKey] public int Id { get; set; }
        public int secondsForCompleteTig { get; set; } = 15;
        public string panelTitle { get; set; }
        public string panelDescription { get; set; }
        private string positionString { get; set; }
        [Ignore]
        public Vector3 position { get => Kit.Vector3Converter.ConvertFromJson(positionString); set => positionString = Kit.Vector3Converter.ConvertToJson(position); }
    }
}