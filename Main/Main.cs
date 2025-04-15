using Life;
using Life.CheckpointSystem;
using Life.DB;
using Life.Network;
using Life.UI;
using Life.VehicleSystem;
using ModKit.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine;

namespace SITig
{
    public class SITig : ModKit.ModKit
    {
        public SITig(IGameAPI gameAPI) : base(gameAPI) { }
        public static List<Player> playerOnPanelTig = new List<Player>();
        public static System.Random randoms = new System.Random();
        public static Config config;
        public override void OnPluginInit()
        {
            base.OnPluginInit();
            config = Config.CreateConfig(pluginsPath);
            Orm.RegisterTable<PointPatern>();
            Orm.RegisterTable<PlayerInTig>();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("[S.I - Tig V.1.1.0] initalized success powered by Zerox_Hue");
            Console.ForegroundColor = ConsoleColor.White;
            SChatCommand command = new SChatCommand("/createtigpoint", "create tig point", "/CTP", PointCreator.OnSlashCreatePoint);
            command.Register();
            new SChatCommand("/tig", "Travaux au joueurs", "/tig", (player, args) =>
            {
                if (player.IsAdmin)
                {
                    if (args.Length < 1)
                    {
                        player.Notify("TIG", "Syntaxe : /tig <ID du joueur>", NotificationManager.Type.Warning);
                        return;
                    }
                    if (!int.TryParse(args[0], out int targetPlayerId))
                    {
                        player.Notify("TIG", "Erreur de syntaxe, utilisez : /tig <id du joueur>", NotificationManager.Type.Error);
                        return;
                    }
                    OnSlashTig(player, targetPlayerId);
                }
            }).Register();
        }
        public async void OnSlashTig(Player player, int targetPlayerId)
        {
            var target = Nova.server.GetPlayer(targetPlayerId);
            if (target == null)
            {
                Characters characters = await LifeDB.FetchCharacter(targetPlayerId);
                if (characters == null)
                {
                    player.Notify("TIG", "Joueur Non existant.", NotificationManager.Type.Error);
                    return;
                }
                player.Notify("TIG", "Joueur Déconnecté.", NotificationManager.Type.Error);
            }
            else
            {
                var elements = await PlayerInTig.Query(x => x.playerId == target.character.Id);
                if (elements.Any())
                {
                    player.Notify("TIG", "Le joueur est déjà en TIG.", NotificationManager.Type.Error);
                    return;
                }

                UIPanel panel = new UIPanel("Nombre de tig", UIPanel.PanelType.Input);
                panel.SetInputPlaceholder("Entrez le nombre de tig...");
                panel.AddButton("Fermer", ui => player.ClosePanel(ui));
                panel.AddButton("Valider", ui => { if (!int.TryParse(ui.inputText, out int value) && value > 0) return; StartTigPlayer(player, target, value); player.ClosePanel(ui); });
                player.ShowPanelUI(panel);
            }
        }
        private async void StartTigPlayer(Player player, Player target, int tigCount)
        {
            List<PointPatern> elements = await PointPatern.QueryAll();
            if (!elements.Any())
            {
                player.Notify("TIG", "Aucun point de tig trouvé.", NotificationManager.Type.Error);
                return;
            }
            PlayerInTig playerInTig = new PlayerInTig()
            {
                playerId = target.character.Id,
                TigRemaining = tigCount,
                AncientPosition = target.setup.transform.position,
                positionString = Json.Vector3Converters.ConvertToJson(target.setup.transform.position),
            };
            await playerInTig.Save();
            Vector3 basePositionTig = Json.Vector3Converters.ConvertFromJson(config.BaseTigPosition);
            target.setup.TargetSetPosition(basePositionTig);
            target.SendText(config.TextOnStartTig);
            player.Notify("TIG", $"Vous avez envoyé {target.FullName} au tig.", NotificationManager.Type.Success);
            TigPlayer(target, elements, playerInTig,-1);
        }
        public static async void TigPlayer(Player ply, List<PointPatern> elements, PlayerInTig playerInTig,int nbr)
        {
            if(playerInTig.TigRemaining > 0) 
            {
                if (!playerOnPanelTig.Contains(ply))
                {
                    playerOnPanelTig.Add(ply);
                    int random = await Random(nbr,elements.Count);
                    PointPatern point = elements[random];
                    try
                    {
                        ply.setup.TargetSetGPSTarget(point.position);

                        NCheckpoint pointTig = new NCheckpoint(ply.netId, point.position, async _ =>
                        {
                            await DeletePoint(ply, point.position);
                            UIPanel panel = new UIPanel(point.panelTitle, UIPanel.PanelType.Text);
                            panel.SetText(point.panelDescription);
                            panel.AddButton("Accepter", async ui =>
                            {
                                ply.ClosePanel(ui);
                                ply.IsFreezed = true;
                                playerInTig.TigRemaining--;
                                await playerInTig.Save();
                                ply.setup.TargetDisableNavigation();
                                await ValidTig(ply, point.secondsForCompleteTig, panel);
                                ply.IsFreezed = false;
                                TigPlayer(ply, elements, playerInTig,random);
                            });
                            ply.ShowPanelUI(panel);
                        });
                        if (playerInTig.TigRemaining > 0)
                        {
                            ply.CreateCheckpoint(pointTig);
                        }
                    }
                    catch (Exception)
                    {
                        playerOnPanelTig.Remove(ply);
                        return;
                    }
                }
            }
            else
            { 
                ply.setup.TargetSetPosition(playerInTig.AncientPosition);
                await playerInTig.Delete();
                return;
            }
        }

        public static async Task ValidTig(Player ply, int seconds,UIPanel panel)
        {
            ply.setup.TargetShowCenterText("TIG", "TIG en cours...", seconds);
            await Task.Delay(TimeSpan.FromSeconds(seconds));
            playerOnPanelTig.Remove(ply);
            return;
        }
        public static Task DeletePoint(Player ply,Vector3 pos)
        {
            foreach (var points in Nova.server.checkpoints.ToList())
            {
                if (points.position == pos)
                    ply.DestroyCheckpoint(points);
            }
            return Task.CompletedTask;
        }
        public static async Task<int> Random(int nbr,int count)
        {
            int random = randoms.Next(0,count);
            if(random == nbr)
            {
                while(true)
                {
                    await Task.Delay(1);
                    int random2 = randoms.Next(0, count);
                    if(random2 != nbr)
                    {
                        return random2;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            else
            {
                return random;
            }
        }
    }
}

