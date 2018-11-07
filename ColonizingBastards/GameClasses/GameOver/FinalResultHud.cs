using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonizingBastards.Base.Director;
using ColonizingBastards.Base.Graphics;
using ColonizingBastards.Base.Objects;
using ColonizingBastards.Base.PlayerInteraction;
using ColonizingBastards.Base.Scene;
using ColonizingBastards.Base.ScreenUtil;
using ColonizingBastards.GameClasses.Match.MatchObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace ColonizingBastards.GameClasses.GameOver
{
    class FinalResultHud:Hud
    {
        private List<TextHudElement> playerScoreTextHudElements;
        private int playerCount;

        public FinalResultHud(ContentManager content, Director director, Scene scene, bool[] selectedPlayers)
            : base(content, director, scene)
        {
            playerCount = selectedPlayers.Where(s => s).Count();
            playerScoreTextHudElements = new List<TextHudElement>(playerCount);

            Vector2 scale = new Vector2(1.5f, 1.5f);
            // Add player score to the Hud
            foreach (KeyValuePair<Player, Score> s in scene.Scores)
            {
                Func<object, string> textFunc = arg => s.Value.getScore().ToString();
                Func<object, Color> colorFunc = arg => Color.White;
                TextHudElement currentPlayerScoreTextHudElement = new TextHudElement(this, fonts.Last(), textFunc,
                    colorFunc, null, null, scale:scale);

                Vector3 playerScorePos = new Vector3(scene.FinalScorePos[(int) s.Key.playerIndex].Center.ToVector2(), 0f);
                Vector3 size = scene.CharacterWonPos[(int) s.Key.playerIndex].GetCenterPosition();
                currentPlayerScoreTextHudElement.SetPosition(new Vector3(playerScorePos.X - 10, playerScorePos.Y - 20, 0));
                playerScoreTextHudElements.Add(currentPlayerScoreTextHudElement);
            }

            hudElements.AddRange(playerScoreTextHudElements);
        }
    }
}
