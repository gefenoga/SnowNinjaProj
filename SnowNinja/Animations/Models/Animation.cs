using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnowNinja.Animations.Models
{
    public class Animation
    {
        public int CurrentFrame { get; set; }// a field of the index of the current frame in the frames order. 

        public int FrameCount { get; set; }// a field of the number of frames the current animation have.

        public int FrameHeight { get { return Texture.Height; } }// a field that contains the height of the frame (identical in every animation) 

        public int FrameWidth { get { return Texture.Width / FrameCount; } }// a field that contains thw frame width 
                                                                            //(single frame width = entire picture width/number of frames)

        public float FrameSpeed { get; set; }// how fast the frames are going to be drown.

        public bool IsLooping { get; set; }// determines whether the animation will loop or not. 

        public Texture2D Texture { get; private set; }// the texture that contains all of the frames in the animation.


        /// <summary>
        /// The constructor of the Animation class. 
        /// </summary>

        public Animation(Texture2D texture, int frameCount)
        {
            Texture = texture;

            FrameCount = frameCount;

            IsLooping = true; //sets the default animation as looping. 

            FrameSpeed = 0.1f; //sets a custom FrameSpeed of 0.1. 
        }
    }
}
