using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SnowNinja.Animations.Models;
using SnowNinja.Animations.Sprites;
using SnowNinja.Snow;
using SnowNinja.StatesMng.Controls;
using SnowNinja.StatesMng.States;
using System.Collections.Generic;

namespace SnowNinja
{    
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static int ScreenWidth; 
        public static int ScreenHeight;

        SnowGenerator snow;

        Player player; 
        Bot bot;

        private SpriteFont _font; // to write on screen

        //static for using in other classes.
        public static Song _menuSong;
        public static Song _duelSong;
        public static SoundEffect _throwing;
        public static SoundEffect _click;
        public static SoundEffect _hovering;

        SoundEffect victory;
        SoundEffect defeat;
        SoundEffect hit;
        SoundEffect exitClick;
        
        public static Texture2D anyPlayerTex;

        public static Texture2D knifeTexture;

        public static Texture2D healthTex;

        Texture2D bg;
        Texture2D optionsTex;
        Texture2D title;
        Texture2D guide;
        Texture2D outLine;
        Texture2D vicTex;
        Texture2D defTex;

        KeyboardState _currentKey;
        KeyboardState _previousKey;
        Button exitButton; // for optionsMenu

        List<Platform> platforms = new List<Platform>();
        
        public static bool escPress = false; // to check if esc is pressed once
        public static bool activeGame = false; 
        public static bool inMenu = true; //for returning to game & menu in optionsMenu
        bool anywon = false;

        int counterVic = 0;
        int counterDef = 0;

        #region States
        private State _currentState;

        private State _nextState;

        //for optionsMenu
        private State _currentOPstate;

        private State _nextOPstate;

        public void ChangeState(State state)
        {
            _nextState = state;
        }

        #endregion
        
        /// <summary>
        /// Game1's constructor.
        /// </summary>
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// sets window sizes and game title.
        /// </summary>
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1300;
            graphics.PreferredBackBufferHeight = 731;
            graphics.ApplyChanges();

            ScreenWidth = graphics.PreferredBackBufferWidth;
            ScreenHeight = graphics.PreferredBackBufferHeight;

            Window.Title = "Snow Ninja!";

