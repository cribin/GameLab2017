using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ColonizingBastards.Base.Graphics;
using ColonizingBastards.Base.Objects;
using ColonizingBastards.GameClasses.Config;
using ColonizingBastards.GameClasses.Match.MatchObjects.Characters;
using ColonizingBastards.GameClasses.Match.MatchObjects.MatchShopKeeper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ColonizingBastards.Base.MiscUtil
{
    class SpritesheetImporter
    {
        //Returns the spritesheets with the corresponding hitbox offsets
        public static Tuple<Spritesheet, Rectangle> ImportSpritesheet(string spritesheetPath, ContentManager content, Scene.Scene scene)
        {
            XDocument spritesheetDoc = XDocument.Load(spritesheetPath);
            
            ////LOAD IN GRAPHICS VARIABLES////

            XElement graphics = spritesheetDoc.Element("graphics");
            XElement spriteSheet = graphics.Element("spritesheet");
            string spriteSheetSrc = (string)spriteSheet.Attribute("src");
            int numOfRows = (int)spriteSheet.Attribute("num_rows");
            int numOfCols = (int)spriteSheet.Attribute("num_cols");
            int spriteWidth = (int)spriteSheet.Attribute("width");
            int spriteHeight = (int)spriteSheet.Attribute("height");

            Texture2D texture = content.Load<Texture2D>(MainConfig.PIPELINE_GRAPHICS_DIRECTORY + spriteSheetSrc);
            Spritesheet spritesheet = new Spritesheet(texture, numOfRows, numOfCols, spriteWidth, spriteHeight, 20);

            //Load in animations
            foreach (XElement animation in spriteSheet.Elements("animation"))
            {
                string animName = (string)animation.Attribute("name");
                int msperframe = (int)animation.Attribute("msperframe");
                int[] msperframes = { msperframe };
                bool loopable = (bool)animation.Attribute("loopable");
                int[] frameSeq;
                Animation newAnim;

                //check if animation has a frame_seq element tag, if not, the frame sequence is continious
                if (animation.HasElements)
                {
                    XElement frameSeqXml = animation.Element("frame_seq");
                    List<Tuple<int, int[], int[]>> layers = new List<Tuple<int, int[], int[]>>();
                    int layerNum = 0;

                    foreach (XElement sprites in frameSeqXml.Elements("sprites"))
                    {
                        string[] frameSeqString = ((string)sprites.Attribute("ids")).Split(',');
                        layerNum = (int)sprites.Attribute("layer");
                        //LINQ expressions(turns string array into int array)
                        frameSeq = frameSeqString.Select(int.Parse).ToArray();
                        layers.Add(new Tuple<int, int[], int[]>(layerNum, frameSeq, msperframes));
                    }

                    //Initilaize Animation depending on the number of layers
                    newAnim = layers.Count > 1 ? new Animation(animName, layers, loopable) : new Animation(animName, layers[0].Item1, layers[0].Item2, layers[0].Item3[0], loopable);

                }
                else
                {
                    string[] frameSeqString = ((string)animation.Attribute("frame_seq")).Split(',');
                    frameSeq = frameSeqString.Select(int.Parse).ToArray();
                    frameSeq = Enumerable.Range(frameSeq[0], (frameSeq[1] - frameSeq[0]) + 1).ToArray();

                    newAnim = new Animation(animName, 1, frameSeq, msperframe, loopable);

                }

                spritesheet.AddAnimation(newAnim);
            }

            //Load in hitbox(hitbox is defined from the upper left corner vs. tiled image defined from the lower left corner)
            XElement hitboxOffset = graphics.Element("hitbox");
            Rectangle hitbox = new Rectangle((int)hitboxOffset.Element("x"), (int)hitboxOffset.Element("y"), (int)hitboxOffset.Element("width"), (int)hitboxOffset.Element("height"));

            return new Tuple<Spritesheet, Rectangle>(spritesheet, hitbox);
        }
    }
}
