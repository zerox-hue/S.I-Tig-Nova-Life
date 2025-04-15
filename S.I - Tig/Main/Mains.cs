using Life;
using Life.CheckpointSystem;
using Life.DB;
using Life.Network;
using Life.UI;
using Life.VehicleSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace SITig
{
    public class SITig : Plugin
    {
        public SITig(IGameAPI gameAPI) : base(gameAPI) { }
        public static List<Player> playerOnPanelTig = new List<Player>();
        public static Config config;
        public Vector3 basePositionTig = Kit.Vector3Converter.ConvertFromJson(config.BaseTigPosition);
        public override void OnPluginInit()
        {
            base.OnPluginInit();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("[HueTig V1.1.0] initalized success powered by Zerox_Hue");
            Console.ForegroundColor = ConsoleColor.White;
            new SChatCommand("/tig", "Travaux au joueurs", "/tig", (player, args) =>
            {
                if (player.IsAdmin)
                {
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
                /*var elements = await PlayerInTig.Query(x => x.playerId == target.character.Id);*/
                /*if (!elements.Any())
                {
                    player.Notify("TIG", "Le joueur est déjà en TIG.", NotificationManager.Type.Error);
                    return;
                }
                */
                UIPanel panel = new UIPanel("Nombre de tig", UIPanel.PanelType.Input);
                panel.SetInputPlaceholder("Entrez le nombre de tig...");
                panel.AddButton("Fermer", ui => player.ClosePanel(ui));
                panel.AddButton("Valider", ui => { if (!int.TryParse(ui.inputText, out int value)) return; StartTigPlayer(player, target, value); player.ClosePanel(ui); });
                player.ShowPanelUI(panel);
            }
        }
        private /*async*/ void StartTigPlayer(Player player, Player target, int tigCount)
        {
            /*List<PointPatern> elements = await PointPatern.QueryAll();*/
            /*if (!elements.Any())
            {
                player.Notify("TIG", "Aucun point de tig trouvé.", NotificationManager.Type.Error);
                return;
            }*/
            PlayerInTig playerInTig = new PlayerInTig()
            {
                playerId = target.character.Id,
                TigRemaining = tigCount,
                AncientPosition = target.setup.transform.position,
            };
            /*await playerInTig.Save();*/
            target.setup.TargetSetPosition(basePositionTig);
            target.SendText(config.TextOnStartTig);
            player.Notify("TIG", $"Vous avez envoyé {target.FullName} au tig.", NotificationManager.Type.Success);
            /*TigPlayer(target, elements, playerInTig);*/
        }
        public static /*async*/ void TigPlayer(Player ply, List<PointPatern> elements, PlayerInTig playerInTig)
        {
            while (true)
            {
                if (!playerOnPanelTig.Contains(ply))
                {
                    if (playerInTig.TigRemaining != 0)
                    {
                        System.Random random = new System.Random();
                        int count = elements.Count;
                        int rdm = random.Next(0, count++);
                        PointPatern element = elements[rdm--];
                        ply.setup.TargetSetGPSTarget(element.position);
                        NCheckpoint point = new NCheckpoint(ply.netId, element.position, _ =>
                        {
                            UIPanel panel = new UIPanel(element.panelTitle, UIPanel.PanelType.Text);
                            panel.SetText(element.panelDescription);
                            panel.AddButton("Fermer", ui => ply.ClosePanel(ui));
                            panel.AddButton("Valider", async ui => { ply.ClosePanel(ui); await ValidTig(ply, element.secondsForCompleteTig); playerInTig.TigRemaining--; /*await playerInTig.Save();*/ });
                            ply.ShowPanelUI(panel);
                        });
                        ply.CreateCheckpoint(point);
                    }
                    else
                    {
                        ply.setup.TargetSetPosition(playerInTig.AncientPosition);
                        playerOnPanelTig.Remove(ply);
                        /*await playerInTig.Delete();*/
                        break;
                    }
                }
            }
        }
        public static async Task ValidTig(Player ply, int seconds)
        {
            playerOnPanelTig.Remove(ply);
            ply.setup.TargetShowCenterText("TIG", "TIG en cours...", seconds);
            await Task.Delay(TimeSpan.FromSeconds(seconds));
            return;
        }
    }
}

