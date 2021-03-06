using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tron.Objects;

namespace Tron.Game
{
    class Game
    {
        public Timer gameTime;
        // int gameTimeInterval=1;
        public Player player;
        //  public AI ai;
        public GameArea form;
        public GameController controller;
        public ArrayList playerList;
        public ArrayList livingPlayers;
        public Dictionary<Player, Line> currentLines;
        public ArrayList lines;
        public bool gameStarted;

        public KeyEventHandler KeyDown { get; private set; }

        public Game(GameArea form)
        {
            this.form = form;
            gameStarted = false;
            controller = new GameController(form);
            gameTime = new Timer();
            gameTime.Enabled = true;
            gameTime.Interval = 1;
            gameTime.Tick += new EventHandler(OnGameTimeTick);

            Movable.mainForm = form;
            playerList = new ArrayList();
            livingPlayers = new ArrayList();
            currentLines = new Dictionary<Player, Line>();
            lines = new ArrayList();

            Random rand = new Random();

            player = new Player(rand);
            player.showBorder();
            createPlayer(player);
            form.startGame(player);

            //Player player2 = new Player(rand);
            //createPlayer(player2);

            //Player player3 = new Player(rand);
            //createPlayer(player3);

            //Player player4 = new Player(rand);
            //createPlayer(player4);
        }

        void OnGameTimeTick(object sender, EventArgs e)
        {
            if (gameStarted) { 
           
            if(livingPlayers.Count > 1)
            {
                foreach (Player bike in playerList)
                {
                    currentLines[bike].update(bike);
                    bike.move();
                    controller.CollisionGameArea(bike);
                    foreach (Line line in lines)
                    {
                        controller.PaddleCollision(line, bike);
                    }
                    if (bike.isDead)
                    {
                        livingPlayers.Remove(bike);
                    }
                }

                if(livingPlayers.Count < 2)
                {
                    Label gameOver = new Label();
                    gameOver.Text = "Game Over";
                    gameOver.BackColor = Color.White;
                    gameOver.BorderStyle = BorderStyle.FixedSingle;
                    gameOver.Size = new Size(400, 200);
                    gameOver.Location = new Point(form.Width / 2 - 210, form.Height / 2 - 120);
                    gameOver.Font = new Font("Arial", 32, FontStyle.Bold);
                    gameOver.TextAlign = ContentAlignment.MiddleCenter;
                    form.Controls.Add(gameOver);
                    gameOver.BringToFront();
                }
                }

            } 
        }

        public void createPlayer(Player p)
        {
            playerList.Add(p);
            livingPlayers.Add(p);
            currentLines[p] = new Line(p);
            lines.Add(currentLines[p]);
        }

        public void addNewPlayer(string jsonPlayer)
        {
            Player p = JsonConvert.DeserializeObject<Player>(jsonPlayer);
            createPlayer(p);
        }

        public void updatePlayer(Player p, string jsonPlayer)
        {
            JObject jsonObject = JObject.Parse(jsonPlayer);
            p.playerSpeedX = (int)jsonObject.GetValue("playerSpeedX");
            p.playerSpeedY = (int)jsonObject.GetValue("playerSpeedY");
            Line newLine = new Line(p);
            currentLines[p] = newLine;
            lines.Add(newLine);
        }
    }
}