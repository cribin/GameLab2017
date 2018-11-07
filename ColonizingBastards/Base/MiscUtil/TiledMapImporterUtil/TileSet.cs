using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ColonizingBastards.Base.MiscUtil.TiledMapImporterUtil
{
    //IMPORTANT:Assumes Collection of images is used and not tiles!!  
    /// <summary>
    /// A TileSet represents a collection of images.
    /// Each image in the tileset is referenced by an object in the objectgroup using the gid(global id of an image)
    /// This class mainly stores the mapping from the Gid to the corresponding texture
    /// </summary>
    public class TileSet
    {
        public int Firstgid { get; }
        private string name;      

        public TileSet(XElement tileSetXml, ContentManager content, string directory, Dictionary<int, Texture2D> gidToImage )
        {
            Firstgid = (int) tileSetXml.Attribute("firstgid");
            name = (string) tileSetXml.Attribute("name");

            foreach (var tile in tileSetXml.Elements("tile"))
            {
                //compute global id of the image
                var imageGid = Firstgid + (int) tile.Attribute("id");
                string[] imagePath = ((string) tile.Element("image").Attribute("source")).Split('.','/');
                var imageSource = "";
                if (imagePath.Length >= 2)
                    imageSource = directory + imagePath[imagePath.Length - 2];
                            
                var image = content.Load<Texture2D>(imageSource);
                gidToImage.Add(imageGid, image);
            }
        }
    }
}
