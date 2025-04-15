using SQLite;
using UnityEngine;
namespace SITig
{
    public class PlayerInTig
    {
        [AutoIncrement][PrimaryKey] public int Id { get; set; }
        public int playerId { get; set; }
        public int TigRemaining { get; set; }
        private string positionString { get; set; }
        [Ignore]
        public Vector3 AncientPosition { get => Kit.Vector3Converter.ConvertFromJson(positionString); set => positionString = Kit.Vector3Converter.ConvertToJson(AncientPosition); }
    }
}