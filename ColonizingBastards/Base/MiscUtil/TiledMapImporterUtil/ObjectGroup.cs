using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ColonizingBastards.Base.Graphics;
using ColonizingBastards.Base.MiscUtil.Collidables;
using ColonizingBastards.Base.Objects;
using ColonizingBastards.GameClasses.Match.MatchObjects.MatchShopKeeper;
using ColonizingBastards.GameClasses.Match.MatchObjects.Vegetation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ColonizingBastards.Base.MiscUtil.TiledMapImporterUtil
{
    /// <summary>
    /// This class represents a layer(objectgroup) in the gamemap.
    /// Each layer can have multiple objects. Each Object represents a static gameobject(platform/ collisionshapes).
    /// </summary>
    class ObjectGroup
    {
        public string Name { get; }

        private Dictionary<int, Texture2D> GidToImage;

        private Scene.Scene scene;

        public ObjectGroup(XElement objectGroupXml, Scene.Scene scene, Dictionary<int, Texture2D> gidToImage, Vector2 tileSize)
        {
            this.scene = scene;
            this.Name = (string)objectGroupXml.Attribute("name");
            bool isClimbable = false;
            bool isWalkable = false;

            IEnumerable<XElement> tiledObjects = objectGroupXml.Elements("object");

            var xElement = objectGroupXml.Element("properties");
            if (xElement != null)
            {
                IEnumerable<XElement> properties = xElement.Elements("property");

                foreach (XElement prop in properties)
                {
                    if (((string) prop.Attribute("name")).Equals("climbable") && (bool) prop.Attribute("value"))
                        isClimbable = true;
                    else if (((string) prop.Attribute("name")).Equals("collision") && (bool) prop.Attribute("value"))
                        isClimbable = false;

               
                }
            }

            this.GidToImage = gidToImage;

            foreach (XElement tiledObject in tiledObjects)
            {
                //TODO: handle more object cases, if necessary
                if (tiledObject.Attribute("gid") != null)
                {
                    Actor actor = XElementToActor(tiledObject);
                    if (Name.Equals("PlayerReady"))
                        scene.CharacterMenuReadyPos.Add(actor);      
                    else if (Name.Equals("PlayerJoin"))
                    {
                        scene.CharacterMenuJoinPos.Add(actor);
                        scene.RegisterObject(actor);
                    }
                    else if (Name.Equals("MatchUI"))
                    {
                        scene.MatchUiPos.Add(actor);
                    }
                    else if (Name.Equals("PlayerWin"))
                    {
                        scene.CharacterWonPos.Add(actor);
                    }
                    else if (Name.Equals("PlayerLose"))
                    {
                        scene.CharacterLostPos.Add(actor);
                    }
                    else if (Name.Equals("PlayerNotPlayed"))
                    {
                        scene.CharacterNotPlayedPos.Add(actor);
                    }
                    else if (Name.Equals("Pause"))
                    {
                        scene.MatchPauseScreen = actor;
                    }
                    else
                        scene.RegisterObject(actor);
                                           
                }
                else if(tiledObject.Attribute("width") != null)
                {
                    //Handle Rectangle object
                    if (Name.Equals("PlayerSpawn"))
                    {
                        Rectangle playerInitPos = new Rectangle((int)tiledObject.Attribute("x"), (int)tiledObject.Attribute("y"),
                                                    (int)tiledObject.Attribute("width"), (int)tiledObject.Attribute("height"));

                        scene.CharacterStartPos.Add(playerInitPos);
                    }
                    else if (Name.Equals("ShopkeeperSpawn"))
                    {
                        Rectangle shopkeeperInitPos = new Rectangle((int)tiledObject.Attribute("x"), (int)tiledObject.Attribute("y"),
                                                    (int)tiledObject.Attribute("width"), (int)tiledObject.Attribute("height"));

                        scene.ShopKeeperStartPos.Add(shopkeeperInitPos);

                    }else if (Name.Equals("FinalScorePos"))
                    {
                        Rectangle finalScorePos = new Rectangle((int)tiledObject.Attribute("x"), (int)tiledObject.Attribute("y"),
                                                    (int)tiledObject.Attribute("width"), (int)tiledObject.Attribute("height"));

                        scene.FinalScorePos.Add(finalScorePos);
                    }
                    
                   
                }
                else if (tiledObject.Element("polygon") != null)
                {
                    //Handle Polygon Object
                    XElement polygon = tiledObject.Element("polygon");
                    List<Vector2> cornerPoints = new List<Vector2>();
                    Vector2 basePosition = new Vector2((float)tiledObject.Attribute("x"), (float)tiledObject.Attribute("y"));

                    //Read in corner points(relative to the base position)
                    string[] pointsString = ((string) polygon.Attribute("points")).Split(',', ' ');
                    float[] points = pointsString.Select(float.Parse).ToArray();

                    for (var i = 0; i < points.Length/2; i++)
                    {
                        Vector2 cornerPoint = new Vector2(basePosition.X + points[2*i], basePosition.Y + points[2*i + 1]);
                        //Vector2 cornerPoint = new Vector2((float)Math.Round(basePosition.X + points[2 * i] + 1, MidpointRounding.AwayFromZero), (float)Math.Round(basePosition.Y + points[2 * i + 1], MidpointRounding.AwayFromZero));
                        cornerPoints.Add(cornerPoint);
                    }
                    
                    var objectProperties = tiledObject.Element("properties");
                    if (objectProperties != null)
                    {
                        IEnumerable<XElement> properties = objectProperties.Elements("property");

                        foreach (XElement prop in properties)
                        {
                            if (((string)prop.Attribute("name")).Equals("walkable"))
                                isWalkable = (bool)prop.Attribute("value");
                        }
                    }
                    Polygon newPolygon = new Polygon(cornerPoints, isClimbable, isWalkable);
                    scene.RegisterCollidable(newPolygon);
                }

            } 
        }

        /// <summary>
        /// Method creates an Actor from Xml Data 
        /// </summary>
        /// <param name="tiledObject">Xml element that contains main information for an Actor</param>
        /// <returns></returns>
        private Actor XElementToActor(XElement tiledObject)
        {         
            int gid = (int)tiledObject.Attribute("gid");
            Vector3 position = new Vector3((int)tiledObject.Attribute("x"), (int)tiledObject.Attribute("y"), 0);
            Vector3 size = new Vector3((int)tiledObject.Attribute("width"), (int)tiledObject.Attribute("height"), 0);

            Texture2D image = GidToImage[gid];

	        Actor actor;

            if (Name.Equals("Undergrowth"))
                actor = new Foliage(new Sprite(image), scene, position);
            else
                actor = new Actor(new Sprite(image));
           
            actor.SetPosition(position);
            actor.SetRotation(Vector3.Zero);
            actor.SetSize(size);

            return actor;
        }

    }
}
