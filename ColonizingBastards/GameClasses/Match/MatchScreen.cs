using ColonizingBastards.Base.Cameras;
using ColonizingBastards.Base.Director;
using ColonizingBastards.Base.Objects;
using ColonizingBastards.Base.Scene;
using ColonizingBastards.Base.ScreenUtil;
using ColonizingBastards.GameClasses.Config;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonizingBastards.Base.Graphics;
using ColonizingBastards.Base.MiscUtil.Collidables;
using ColonizingBastards.GameClasses.Match.MatchObjects.Ai;
using ColonizingBastards.GameClasses.Match.MatchObjects.Characters;
using ColonizingBastards.GameClasses.Match.MatchObjects.Vegetation;
using ColonizingBastards.GameClasses.Match.ParticleSystem;

namespace ColonizingBastards.GameClasses.Match
{
    class MatchScreen : Screen
    {
        private Texture2D point;
        private Texture2D platformAlphaMask;
        private DefaultCharacter character;
        public MatchScreen(Director director, Scene scene, Vector2 baseScreenSize, bool[] selectedPlayers)
        {
            this.director = director;
            this.scene = scene;
            this.hud = new MatchHud(director.Content, director, scene, selectedPlayers);
            //this.camera = new BaseCamera(new Vector2(0.0f, 0.0f), 0, 1);
            this.camera = new ScalableCamera(director.Graphics, baseScreenSize, new Vector2(0.0f, 0.0f), 0, 1);
            //debug code
            // create 1x1 texture for line drawing
            point = new Texture2D(director.Graphics.GraphicsDevice, 1, 1);
            point.SetData(
                new[] {Color.White}); // fill the texture with white

            platformAlphaMask =
                director.Content.Load<Texture2D>(MainConfig.PIPELINE_GRAPHICS_DIRECTORY + "FinalPlatforms");

            character = scene.GetPlayers()[0].PossesedCharacters[0] as DefaultCharacter;
        }

        public override void Draw(SpriteBatch batch)
        {
            batch.Begin(transformMatrix: camera.GetViewMatrix());

            foreach (Actor a in scene.GetActors())
            {
                a.Draw(batch);
            }


            batch.End();

            DrawFoliageWithAlphaMask(batch);

            foreach (ParticleEffectManager particleEffect in scene.ParticleEffects)
            {
                particleEffect.Draw(batch, camera);
            }

            scene.ParticleEffects.RemoveAll(pEffect => (pEffect.EffectDuration <= 0 && !pEffect.Continuous));

            scene.LightningManager.Draw(batch);

            batch.Begin(transformMatrix: camera.GetViewMatrix());

			//Debug code, TODO: REMOVE LATER
			SpriteFont debugFont = hud.fonts[1];
            /*Draw outlines for each of the collidables
            foreach (Collidable polygon in scene.GetCollidables())
            {
               DrawPolygon(batch, polygon, Color.Red, false);
            }

            //character Boundingbox
            Collidable boundingBox = Polygon.RectangleToPolygon(character.CharacterPhysics.BoundingBox);
            DrawPolygon(batch, boundingBox, Color.Green, false);*/
            //Draw  MinX and MaxX for each walkable polygon
            /*foreach (Collidable polygon in scene.WalkableCollidables)
            {
                DrawLine(batch, new Vector2(polygon.WalkableMinMax.X, polygon.MaxHeight), new Vector2(polygon.WalkableMinMax.Y, polygon.MinHeight), Color.Red);
            }

            //Draw the same numbers for climbable collidables that are connected to each other (= ClimbGroup)
            int counter = 0;

            foreach (ClimbGroup climbGroup in scene.NavigationGraph.CurrClimbGroup)
            {
                foreach (Collidable c in climbGroup.ClimbCollidables)
                    batch.DrawString(hud.defaultFont, counter.ToString(), new Vector2(c.Center.X, c.CenterYClimbGroup), Color.White);

                counter++;
            }

            counter = 0;
            //Draw the same numbers for platforms that are connected to each other via climbable polygon
            foreach (NavGraphNode nav1 in scene.NavigationGraph.NavGraphNodes)
            {
                if (nav1.ReachableNodes.Count == 0) continue;



                foreach (var nav2 in nav1.ReachableNodes)
                {
                    batch.DrawString(hud.defaultFont, counter.ToString(), nav2.Item2, Color.White);
                    batch.DrawString(hud.defaultFont, counter.ToString(), nav2.Item3, Color.White);
                }

                counter++;
            }

            foreach (NavGraphNode node in scene.NavigationGraph.NavGraphNodes)
            {
                DrawPolygon(batch, node.Platform, Color.White, true);
            }*/
#if DEBUG
            // Draw the character's query point for the platforms
			for (int i = 0; i < scene.Characters.Count; i++)
            {
                DefaultCharacter c = (DefaultCharacter)scene.Characters[i];

                if (c?.CharacterPhysics == null)
                    continue;

                /*float tempX, tempY, tempWidth, tempHeight;
                tempX = (float)Math.Round(c.CharacterPhysics.GetPosition().X + c.CharacterPhysics.hitboxOffset.X - 1, MidpointRounding.AwayFromZero);
                tempY = (float)Math.Round(c.CharacterPhysics.GetPosition().Y + c.CharacterPhysics.hitboxOffset.Y - 1, MidpointRounding.AwayFromZero);
                tempWidth = c.CharacterPhysics.BoundingBox.Width;
                tempHeight = c.CharacterPhysics.BoundingBox.Height;

                // Point below character
                Vector2 pos = new Vector2(tempX + 0.5f * tempWidth, tempY);
                Vector2 p = new Vector2(pos.X, tempY + 1.5f * tempHeight);

                //batch.DrawString(hud.defaultFont, "o", p, Color.White);
                DrawLine(batch, pos, p, Color.AntiqueWhite);*/

            	Collidable col = c.CharacterPhysics.QueryCurrentPlatform();
                DrawPolygon(batch, col, Color.Blue, false, "p" + i);
            }

            // Debug Rendering of Navigational-Graph
            for (int i = 0; i < scene.NavigationGraph.NavGraphNodes.Length; i++)
            {
                NavGraphNode n = scene.NavigationGraph.NavGraphNodes[i];

                DrawLine(batch, new Vector2(n.Platform.WalkableMinMax.X, n.Platform.MinHeight), new Vector2(n.Platform.WalkableMinMax.Y, n.Platform.MinHeight), Color.Yellow);

                batch.DrawString(debugFont, "n_"+i, n.Platform.Center + new Vector2(0, 10), Color.Orange);
                foreach (Tuple<NavGraphNode, Vector2, Vector2> t in n.ReachableNodes)
                {
                    DrawLine(batch, t.Item2, t.Item3, Color.Orange);
                    batch.DrawString(debugFont, "rn_"+i, t.Item2 + new Vector2(0,0), Color.OrangeRed);
                }
            }

            // Debug Rendering of the Path of the Ai-Characters
            List<AiController> lc = scene.GetAiControllers();
            for (int i = 0; i < lc.Count; i++)
            {
                AiIndigenController c = (AiIndigenController)lc[i];
                if (c == null || c.Path.Count == 0)
                    continue;

                Vector3 pos = c.PossesedCharacters[0].GetCenterPosition();
                DrawLine(batch, pos, c.Path[0].Position, Color.White);
                batch.DrawString(debugFont, "w"+i,new Vector2(pos.X, pos.Y), Color.White);
                for (int j = 0; j < c.Path.Count-1; j++)
                {
                    DrawLine(batch, c.Path[j].Position, c.Path[j+1].Position, Color.White);
                    batch.DrawString(debugFont, (j+1).ToString(), new Vector2(c.Path[j].Position.X, c.Path[j].Position.Y), Color.White);
                }

            }

#endif
            hud.DrawHud(batch);

            if(scene.MatchPaused)
                scene.MatchPauseScreen.Draw(batch);

            batch.End();
        }

