using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Invaders
{
    enum InvType { Star = 10, Spaceship = 20, Saucer = 30, Bug = 40, Satellite = 50 }

    class Invader
    {
        internal InvType Type { get; private set; }
        internal Point Location { get { return location; } }
        private Point location;
        internal bool Alive { get; private set; }
        private const byte HorizontalInterval = 10;
        private const byte VerticalInterval = 5;
        private Bitmap[] image = new Bitmap[4];
        private Bitmap imageToDraw;
        internal Rectangle Area { get { return new Rectangle(Location, imageToDraw.Size); } }

        internal Invader(InvType type, Point location)
        {
            this.location = location;
            Type = type;
            Alive = true;
            switch (type)
            {
                case InvType.Star:
                    image[0] = new Bitmap(Properties.Resources.star1);
                    image[1] = new Bitmap(Properties.Resources.star2);
                    image[2] = new Bitmap(Properties.Resources.star3);
                    image[3] = new Bitmap(Properties.Resources.star4);
                    break;
                case InvType.Spaceship:
                    image[0] = new Bitmap(Properties.Resources.spaceship1);
                    image[1] = new Bitmap(Properties.Resources.spaceship2);
                    image[2] = new Bitmap(Properties.Resources.spaceship3);
                    image[3] = new Bitmap(Properties.Resources.spaceship4);
                    break;
                case InvType.Saucer:
                    image[0] = new Bitmap(Properties.Resources.flyingsaucer1);
                    image[1] = new Bitmap(Properties.Resources.flyingsaucer2);
                    image[2] = new Bitmap(Properties.Resources.flyingsaucer3);
                    image[3] = new Bitmap(Properties.Resources.flyingsaucer4);
                    break;
                case InvType.Bug:
                    image[0] = new Bitmap(Properties.Resources.bug1);
                    image[1] = new Bitmap(Properties.Resources.bug2);
                    image[2] = new Bitmap(Properties.Resources.bug3);
                    image[3] = new Bitmap(Properties.Resources.bug4);
                    break;
                case InvType.Satellite:
                    image[0] = new Bitmap(Properties.Resources.satellite1);
                    image[1] = new Bitmap(Properties.Resources.satellite2);
                    image[2] = new Bitmap(Properties.Resources.satellite3);
                    image[3] = new Bitmap(Properties.Resources.satellite4);
                    break;
            }
            for (int i = 0; i < 4; i++)
                image[i] = new Bitmap(image[i], new Size(39, 29));
            imageToDraw = image[0];
        }

        internal void Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.Right:
                    location.X += VerticalInterval;
                    break;
                case Direction.Down:
                    location.Y += HorizontalInterval;
                    break;
                case Direction.Left:
                    location.X -= VerticalInterval;
                    break;
                default:
                    throw new Exception("Invader invalid direction");
            }
        }

        internal void Kill()
        {
            Alive = false;
        }

        internal void Draw(Graphics g, byte animationCell)
        {
            imageToDraw = image[animationCell];
            g.DrawImage(imageToDraw, location);
        }
    }
}
