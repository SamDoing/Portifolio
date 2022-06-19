using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Invaders
{
    enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    delegate void GameOver();
    class Game
    {
        private int score = 0;
        private int framesSkipped = 0;
        private int wave = 1;
        private byte life = 3;

        private Rectangle boundaries;
        private Rectangle lifeDisplay;
        private Random random;
        private Direction invaderDirection;

        private Stars stars;
        private GameOver gameOver;
        private PlayerShip player;


        private List<Invader> invaders;
        private List<Shot> shotsEnemy;
        private List<Shot> shotsPlayer;

        internal void Go()
        {
            if (life <= 0 && framesSkipped == 0)
            {
                gameOver();
                return;
            }

            if (framesSkipped <= 0)
                framesSkipped = 10 - wave;
            framesSkipped--;
            if (!player.Alive && framesSkipped <= 0)
                player.Revive();

            for (int i = shotsEnemy.Count - 1; i >= 0; i--)
            {
                shotsEnemy[i].Move(Direction.Down);
                if (shotsEnemy[i].Out)
                    shotsEnemy.RemoveAt(i);
            }
            for (int i = 0; i < shotsPlayer.Count; i++)
            {
                shotsPlayer[i].Move(Direction.Up);
                if (shotsPlayer[i].Out)
                    shotsPlayer.RemoveAt(i);
            }
            if (player.Alive)
            {
                var invadersShot =
                        from invader in invaders
                        where invader.Type == invaders.Last().Type
                        select invader;

                var invadersDown =
                    from inv in invaders
                    where inv.Location.X > boundaries.Width - 50 || inv.Location.X < boundaries.X + 10
                    select inv;

                if (invaderDirection == Direction.Down && invaders.Exists(inv => inv.Location.X <= boundaries.X + 50))
                    invaderDirection = Direction.Right;
                else if (invaderDirection == Direction.Down && invaders.Exists(inv => inv.Location.X >= boundaries.Width - 50))
                    invaderDirection = Direction.Left;
                else if (invadersDown.Count() >= 1)
                    invaderDirection = Direction.Down;

                for (int i = invaders.Count - 1; i >= 0; i--)
                {
                    for (int b = shotsPlayer.Count - 1; b >= 0; b--)
                    {
                        if (invaders[i].Area.Contains(shotsPlayer[b].Location))
                        {
                            score += (int) invaders[i].Type;
                            invaders[i].Kill();
                            shotsPlayer.RemoveAt(b);
                        }
                    }
                    if (!invaders[i].Alive)
                        invaders.RemoveAt(i);
                }

                foreach (Invader invaderToShot in invadersShot)
                    if (shotsEnemy.Count < 1 + wave && random.Next(10) > 5 && framesSkipped == 0)
                        shotsEnemy.Add(new Shot(boundaries, new Point(invaderToShot.Location.X+3, invaderToShot.Location.Y+10)));
                if (framesSkipped <= 0)
                    foreach (Invader invMove in invaders)
                          invMove.Move(invaderDirection);

                for (int i = shotsEnemy.Count - 1; i >= 0; i--)
                    if (player.Area.Contains(shotsEnemy[i].Location))
                    {
                        player.Shoted();
                        shotsEnemy.RemoveAt(i);
                        framesSkipped = 60;
                        life--;
                    }
            }

            if (invaders.Count <= 0)
                NextWave();
        }

        internal void Draw(Graphics g, byte animationCell, float fps)
        {
            g.FillRectangle(Brushes.Black, boundaries);
            g.DrawString("Score: " + score.ToString(), new Font(new FontFamily("Arial"), 10), Brushes.White, boundaries.X, boundaries.Y);
            stars.Draw(g);
            foreach (Shot shot in shotsEnemy)
                shot.Draw(g);
            foreach (Shot shot in shotsPlayer)
                shot.Draw(g);
            foreach (Invader invader in invaders)
                invader.Draw(g, animationCell);
            player.Draw(g);
            g.DrawString("FPS: " + fps.ToString(), new Font(new FontFamily("Arial"), 10), Brushes.White, boundaries.Width/2, boundaries.Y);
            switch (life)
            {
                case 3:
                    g.DrawImage(Properties.Resources.player, lifeDisplay);
                    g.DrawImage(Properties.Resources.player, lifeDisplay.X + 50, lifeDisplay.Y, lifeDisplay.Width, lifeDisplay.Height);
                    g.DrawImage(Properties.Resources.player, lifeDisplay.X + 100, lifeDisplay.Y, lifeDisplay.Width, lifeDisplay.Height);
                    break;
                case 2:
                    g.DrawImage(Properties.Resources.player, lifeDisplay.X + 50, lifeDisplay.Y, lifeDisplay.Width, lifeDisplay.Height);
                    g.DrawImage(Properties.Resources.player, lifeDisplay.X + 100, lifeDisplay.Y, lifeDisplay.Width, lifeDisplay.Height);
                    break;
                case 1:
                    g.DrawImage(Properties.Resources.player, lifeDisplay.X + 100, lifeDisplay.Y, lifeDisplay.Width, lifeDisplay.Height);
                    break;
            }
        }

        internal Game(Rectangle boundaries, GameOver gameOver)
        {
            this.boundaries = boundaries;
            player = new PlayerShip(boundaries, new Point(boundaries.Width / 2, boundaries.Height - 35));
            this.gameOver = gameOver;
            random = new Random();
            invaderDirection = Direction.Right;
            stars = new Stars(boundaries, random);
            shotsPlayer = new List<Shot>();
            shotsEnemy = new List<Shot>();
            CreateInvaders();
        }

        private void CreateInvaders()
        {
            invaders = new List<Invader>();
            bool keep = true;
            int count = 1;
            Point invaderLocation = new Point(190, 40);
            while (keep)
            {
                int xInterval = 80;
                int yInterval = 60;
                if (count <= 5)
                {
                    invaders.Add(new Invader(InvType.Satellite, invaderLocation));
                    invaderLocation.X += xInterval;
                }
                else if (count >= 6 && count <= 10)
                {
                    if (count == 6)
                        invaderLocation.Y += yInterval;
                    invaderLocation.X -= xInterval;
                    invaders.Add(new Invader(InvType.Bug, invaderLocation));
                }
                else if (count >= 11 && count <= 15)
                {
                    if (count == 11)
                        invaderLocation.Y += yInterval;
                    invaders.Add(new Invader(InvType.Spaceship, invaderLocation));
                    invaderLocation.X += xInterval;
                }
                else if (count >= 16 && count <= 20)
                {
                    if (count == 16)
                        invaderLocation.Y += yInterval;
                    invaderLocation.X -= xInterval;
                    invaders.Add(new Invader(InvType.Saucer, invaderLocation));
                }
                else if (count >= 21 && count <= 25)
                {
                    if (count == 21)
                        invaderLocation.Y += yInterval;
                    invaders.Add(new Invader(InvType.Star, invaderLocation));
                    invaderLocation.X += xInterval;
                }
                count++;
                if (count >= 30)
                    keep = false;
                lifeDisplay = new Rectangle(boundaries.Width - 150, boundaries.Y+5, 30, 20);

            }
        }

        internal void Twinkle()
        {
            stars.Twinkle();
        }

        internal void FireShot()
        {
            if (!player.Alive)
                return;
            int countShot = shotsEnemy.Count + shotsPlayer.Count;
            if (countShot < 4)
                shotsPlayer.Add(new Shot(boundaries, player.Location));
        }

        internal void MovePlayer(Direction direction)
        {
            if (player.Alive)
                player.Move(direction);
        }

        private void NextWave()
        {
            wave++;
            CreateInvaders();
        }
    }
}