        //Debug code
        private void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end, Color color)
        {
            Vector2 edge = end - start;
            // calculate angle to rotate line
            float angle =
                (float) Math.Atan2(edge.Y, edge.X);


            sb.Draw(point,
                new Rectangle( // rectangle defines shape of line and position of start of line
                    (int) start.X,
                    (int) start.Y,
                    (int) edge.Length(), //sb will strech the texture to fill this rectangle
                    1), //width of line, change this to make thicker line
                null,
                color, //colour of line
                angle, //angle of line (calulated above)
                new Vector2(0, 0), // point in line about which to rotate
                SpriteEffects.None,
                0);

        }

	    private void DrawLine(SpriteBatch sb, Vector3 start, Vector3 end, Color color)
	    {
		    DrawLine(sb, new Vector2(start.X, start.Y), new Vector2(end.X, end.Y), color);
	    }

        private void DrawPolygon(SpriteBatch sb, Collidable collidable, Color color, bool names, String centerLabel = "")
        {
            if (collidable == null)
                return;

            int i = 0;
            foreach (Vector2 v in collidable.Points)
            {
                if (names)
                    sb.DrawString(hud.defaultFont, "v_" + i, v, color);

                DrawLine(sb, v, v + collidable.Edges[i], color);
                if (names)
                    sb.DrawString(hud.defaultFont, "e_" + i, v + 0.5f * collidable.Edges[i], color);
                i++;
            }

            if (centerLabel != null && !centerLabel.Equals(""))
                sb.DrawString(hud.defaultFont, centerLabel, collidable.Center, color);
        }

        private void DrawFoliageWithAlphaMask(SpriteBatch batch)
        {
            //director.Graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;

            var m = Matrix.CreateOrthographicOffCenter(0,
                director.Graphics.GraphicsDevice.PresentationParameters.BackBufferWidth,
                director.Graphics.GraphicsDevice.PresentationParameters.BackBufferHeight,
                0, 0, 1
            );

            var a = new AlphaTestEffect(director.Graphics.GraphicsDevice)
            {
                Projection = camera.GetViewMatrix() * m
            };

            //Create a mask to the backbuffer
            var s1 = new DepthStencilState
            {
                StencilEnable = true,
                StencilFunction = CompareFunction.Always,
                StencilPass = StencilOperation.Replace,
                ReferenceStencil = 1,
                DepthBufferEnable = false,
            };

            //Draw only if the alpha value is greater or equal to 0 (foliages on the white background)
            var s2 = new DepthStencilState
            {
                StencilEnable = true,
                StencilFunction = CompareFunction.GreaterEqual,
                StencilPass = StencilOperation.Keep,
                ReferenceStencil = 0,
                DepthBufferEnable = false,
            };

            batch.Begin(SpriteSortMode.Immediate, null, null, s1, null, a);
            Rectangle destinationRectangle = new Rectangle(0, 0, 1920, 1080);
            batch.Draw(platformAlphaMask, destinationRectangle, Color.White); //The mask                                   
            batch.End();

            batch.Begin(transformMatrix:camera.GetViewMatrix());
            foreach(Actor actor in scene.Characters)
                actor.Draw(batch);
            batch.End();

            batch.Begin(SpriteSortMode.Immediate, null, null, s2, null, a);
            foreach(Foliage actor in scene.GetFoliage())
                actor.Draw(batch);
            batch.End();

            director.Graphics.PreferredDepthStencilFormat = DepthFormat.Depth24;
        }
    }
}
