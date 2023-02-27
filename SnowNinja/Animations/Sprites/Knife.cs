using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnowNinja.Animations.Sprites
{
    public class Knife : Sprite
    {
        public bool IsRemoved = false; // controls whether to delete the knife or not in PostUpdate in Game1. 

        public string knifeDir; // determines which direction the knife is being thrown to.

        /// <summary>
        /// inherits the constructor from its parent, Sprite.
        /// </summary>

        public Knife(Texture2D texture) : base(texture)
        {

        }

        /// <summary>
        /// creates the flipping motion of the knife while updating its position.
        /// </summary>

        public void UpdateKnife()
        {
            if (knifeDir == "right")
            {
                rotation -= 0.3f;
                _position.X += 10f;
            }
            if (knifeDir == "left")
            {
                rotation += 0.3f;
                _position.X -= 10f;
            }
        }

        protected override void Move()
        {
        }
    }
}
