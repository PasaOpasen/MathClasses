﻿namespace Computator.NET.DataTypes.Charts
{
    public class Point2D
    {
        public Point2D()
        {
        }

        public Point2D(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public double x { get; set; }
        public double y { get; set; }
    }
}