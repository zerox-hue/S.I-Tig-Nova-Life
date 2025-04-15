using Life;
using Life.DB;
using Life.Network;
using Mirror;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SITig
{
    public class OnPlayerSpawn : Plugin
    {
        public OnPlayerSpawn(IGameAPI gameAPI) : base(gameAPI)
        {
        }
        public override async void OnPlayerSpawnCharacter(Player player, NetworkConnection conn, Characters character)
        {
            base.OnPlayerSpawnCharacter(player, conn, character);
            await Task.Delay(500);
            CheckPlayerInTig(player);
        }
        private async void CheckPlayerInTig(Player player)
        {
            var elements = await PlayerInTig.Query(x => x.playerId == player.character.Id);
            if (!elements.Any())
            {
                return;
            }
            foreach (var panels in Nova.server.panels.ToList())
            {
                player.ClosePanel(panels);
            }
            PlayerInTig playerInTig = elements.FirstOrDefault();
            List<PointPatern> allPoint = await PointPatern.QueryAll();
            player.setup.TargetSetPosition(Json.Vector3Converters.ConvertFromJson(SITig.config.BaseTigPosition));
            if(playerInTig.TigRemaining >= 0)
            {
                await playerInTig.Delete();
                return;
            }
            SITig.TigPlayer(player, allPoint, playerInTig,-1);
        }
    }
}