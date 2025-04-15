using SQLite;
using UnityEngine;

namespace SITig
{
    public class PointPatern : ModKit.ORM.ModEntity<PointPatern>
    {
        [AutoIncrement][PrimaryKey] public int Id { get; set; }
        public int secondsForCompleteTig { get; set; } = 15;
        public string panelTitle { get; set; }
        public string panelDescription { get; set; }
        public string positionString { get; set; }
        [Ignore]
        public Vector3 position { get => Json.Vector3Converters.ConvertFromJson(positionString); set => positionString = Json.Vector3Converters.ConvertToJson(position); }
    }
}