            IsMouseVisible = true;
            base.Initialize();
        }
        
        /// <summary>
        /// loads all the content (Texture2D, Song, SoundEffect, Font) from Content file.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            _menuSong = Content.Load<Song>("Audio/intro music");
            MediaPlayer.Play(_menuSong);
            MediaPlayer.Volume = 0.3f;
            MediaPlayer.IsRepeating = true; // infinite loop 
            
            title = Content.Load<Texture2D>("General/GameTitle");
            _throwing = Content.Load<SoundEffect>("Audio/throwing");
            victory = Content.Load<SoundEffect>("Audio/victory");
            defeat = Content.Load<SoundEffect>("Audio/defeat");
            hit = Content.Load<SoundEffect>("Audio/ouch");
            _hovering = Content.Load<SoundEffect>("Audio/hovering");
            _click = Content.Load<SoundEffect>("Audio/button click");
            exitClick = Content.Load<SoundEffect>("Audio/exit click");
            _duelSong = Content.Load<Song>("Audio/Warrior Rising");
            knifeTexture = Content.Load<Texture2D>("Objects/Kunai");
            optionsTex = Content.Load<Texture2D>("General/options frame");
            vicTex = Content.Load<Texture2D>("General/victory");
            defTex = Content.Load<Texture2D>("General/defeat");
            outLine = Content.Load<Texture2D>("Objects/HealthBarOutLine");
            healthTex = Content.Load<Texture2D>("Objects/HealthBarInside");
            anyPlayerTex = Content.Load<Texture2D>("AnimationFrames/runningR");
            _font = Content.Load<SpriteFont>("Fonts/Font");
            Texture2D platTex = Content.Load<Texture2D>("General/snowPlat");
            Texture2D buttonTex = Content.Load<Texture2D>("Controls/blueButton");
            bg = Content.Load<Texture2D>("General/snowBack2");
            guide = Content.Load<Texture2D>("General/Guide");

            snow = new SnowGenerator(Content.Load<Texture2D>("Snow/smallParticle"), ScreenWidth, 100);

            Dictionary<string, Animation> animations = new Dictionary<string, Animation>() // gets the animation and num of frames.
            {
                {"runningR", new Animation(Content.Load<Texture2D>("AnimationFrames/runningR"), 10)},
                {"runningL", new Animation(Content.Load<Texture2D>("AnimationFrames/runningL"), 10)},
                {"throwingR", new Animation(Content.Load<Texture2D>("AnimationFrames/throwingR"), 10)},
                {"throwingL", new Animation(Content.Load<Texture2D>("AnimationFrames/throwingL"), 10)},
                {"standingL", new Animation(Content.Load<Texture2D>("AnimationFrames/standingL"), 10)},
                {"standingR", new Animation(Content.Load<Texture2D>("AnimationFrames/standingR"), 10)},
                {"jumpingR", new Animation(Content.Load<Texture2D>("AnimationFrames/jumpingR"), 10)},
                {"jumpingL", new Animation(Content.Load<Texture2D>("AnimationFrames/jumpingL"), 10)},
            };

            player = new Player(animations);

            bot = new Bot(animations, player);
            
            platforms.Add(new Platform(platTex)
            {
                _position = new Vector2(20, 300),
            });
            platforms.Add(new Platform(platTex)
            {
                _position = new Vector2(20, 500),
            });
            
            _currentState = new MainMenu(this, graphics.GraphicsDevice, Content);
            _currentOPstate = new OptionsMenu(this, graphics.GraphicsDevice, Content);
            
            exitButton = new Button(buttonTex, _font)
            {
                Text = "Exit",
                Position = new Vector2(ScreenWidth / 2 - buttonTex.Width / 2, 400) //roughly center of screen
            };

            exitButton.Click += ExitButton_Click;
            
        }

        /// <summary>
        /// when called, exits the game.
        /// </summary>
        private void ExitButton_Click(object sender, System.EventArgs e)
        {
            Exit();
        }
        
        /// <summary>
        /// updates the different elements of the game.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            // used to check if a key is pressed once. 
            _previousKey = _currentKey;
            _currentKey = Keyboard.GetState();

            snow.Update(gameTime, graphics.GraphicsDevice);

            if (anywon) // if either player or enemy has won.
            {
                exitButton.Update(gameTime);
            }

            if (_currentKey.IsKeyDown(Keys.Escape))
            {
                escPress = true;
            }
            if (_currentOPstate.gameActivation && escPress && !_currentOPstate.returnToMenu) //stops the game and updates the options menu.
            {
                activeGame = false;
                MediaPlayer.Stop();
                if (_nextOPstate != null)
                {
                    _currentOPstate = _nextOPstate;

                    _nextOPstate = null;
                }

                _currentOPstate.Update(gameTime);

            }

            if (activeGame)
            {
                #region platforms

                foreach (var platform in platforms)
                {
                    if (player.aniRectangle.isOnTopOf(platform.rec)) // if the player has landed on a platform.
                    {
                        player.Velocity.Y = 0f;
                        player.hasJumped = false;

                        if (_currentKey.IsKeyUp(player.Input.Down) &&
                        _previousKey.IsKeyDown(player.Input.Down))
                        {
                            player.hasJumped = true;
                            player.Velocity.Y += 3f;
                        }
                    }

                    if (player._position.Y + player.texHeight >= ScreenHeight)
                    {
                        player.hasJumped = false;
                    }
                    
                    if (bot.aniRectangle.isOnTopOf(platform.rec))// if the bot has landed on a platform.
                    {
                        bot.Velocity.Y = 0f;
                        bot.hasJumped = false;

                        if (bot.decision.Equals("down"))
                        {
                            bot.hasJumped = true;
                            bot.Velocity.Y += 3f;
                        }
                    }

                    if (bot._position.Y + bot.texHeight / 4 >= ScreenHeight)
                    {
                        bot.hasJumped = false;

                    }
                }

                #endregion
                

                player.Update(gameTime, bot);
                bot.Update(gameTime, player);
                
                PostUpdate(); //knife managing.

                if (_currentOPstate.returnToMenu)// when going back to menu, the game resets.
                {
                    _currentState.gameActivation = false;
                    resetPlayers();
                    _currentState.guideActivation = false;
                    _currentOPstate.returnToMenu = false;
                    inMenu = true;

                }
                if (!_currentState.guideActivation && !_currentState.gameActivation)
                {
                    _currentState.Update(gameTime);
                    inMenu = true;
                }

            }

            #region States updates
            if (_nextState != null)
            {
                _currentState = _nextState;

                _nextState = null;
            }
            
            if (!_currentState.guideActivation && !_currentState.gameActivation) // in main menu
            {
                _currentState.Update(gameTime);
            }

            #endregion

            #region win&lose scenario
            if (player.hasWon && counterVic == 0)// enabling the player to win once before reseting.
            {
                anywon = true;
                MediaPlayer.Stop();
                victory.Play(volume: 0.2f, pitch: 0.0f, pan: 0.0f);
                counterVic++;
            }
            if (bot.hasWon && counterDef == 0)// enabling the player to win once before reseting.
            {
                counterDef++;
                anywon = true;
                MediaPlayer.Stop();
                defeat.Play(volume: 0.2f, pitch: 0.0f, pan: 0.0f);
            }
            #endregion

            base.Update(gameTime);
        }

        /// <summary>
        /// removes the knife from set if one hit the player/enemy.
        /// </summary>
        private void PostUpdate()
        {
            for (int i = 0; i < player.knivesSet.Count; i++)
            {
                if (player.knivesSet[i].IsRemoved)
                {
                    hit.Play(volume: 0.15f, pitch: 0.0f, pan: 0.0f);
                    player.knivesSet.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < bot.knivesSet.Count; i++)
            {
                if (bot.knivesSet[i].IsRemoved)
                {
                    hit.Play(volume: 0.15f, pitch: 0.0f, pan: 0.0f);
                    bot.knivesSet.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// max their health back and takes them back to their positions.
        /// </summary>
        private void resetPlayers()
        {
            player.health.rectangle.Width = Content.Load<Texture2D>("Objects/HealthBarInside").Width - 20;
            player._position.X = 50;
            player._position.Y = 700;
            player.knivesSet.Clear();

            bot.health.rectangle.Width = Content.Load<Texture2D>("Objects/HealthBarInside").Width - 20;
            bot._position.X = 1200;
            bot._position.Y = 700;
            bot.knivesSet.Clear();

        }

        /// <summary>
        /// draws the entire game.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            spriteBatch.Draw(bg, new Vector2(0, 0), Color.White);
            snow.Draw(spriteBatch);

            if (!_currentState.gameActivation)//if in main menu
            {
                spriteBatch.Draw(title, new Vector2(350, 30), null, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 1);

                _currentState.Draw(gameTime, spriteBatch); // drawing the menu

                #region guide
                if (_currentState.guideActivation == true)
                {
                    spriteBatch.Draw(guide, new Vector2(225, 125), null, Color.White, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 1);
                    if (_currentKey.IsKeyUp(Keys.Escape) && _previousKey.IsKeyDown(Keys.Escape))
                    {
                        exitClick.Play(volume: 0.3f, pitch: 0.0f, pan: 0.0f);
                        _currentState.guideActivation = false;
                    }
                }
                #endregion

            }

            else
            {
                #region drawing the game
                _currentOPstate.gameActivation = true;
                spriteBatch.Draw(outLine, new Vector2(115, 94), null, Color.White, 0, Vector2.Zero, 0.4f, SpriteEffects.None, 1);
                spriteBatch.Draw(outLine, new Vector2(1015, 94), null, Color.White, 0, Vector2.Zero, 0.4f, SpriteEffects.None, 1);

                spriteBatch.DrawString(_font, "My Player", new Vector2(150, 50), Color.Black);
                spriteBatch.DrawString(_font, "Enemy", new Vector2(1050, 50), Color.Black);

                foreach (var platform in platforms)
                {
                    platform.Draw(spriteBatch);
                }

                player.Draw(spriteBatch);
                bot.Draw(spriteBatch);

                if (_currentOPstate.gameActivation && escPress)// to draw the options menu 
                {
                    spriteBatch.Draw(optionsTex, new Vector2(390, 60), null, Color.White, 0f, Vector2.Zero, 0.9f, SpriteEffects.None, 1);
                    _currentOPstate.Draw(gameTime, spriteBatch);
                }


                #region win&lose scenario
                if (player.hasWon)
                {
                    activeGame = false;
                    var vicX = ScreenWidth / 2 - vicTex.Width / 2 - 5;
                    spriteBatch.Draw(vicTex, new Vector2(vicX, 100), Color.White);
                    exitButton.Draw(gameTime, spriteBatch);
                }

                if (bot.hasWon)
                {
                    activeGame = false;
                    spriteBatch.Draw(defTex, new Vector2((ScreenWidth / 2 - defTex.Width / 2) + 5, 100), Color.White);
                    exitButton.Draw(gameTime, spriteBatch);
                }
                #endregion


                #endregion
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}


// a class that helps with the platforms.
static class RectangleHelper
{
    const int penetrationMargin = 47; // custom number chosen by viewing the game.
    
    /// <summary>
    /// checks if the character has landed on the platform.
    /// </summary>
    /// <param name="r1"> character's rectangle</param>
    /// <param name="r2"> platform's rectangle</param>
    /// 
    public static bool isOnTopOf(this Rectangle r1, Rectangle r2)
    {
        return r1.Bottom >= r2.Top + penetrationMargin &&
                r1.Bottom <= r2.Top + (r2.Height / 2 + 32);
    }

}