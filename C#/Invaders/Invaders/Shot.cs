using System;
using System.Drawing;
using System.Windows.Forms;

namespace Invaders
{
    class Shot
    {
        internal Point Location { get { return location; } }
        private Point location;
        internal bool Out { get; private set; }
        private Rectangle boundaries;

        internal void Move(Direction direction)
        {
            if (direction == Direction.Right || direction == Direction.Left)
                throw new Exception("Shot direction wrong");
            location.Y = direction == Direction.Up ? location.Y - 3 : location.Y + 3;
            if(Location.Y >= boundaries.Height || Location.Y <= boundaries.Y)
                Out = true;
            
        }

        internal Shot(Rectangle boundaries, Point location) 
        {
            this.boundaries = boundaries;
            this.location = new Point(location.X+27,location.Y);
            Out = false;
        }

        internal void Draw(Graphics g)
        {
            g.DrawLine(Pens.Red, location.X, location.Y, location.X, location.Y + 4);
        }
    }
}