using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnowNinja.Animations.Sprites
{
    public class HealthBar : Sprite
    {
        public Texture2D texture;// The bar's texture

        public Vector2 position;// The bar's position on the screen

        public Rectangle rectangle;// The bar's rectangle.

        /// <summary>
        /// The constructor of the HealthBar class
        /// </summary>

        public HealthBar(Texture2D newTex, Vector2 newPos, Rectangle newRec) : base(newTex)
        {
            texture = newTex;
            position = newPos;
            rectangle = newRec;
        }

        /// <summary>
        /// reducing the width of rectangle by 20 pixels. 
        /// </summary>

        public void hit()
        {
            rectangle.Width -= 20;
        }

        /// <summary>
        /// draws the health bar on the screen.
        /// </summary>

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, position, rectangle, Color.White, 0, Vector2.Zero, 0.4f, SpriteEffects.None, 1);
        }
    }
}
