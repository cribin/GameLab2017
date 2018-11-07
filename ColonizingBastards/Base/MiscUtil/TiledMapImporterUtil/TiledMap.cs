using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ColonizingBastards.Base.MiscUtil.TiledMapImporterUtil
{
    class TiledMap
    {
       
        private Scene.Scene scene;
        private List<TileSet> tileSets;
        private List<ObjectGroup> objectGroups;

        public Vector2 MapSize { get;}
        public Vector2 TileSize { get; }

        public Dictionary<int, Texture2D> GidToImage; 

        public TiledMap(string mapPath, ContentManager content, string tilesDirectory, Scene.Scene currentScene)
        {
            //Change extension of the file from tmx(tiled xml format) to xml
            /*string mapPathXml;
            bool hasTmxExtension = Path.GetExtension(mapPath).Equals(".tmx");
            if (hasTmxExtension)
            {
                mapPathXml = Path.ChangeExtension(mapPath, ".xml");
                File.Move(mapPath, mapPathXml);

            }else if (Path.GetExtension(mapPath).Equals(".xml"))
                mapPathXml = mapPath;
            else
            {
                //Unrecognized file extension => error?
                mapPathXml = null;
            }*/

            XDocument tiledMapXml = XDocument.Load(mapPath);
            XElement mapElement = tiledMapXml.Element("map");
            TileSize = new Vector2((int) mapElement.Attribute("tilewidth"), (int) mapElement.Attribute("tileheight"));
            MapSize = new Vector2((int)mapElement.Attribute("width") * TileSize.X, (int)mapElement.Attribute("height") * TileSize.Y);

            GidToImage = new Dictionary<int, Texture2D>();
            //Init TileSets
            tileSets = new List<TileSet>();
            IEnumerable<XElement> tileSetsXml = mapElement.Elements("tileset");

            foreach (XElement tileSet in tileSetsXml)
                tileSets.Add(new TileSet(tileSet, content, tilesDirectory, GidToImage));

            //Init ObjectGroups and add actors to the scene
            this.scene = currentScene;
            objectGroups = new List<ObjectGroup>();
            IEnumerable<XElement> objectGroupsXml = mapElement.Elements("objectgroup");

            foreach (XElement objectGroup in objectGroupsXml)
                objectGroups.Add(new ObjectGroup(objectGroup, scene, GidToImage, TileSize));

        }
       

    }
}
