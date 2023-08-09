using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace MapPropertyExtensions
{
    internal class ModData
    {
        public List<OverlayProperty> OverlayProperties { get; set; }
        public ModData(List<OverlayProperty> properties)
        {
            OverlayProperties = properties;
        }
    }
    public class OverlayProperty
    {
        /// <summary>
        /// path of image. e.g: 'assets/Image'. if animated, will cycle through Images via number at end.
        /// </summary>
        public string ImagePath { get; set; } = "assets/Image";
        internal List<Texture2D> ImageTextures = new List<Texture2D>();
        public string PropertyName { get; set; } = "";
        public OverlayAnimation Animation { get; set; }

        public OverlayProperty(string propertyName, string imagePath, OverlayAnimation animation)
        {
            PropertyName = propertyName;
            ImagePath = imagePath;
            Animation = animation;
        }
    }
    public class OverlayAnimation
    {
        public int NumberOfFrames { get; set; } = 1;
        /// <summary>
        /// in ticks
        /// </summary>
        public int FrameDuration { get; set; } = 60;
        public OverlayAnimation(int numFrames, int frameDur)
        {
            NumberOfFrames = numFrames;
            FrameDuration = frameDur;
        }
    }
}
