using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Invaders
{
    class PlayerShip
    {
        internal bool Alive { get { return alive; } }
        private bool alive;
        internal Point Location { get { return location; } }
        private Point location;
        private Rectangle boundaries;
        internal Rectangle Area { get { return new Rectangle(location, image.Size); } }
        private Bitmap image;
        private byte animationCounter = 2;

        public PlayerShip(Rectangle boundaries, Point location)
        {
            alive = true;
            this.boundaries = boundaries;
            this.location = location;
            image = new Bitmap(Properties.Resources.player);
        }

        internal void Shoted()
        {
            alive = false;
        }

        internal void Revive()
        {
            alive = true;
        }

        internal void Draw(Graphics g)
        {
            if (alive)
            {
                g.DrawImage(image, location);
                return;
            }
            Point locationToDie = new Point(location.X+(animationCounter*2), location.Y+(animationCounter*2));
            g.DrawImage(image, new Rectangle(new Point(locationToDie.X+animationCounter, locationToDie.Y+animationCounter), new Size(image.Width/animationCounter, image.Height/animationCounter)));
            if (animationCounter == 2 || animationCounter == 4) 
                animationCounter += 2;
            else
                animationCounter = 2;

        }

        internal void Move(Direction direction)
        {
            switch(direction)
            {
                case Direction.Left:
                    if (boundaries.X < Location.X - 5)
                        location.X -= 8;
                    break;
                case Direction.Right:
                    if (boundaries.Width > Location.X + 60)
                        location.X += 8;
                    break;
                default:
                    throw new Exception("Player invalid move");
            }
        }
    }
}
