using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SnowNinja.Animations.Models;
using SnowNinja.Animations.Sprites;
using SnowNinja.Internet;
using SnowNinja.StatesMng.Controls;
using SnowNinja.StatesMng.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnowNinja
{
    class MultiPlayer : State
    {
        enum OnlineState // different states of the game.
        {
            AskingRole, //host or join
            Connecting,
            PlayingServer,
            PlayingClient
        }

        OnlineAbst onlineManager;
        Texture2D vicTex;
        Texture2D defTex;
        SoundEffect hit;
        SoundEffect victory;
        SoundEffect defeat;
        bool anywon = false;
        bool activeGame = true;
        private SpriteFont _font; // to write on screen

        OnlineState state = OnlineState.AskingRole; // the initial state of the game. 

        public Dictionary<string, Animation> animations; 

        List<Platform> platforms = new List<Platform>();

        KeyboardState _currentKey;
        KeyboardState _previousKey;

        int counterVic = 0;
        int counterDef = 0;

        Button exitButton;

        Texture2D outLine;

        /// <summary>
        /// MultiPlayer's constructor.
        /// </summary>
        public MultiPlayer(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            animations = new Dictionary<string, Animation>()
            {
                {"runningR", new Animation(content.Load<Texture2D>("AnimationFrames/runningR"), 10)},
                {"runningL", new Animation(content.Load<Texture2D>("AnimationFrames/runningL"), 10)},
                {"throwingR", new Animation(content.Load<Texture2D>("AnimationFrames/throwingR"), 10)},
                {"throwingL", new Animation(content.Load<Texture2D>("AnimationFrames/throwingL"), 10)},
                {"standingL", new Animation(content.Load<Texture2D>("AnimationFrames/standingL"), 10)},
                {"standingR", new Animation(content.Load<Texture2D>("AnimationFrames/standingR"), 10)},
                {"jumpingR", new Animation(content.Load<Texture2D>("AnimationFrames/jumpingR"), 10)},
                {"jumpingL", new Animation(content.Load<Texture2D>("AnimationFrames/jumpingL"), 10)},
            };

            var platTex = content.Load<Texture2D>("General/snowPlat");
            _font = content.Load<SpriteFont>("Fonts/Font");
            hit = content.Load<SoundEffect>("Audio/ouch");
            vicTex = content.Load<Texture2D>("General/victory");
            defTex = content.Load<Texture2D>("General/defeat");
            victory = content.Load<SoundEffect>("Audio/victory");
            defeat = content.Load<SoundEffect>("Audio/defeat");

            platforms.Add(new Platform(platTex)
            {
                _position = new Vector2(20, 300),
            });
            platforms.Add(new Platform(platTex)
            {
                _position = new Vector2(20, 500),
            });

            outLine = content.Load<Texture2D>("Objects/HealthBarOutLine");

            Texture2D buttonTex = content.Load<Texture2D>("Controls/blueButton");

            MediaPlayer.Play(Game1._duelSong);

            exitButton = new Button(buttonTex, _font)
            {
                Text = "Exit",
                Position = new Vector2(Game1.ScreenWidth / 2 - buttonTex.Width / 2, 400) //roughly center of screen
            };

            exitButton.Click += ExitButton_Click;
        }
        
        /// <summary>
        /// when called, exits the game.
        /// </summary>
        private void ExitButton_Click(object sender, System.EventArgs e)
        {
            _game.Exit();
        }


        /// <summary>
        /// overrides State's Update func
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            _previousKey = _currentKey;
            _currentKey = Keyboard.GetState();

            
            switch (state)
            {
                case OnlineState.AskingRole:
                    
                    if (Keyboard.GetState().IsKeyDown(Keys.H)) // sets the user as server.
                    {
                        onlineManager = new ServerComp(5050);
                        onlineManager.connectHandler += onlineGame_OnConnectionServer;
                        onlineManager.Initialize(animations, _content);

                        state = OnlineState.Connecting;
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.J)) // sets the user as client.
                    {
                        onlineManager = new ClientComp("127.0.0.1", 5050);
                        onlineManager.connectHandler += onlineGame_OnConnectionClient;
                        onlineManager.Initialize(animations, _content);

                        state = OnlineState.Connecting; // initiates connecting.
                    }
                    break;

                case OnlineState.Connecting:
                    break;

                case OnlineState.PlayingServer:
                    if (activeGame)
                    {
                        onlineManager.serverPlayer.Update(gameTime, onlineManager.clientPlayer); // updates the player/enemy.
                        onlineManager.clientPlayer.SetAnimations();
                        onlineManager.clientPlayer.CheckHealth(onlineManager.serverPlayer);

                        foreach (var platform in platforms) // platforms update
                        {
                            if (onlineManager.serverPlayer.aniRectangle.isOnTopOf(platform.rec))
                            {
                                onlineManager.serverPlayer.Velocity.Y = 0f;
                                onlineManager.serverPlayer.hasJumped = false;

                                if (_currentKey.IsKeyUp(Keys.S) &&
                                _previousKey.IsKeyDown(Keys.S))
                                {
                                    onlineManager.serverPlayer.hasJumped = true;
                                    onlineManager.serverPlayer.Velocity.Y += 3f;
                                }
                            }

                            if (onlineManager.serverPlayer._position.Y + onlineManager.serverPlayer.texHeight >= Game1.ScreenHeight)
                            {
                                onlineManager.serverPlayer.hasJumped = false;
                            }
                            _game.Window.Title = onlineManager.serverPlayer._position.ToString();

                        }
                        PostUpdate();
                    }

                    if (anywon) // if either player or enemy has won.
                    {
                        exitButton.Update(gameTime);
                    }

                    if (onlineManager.serverPlayer.hasWon && counterVic == 0)// enabling the player to win once before reseting.
                    {
                        counterVic++;
                        anywon = true;
                        MediaPlayer.Stop();
                        victory.Play(volume: 0.2f, pitch: 0.0f, pan: 0.0f);
                    }
                    else if(onlineManager.clientPlayer.hasWon && counterDef == 0)
                    {
                        counterDef++;
                        anywon = true;
                        MediaPlayer.Stop();
                        defeat.Play(volume: 0.2f, pitch: 0.0f, pan: 0.0f);
                    }

                    break;

                case OnlineState.PlayingClient:
                    if (activeGame)
                    {
                        onlineManager.clientPlayer.Update(gameTime, onlineManager.serverPlayer); // updates the player/enemy.
                        onlineManager.serverPlayer.SetAnimations();
                        onlineManager.serverPlayer.CheckHealth(onlineManager.clientPlayer);

                        foreach (var platform in platforms) // platforms update
                        {
                            if (onlineManager.clientPlayer.aniRectangle.isOnTopOf(platform.rec))
                            {
                                onlineManager.clientPlayer.Velocity.Y = 0f;
                                onlineManager.clientPlayer.hasJumped = false;

                                if (_currentKey.IsKeyUp(Keys.S) &&
                                _previousKey.IsKeyDown(Keys.S))
                                {
                                    onlineManager.clientPlayer.hasJumped = true;
                                    onlineManager.clientPlayer.Velocity.Y += 3f;
                                }
                            }

                            if (onlineManager.clientPlayer._position.Y + onlineManager.clientPlayer.texHeight / 4 >= Game1.ScreenHeight)
                            {
                                onlineManager.clientPlayer.hasJumped = false;
                            }
                        }
                        PostUpdate();
                    }

                    if (anywon) // if either player or enemy has won.
                    {
                        exitButton.Update(gameTime);
                    }

                    if (onlineManager.clientPlayer.hasWon && counterVic == 0)// enabling the player to win once before reseting.
                    {
                        counterVic++;
                        anywon = true;
                        MediaPlayer.Stop();
                        victory.Play(volume: 0.2f, pitch: 0.0f, pan: 0.0f);
                        
                    }
                    else if(onlineManager.serverPlayer.hasWon && counterDef == 0)
                    {
                        counterDef++;
                        anywon = true;
                        MediaPlayer.Stop();
                        defeat.Play(volume: 0.2f, pitch: 0.0f, pan: 0.0f);
                    }
                    break;
            }

            _game.Window.Title = state.ToString();// prints the current state
            
        }

        /// <summary>
        /// the func that is called when there is a connection.
        /// </summary>
        private void onlineGame_OnConnectionServer()
        {
            state = OnlineState.PlayingServer;
        }

        private void onlineGame_OnConnectionClient()
        {
            state = OnlineState.PlayingClient;
        }

        /// <summary>
        /// removes the knife from set if one hit the player/enemy.
        /// </summary>
        private void PostUpdate()
        {
            for (int i = 0; i < onlineManager.serverPlayer.knivesSet.Count; i++)
            {
                if (onlineManager.serverPlayer.knivesSet[i].IsRemoved)
                {
                    hit.Play(volume: 0.15f, pitch: 0.0f, pan: 0.0f);
                    onlineManager.serverPlayer.knivesSet.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < onlineManager.clientPlayer.knivesSet.Count; i++)
            {
                if (onlineManager.clientPlayer.knivesSet[i].IsRemoved)
                {
                    hit.Play(volume: 0.15f, pitch: 0.0f, pan: 0.0f);
                    onlineManager.clientPlayer.knivesSet.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// overrides State's Draw func 
        /// </summary>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (state == OnlineState.PlayingServer || state == OnlineState.PlayingClient)
            {
                spriteBatch.Draw(outLine, new Vector2(115, 94), null, Color.White, 0, Vector2.Zero, 0.4f, SpriteEffects.None, 1);
                spriteBatch.Draw(outLine, new Vector2(1015, 94), null, Color.White, 0, Vector2.Zero, 0.4f, SpriteEffects.None, 1);

                onlineManager.clientPlayer.Draw(spriteBatch);
                onlineManager.serverPlayer.Draw(spriteBatch);

                foreach (var platform in platforms)
                {
                    platform.Draw(spriteBatch);
                }

                spriteBatch.DrawString(_font, "Player 1", new Vector2(150, 50), Color.Black);
                spriteBatch.DrawString(_font, "Player 2", new Vector2(1050, 50), Color.Black);

                if (state == OnlineState.PlayingServer)
                {
                    if (onlineManager.serverPlayer.hasWon)
                    {
                        anywon = true;
                        activeGame = false;
                        var vicX = Game1.ScreenWidth / 2 - vicTex.Width / 2 - 5;
                        spriteBatch.Draw(vicTex, new Vector2(vicX, 100), Color.White);
                        exitButton.Draw(gameTime, spriteBatch);
                    }
                    else if (onlineManager.clientPlayer.hasWon)
                    {
                        anywon = true;
                        activeGame = false;
                        spriteBatch.Draw(defTex, new Vector2((Game1.ScreenWidth / 2 - defTex.Width / 2) + 5, 100), Color.White);
                        exitButton.Draw(gameTime, spriteBatch);
                    }
                    
                }
                else
                {
                   
                    if (onlineManager.clientPlayer.hasWon)
                    {
                        anywon = true;
                        activeGame = false;
                        var vicX = Game1.ScreenWidth / 2 - vicTex.Width / 2 - 5;
                        spriteBatch.Draw(vicTex, new Vector2(vicX, 100), Color.White);
                        exitButton.Draw(gameTime, spriteBatch);
                    }
                    else if (onlineManager.serverPlayer.hasWon)
                    {
                        anywon = true;
                        activeGame = false;
                        spriteBatch.Draw(defTex, new Vector2((Game1.ScreenWidth / 2 - defTex.Width / 2) + 5, 100), Color.White);
                        exitButton.Draw(gameTime, spriteBatch);
                    }
                }

            }
        }

    }
}
