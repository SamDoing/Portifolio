namespace Invaders
{
    using System.Drawing;
    struct Star
    {
        internal Point Location { get; private set; }
        internal Pen MyPen { get; private set; }

        internal Star(Point location, Pen myPen)
        {
            Location = location;
            MyPen = myPen;
        }
    }
}