using Life;
using Life.UI;
using Life.Network;
using System.Threading.Tasks;
using System;
using UnityEngine;

namespace SITig
{
    public class PointCreator : Plugin
    {
        public PointCreator(IGameAPI gameAPI) : base(gameAPI)
        {
        }
        public static void OnSlashCreatePoint(Player player, string[] args)
        {
            if (!player.IsAdmin)
            {
                player.Notify("POINT CREATOR", "Tu n'es pas administateur", NotificationManager.Type.Error);
                return;
            }
            UIPanel panel = new UIPanel("Créateur de point", UIPanel.PanelType.Input);
            panel.SetInputPlaceholder("Entrez Titre du panel/Texte du panel/Secondes");
            panel.SetText("Entrez les informations suivantes :");
            panel.AddButton("Fermer", ui => player.ClosePanel(ui));
            panel.AddButton("Valider", ui => { CreateNewPoint(player, ui.inputText); player.ClosePanel(ui); });
            player.ShowPanelUI(panel);
        }
        public static async void CreateNewPoint(Player player, string text)
        {
            string[] args = text.Split('/');
            if (args.Length != 3)
            {
                player.Notify("POINT CREATOR", "Erreur de syntaxe, utilisez : Titre du panel/Texte du panel/Secondes", NotificationManager.Type.Error);
                return;
            }
            string panelTitle = args[0];
            string panelDescription = args[1];
            if (!int.TryParse(args[2], out int secondsForCompleteTig))
            {
                player.Notify("POINT CREATOR", "Erreur de syntaxe, utilisez : Nom du point/Titre du panel/Texte du panel/Secondes", NotificationManager.Type.Error);
                return;
            }
            if (player.setup == null || player == null)
            {
                player.Notify("POINT CREATOR", "Erreur nulle", NotificationManager.Type.Error);
                return;
            }
            PointPatern tig = new PointPatern()
            {
                position = player.setup.transform.position,
                secondsForCompleteTig = secondsForCompleteTig,
                panelTitle = panelTitle,
                panelDescription = panelDescription,
                positionString = Json.Vector3Converters.ConvertToJson(player.setup.transform.position),
            };
            try
            {
                bool flag = await tig.Save();
                if (!flag)
                {
                    player.Notify("POINT CREATOR", "ERREUR DE SAUVEGARDE", NotificationManager.Type.Error);
                    return;
                }
                player.Notify("POINT CREATOR", "Point créé avec succès", NotificationManager.Type.Success);
            }
            catch (Exception ex)
            {
                player.Notify("POINT CREATOR", "Erreur lors de la création du point : " + ex.Message, NotificationManager.Type.Error);
                Debug.LogError("Erreur lors de la création du point : " + ex.Message);
                return;
            }
        }
    }
}
