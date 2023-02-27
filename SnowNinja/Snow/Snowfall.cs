using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnowNinja.Snow
{
    class Snowfall
    {
        Texture2D texture;
        Vector2 position;
        Vector2 velocity;

        public Vector2 Position
        {
            get { return position; }
        }

        /// <summary>
        /// Snowfall's constructor.
        /// </summary>
        public Snowfall(Texture2D newTexture, Vector2 newPosition, Vector2 newVelocity)
        {
            texture = newTexture;
            position = newPosition;
            velocity = newVelocity;
        }

        /// <summary>
        /// updates the position.
        /// </summary>
        public void Update()
        {
            position += velocity;
        }

        /// <summary>
        /// draws the snowfall.
        /// </summary>
        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, position, Color.White);
        }
    }
}
