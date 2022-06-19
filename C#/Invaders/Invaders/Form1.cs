using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime;
using System.Globalization;

namespace Invaders
{
    public partial class Form1 : Form
    {
        private byte counter;
        private Game game;
        private List<Keys> keysPressed;
        private bool runing;
        private Graphics graphics;
        private DateTime dateNow;
        private float fpsCount;
        private float fpsReal;
        private byte animationDelay;
        private bool gameOver;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Q && MessageBox.Show("Exit?", "Quit", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    Application.Exit();
            if (!runing && !gameOver)
            {
                if (!(e.KeyCode == Keys.Q))
                {
                    dateNow = DateTime.Now;
                    animationTimer.Start();
                    gameTimer.Start();
                    runing = true;
                }
            }
            else if (gameOver)
            {
                if(MessageBox.Show("Restart?", "Restart", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    StartGame();
                else
                    Application.Exit();
            }
            if (e.KeyCode == Keys.Space)
                game.FireShot();
            if (keysPressed.Contains(e.KeyCode))
                keysPressed.Remove(e.KeyCode);
            keysPressed.Add(e.KeyCode);
        }


        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (keysPressed.Contains(e.KeyCode))
                keysPressed.Remove(e.KeyCode);
        }

        private void animationTimer_Tick(object sender, EventArgs e)
        {
            if (animationDelay>10)
            {
                if (counter < 3)
                    counter++;
                else counter = 0;
                animationDelay = 0;
                game.Twinkle();
            }
            animationDelay++;
            Refresh();
            fpsCount++;
            DateTime dateTime = DateTime.Now;
            if (dateTime - dateNow >= TimeSpan.FromSeconds(1d))
            {
                fpsReal = fpsCount;
                fpsCount = 0;
                dateNow = DateTime.Now;
            }
        }
        

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            game.Go();
            foreach (Keys key in keysPressed)
                if (key == Keys.Left)
                    game.MovePlayer(Direction.Left);
                else if(key == Keys.Right)
                    game.MovePlayer(Direction.Right);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StartGame();
        }

        private void StartGame() 
        {
            game = new Game(ClientRectangle, GameOver);
            counter = 0;
            keysPressed = new List<Keys>();
            runing = false;
            fpsCount = 0f;
            animationDelay = 0;
            gameOver = false;
            Refresh();
        }
        private void GameOver()
        {
            gameTimer.Stop();
            animationTimer.Stop();
            gameOver = true;
            runing = false;
            Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (runing)
                game.Draw(e.Graphics, counter, fpsReal);
            else if (gameOver)
            {
                e.Graphics.DrawImage(Properties.Resources.gameover, ClientRectangle);
            }
            else
            {
                graphics = e.Graphics;
                graphics.DrawImage(Properties.Resources.HEHEHE, ClientRectangle);
            }
        }
    }
}
            