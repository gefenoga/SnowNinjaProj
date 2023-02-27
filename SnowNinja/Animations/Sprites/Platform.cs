using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnowNinja.Animations.Sprites
{
    class Platform : Sprite
    {

        /// <summary>
        /// Platform's constructor.
        /// </summary>
        public Platform(Texture2D newTexture) : base(newTexture)
        {
        }

        /// <summary>
        /// overrides Sprite's Draw function
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, rec, Color.White);
        }
    }   
}
