using SQLite;
using UnityEngine;
namespace SITig
{
    public class PlayerInTig : ModKit.ORM.ModEntity<PlayerInTig>
    {
        [AutoIncrement][PrimaryKey] public int Id { get; set; }
        public int playerId { get; set; }
        public int TigRemaining { get; set; }
        public string positionString { get; set; }
        [Ignore]
        public Vector3 AncientPosition { get => Json.Vector3Converters.ConvertFromJson(positionString); set => positionString = Json.Vector3Converters.ConvertToJson(AncientPosition); }
    }
}