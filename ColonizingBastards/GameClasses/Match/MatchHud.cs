using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using ColonizingBastards.Base.Director;
using ColonizingBastards.Base.Graphics;
using ColonizingBastards.Base.Objects;
using ColonizingBastards.Base.PlayerInteraction;
using ColonizingBastards.Base.Scene;
using ColonizingBastards.Base.ScreenUtil;
using ColonizingBastards.GameClasses.Match.Config;
using ColonizingBastards.GameClasses.Match.MatchObjects;
using ColonizingBastards.GameClasses.Match.MatchObjects.Characters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ColonizingBastards.GameClasses.Match
{
	class MatchHud : Hud
	{
	    private List<TextHudElement> playerScoreTextHudElements;
	    private List<SimpleSpriteHudElement> matchUI;
	    private List<AmmoSpriteHudElement> playerAmmoCount;
	    private List<Sprite>[] playerAmmoSprites;
	    private Vector2[] playerScorePos;
	    private Vector2 treasureScorePos;

	    private int playerCount;
		public Vector2 GetPlayerScorePosition(int player, int playerCount)
		{
			int width = MatchConfig.TotalWidth;
			float border = MatchConfig.BorderRatio;
			int x = (int) ((border * width) + (1f / (playerCount + 1)) * (width - (2 * border * width)) * player);
			int y = MatchConfig.TopSpacing;
		    return new Vector2(x, y);
		}

		public MatchHud(ContentManager content, Director director, Scene scene, bool[] selectedPlayers) : base(content, director, scene)
		{
			playerCount = 4;//scene.GetPlayers().Count;
            playerScoreTextHudElements = new List<TextHudElement>(playerCount);
			playerAmmoCount = new List<AmmoSpriteHudElement>();
            playerAmmoSprites = new List<Sprite>[playerCount];
            //Init SpriteHudElements
            matchUI = new List<SimpleSpriteHudElement>();

            //Add Match UI for the selected players
		    Actor actor;
            for (int i = 0; i < selectedPlayers.Length; i++)
		    {
		        if (selectedPlayers[i])
		        {
		            actor = scene.MatchUiPos[i];
		            matchUI.Add(new SimpleSpriteHudElement(this, (Sprite) actor.getRepresentation()));
		        }
		    }

            //Add Match trasure count UI( assumes that Treasure UI is saved last in the tiled map)
		    actor = scene.MatchUiPos[scene.MatchUiPos.Count - 1];
            matchUI.Add(new SimpleSpriteHudElement(this, (Sprite)actor.getRepresentation()));

            //Init TextHudElements
            InitHudUIPositions();

            InitAmmoCountSprites();

			// Add player score to the Hud
			foreach (Player p in scene.GetPlayers())
			{
				Func<object, string> textFunc = (object arg) => scene.Scores[p].getScore().ToString();
				Func<object, Color> colorFunc = (object arg) => Color.Black;
				TextHudElement currentPlayerScoreTextHudElement = new TextHudElement(this, fonts.Last(), textFunc, colorFunc, null, null);
				currentPlayerScoreTextHudElement.SetPosition(new Vector3(playerScorePos[(int) p.playerIndex], 0));
				playerScoreTextHudElements.Add(currentPlayerScoreTextHudElement);

                DefaultCharacter character = p.PossesedCharacters[0] as DefaultCharacter;
			    if (character != null)
			    {
                    Func<Sprite> spriteFunc = () =>
                    {
                        var ammoCount = ((DefaultCharacter) p.PossesedCharacters[0]).AmmoCount;
                        if (ammoCount > 0)
                        {
                            Sprite playerAmmoSprite =
                                playerAmmoSprites[(int) p.playerIndex][(int) ammoCount - 1];
                            Rectangle boundingBox = character.CharacterPhysics.BoundingBox;
                            playerAmmoSprite.SetPosition(new Vector3(boundingBox.Center.X - 10, boundingBox.Top - 10, 0));
                            return playerAmmoSprite;
                        }
                        return null;
                    };
			        AmmoSpriteHudElement currentPlayerAmmoHudElement = new AmmoSpriteHudElement(this, spriteFunc, null);
                    playerAmmoCount.Add(currentPlayerAmmoHudElement);
                    //......
                }
			}

			//Add treasureCount to the Hud
			Func<object, string> treasureTextFunc = (object arg) => scene.TreasuresLeft.ToString();
            Func<object, Color> treasureColorFunc = (object arg) => Color.IndianRed;
            TextHudElement treasureCount = new TextHudElement(this, fonts.Last(), treasureTextFunc, treasureColorFunc, null, null);
            treasureCount.SetPosition(new Vector3(treasureScorePos,0f));
            playerScoreTextHudElements.Add(treasureCount);

            hudElements.AddRange(matchUI);
			hudElements.AddRange(playerScoreTextHudElements);
            hudElements.AddRange(playerAmmoCount);

		}

	    private void InitHudUIPositions()
	    {
	        playerScorePos = new Vector2[4];
	        foreach (Player p in scene.GetPlayers())
	        {
	            var playerIndex = (int) p.playerIndex;

	            var scorePos = scene.MatchUiPos[playerIndex].GetPosition();
	            var scoreSize = scene.MatchUiPos[playerIndex].GetSize();

                playerScorePos[playerIndex] = new Vector2(scorePos.X + scoreSize.X/2f, scorePos.Y - (2 *scoreSize.Y)/3f);
	        }

            var treasureCountPos = scene.MatchUiPos[4].GetPosition();
            var treasureCountSize = scene.MatchUiPos[4].GetSize();

            treasureScorePos = new Vector2(treasureCountPos.X + treasureCountSize.X/2f, treasureCountPos.Y - (2 * treasureCountSize.Y)/3f);

        }

	    private void InitAmmoCountSprites()
	    {
	        for (var i = 0; i < playerCount; i++)
	        {
	            playerAmmoSprites[i] = new List<Sprite>();
                //var playerAmmoPos = new Vector3(playerScorePos[i].X + 85, playerScorePos[i].Y + 45, 0f);
	            for (var j = 1; j < 4; j++)
	            {
                    var sprite = new Sprite(ItemsConfig.AMMO_COUNT_TEXTURES[j]);
                    //sprite.SetPosition(playerAmmoPos);
                    sprite.SetSize(new Vector3(sprite.Texture.Width, sprite.Texture.Height, 0f));
	                playerAmmoSprites[i].Add(sprite);
	            }
	        }
	    }

	}
}
