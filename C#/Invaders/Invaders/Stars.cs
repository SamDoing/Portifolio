using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Resources;

namespace Invaders
{

    class Stars
    {
        private List<Star> stars;
        private Random random;
        private Rectangle boundaries;

        public void Twinkle()
        {
            if (random.Next(2) >= 1 && stars.Count < 300)
                stars.Add(new Star(new Point(random.Next(0, boundaries.Width), random.Next(0, boundaries.Height)), StarColor()));
            else if (stars.Count > 250)
                for (int i = 0; i < 4; i++)
                    stars.RemoveAt(random.Next(1, 245));
        }

        internal void Draw(Graphics g)
        {
            foreach (Star star in stars)
                    g.DrawLine(star.MyPen, star.Location, new Point(star.Location.X + 1, star.Location.Y));
        }

        internal Stars(Rectangle boundaries, Random random)
        {
            stars = new List<Star>();
            this.boundaries = boundaries;
            Point xY = new Point(boundaries.X, boundaries.Y+15);
            this.random = random;
            for (int i = 0; i < 300; i++)
                stars.Add(new Star(new Point(random.Next(0, boundaries.Width),random.Next(0,boundaries.Height)), StarColor()));
        }

        private Pen StarColor()
        {
            int rand = random.Next(3);
            switch (rand)
            {
                case 0:
                    return Pens.Gray;
                case 1:
                    return Pens.YellowGreen;
                case 2:
                    return Pens.White;
            }
            return null;
        }
    }
}
