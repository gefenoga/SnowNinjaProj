using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SnowNinja.Animations.Models;
using SnowNinja.Animations.Sprites;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SnowNinja.Internet
{
    class ClientComp : OnlineAbst
    {
        string hostip; // to find the server.

        /// <summary>
        /// ClientComp's constructor.
        /// </summary>
        public ClientComp(string hostip, int port)
        {
            this.port = port;
            this.hostip = hostip;

        }

        /// <summary>
        /// creates the characters- player and enemy.
        /// </summary>
        protected override void InitPlayers(Dictionary<string, Animation> animations, ContentManager content)
        {
            healthTex = content.Load<Texture2D>("Objects/HealthBarInside");

            serverPlayer = new Sprite(animations)
            {
                PlayerType = "enemy",
                health = new HealthBar(healthTex, new Vector2(119, 101),
                    new Rectangle(0, 0, healthTex.Width - 20, healthTex.Height - 4)),
                _position = new Vector2(50, 700),
                knivesSet = new List<Knife>(),
                Speed = 5f,
                scale = 0.35f,
                knifeScale = 0.15f,
                origin = new Vector2(Game1.anyPlayerTex.Width / 20, Game1.anyPlayerTex.Height / 2),
                texHeight = Game1.anyPlayerTex.Height * 0.35f,
                color = new Color(64, 13, 13),
                

            };

            clientPlayer = new Sprite(animations)
            {
                PlayerType = "player",
                health = new HealthBar(healthTex, new Vector2(1019, 101),
                      new Rectangle(0, 0, healthTex.Width - 20, healthTex.Height - 4)),

                knivesSet = new List<Knife>(),
                Speed = 5f,
                _position = new Vector2(1200, 700),
                scale = 0.35f,
                knifeScale = 0.15f,
                origin = new Vector2(Game1.anyPlayerTex.Width / 20, Game1.anyPlayerTex.Height / 2),
                texHeight = Game1.anyPlayerTex.Height * 0.35f,
                color = Color.White,
                Input = new Input()
                {
                    Down = Keys.S,
                    Jump = Keys.Space,
                    Left = Keys.A,
                    Right = Keys.D,
                    Throw = Keys.E,
                }
            };
        }

        /// <summary>
        /// finds the server and transports data in a connection.
        /// </summary>
        protected override void SocketThread()
        {
            client = new TcpClient();
            client.Connect(hostip, port);

            reader = new BinaryReader(client.GetStream());
            writer = new BinaryWriter(client.GetStream());

            ConnectionEventTrigger();

            while (true)
            {
                WriteCharacterData(clientPlayer, serverPlayer);
                ReadAndUpdateCharacter(serverPlayer, clientPlayer);
                
                Thread.Sleep(10);
            }
        }
    }
}
