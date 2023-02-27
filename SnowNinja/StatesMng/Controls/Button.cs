using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnowNinja.StatesMng.Controls
{
    class Button : Component
    {
        #region Fields

        private MouseState _currentMouse;// mouse managment

        private MouseState _previousMouse;// mouse managment

        private SpriteFont _font;

        public bool _isHovering;//if the mouse is on the button

        private Texture2D _texture;

        #endregion

        #region Properties

        public event EventHandler Click;// Event that is called once a click occours.

        public bool beenClicked { get; private set; }// if the button is clicked or not

        public Color textColour { get; set; }

        public Vector2 Position { get; set; }

        public Rectangle rec
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);
            }
        }

        public string Text { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Button's constructor
        /// </summary>

        public Button(Texture2D texture, SpriteFont font)
        {
            _texture = texture;

            _font = font;

            textColour = Color.Black;
        }

        /// <summary>
        /// draws the button with text in it.
        /// </summary>

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var colour = Color.White;

            if (_isHovering)
            {
                colour = Color.Gray;
            }

            spriteBatch.Draw(_texture, rec, colour);

            if (!string.IsNullOrEmpty(Text))// if Text contains text, draw it inside the button.
            {
                var x = (rec.X + (rec.Width / 2)) - (_font.MeasureString(Text).X / 2);
                var y = (rec.Y + (rec.Height / 2)) - (_font.MeasureString(Text).Y / 2);

                spriteBatch.DrawString(_font, Text, new Vector2(x, y), textColour);
            }
        }

        /// <summary>
        /// checks mouse hovering and call the event in case of clicking. 
        /// </summary>

        public override void Update(GameTime gameTime)
        {
            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();

            var mouseRectangle = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);
            var prevMouseRec = new Rectangle(_previousMouse.X, _previousMouse.Y, 1, 1);

            _isHovering = false;

            if (mouseRectangle.Intersects(rec))// hover
            {
                _isHovering = true;

                if (_currentMouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed)// click
                {
                    Game1._click.Play();

                    Click?.Invoke(this, new EventArgs());
                }
            }

            if (mouseRectangle.Intersects(rec) && !prevMouseRec.Intersects(rec))
            {
                Game1._hovering.Play();
            }
        }

        #endregion
    }
}
