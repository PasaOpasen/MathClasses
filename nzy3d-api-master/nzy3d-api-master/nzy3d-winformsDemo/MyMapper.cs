using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nzy3d_wpfDemo
{
    class MyMapper : nzy3D.Plot3D.Builder.Mapper
    {
        public MyMapper() { }
        public MyMapper(Func<double,double,double> func) { F = func; }
        private Func<double, double, double> F = null;

        public override double f(double x, double y)
        {
            if (F == null)
                return 10 * Math.Sin(x / 10) * Math.Cos(y / 20) * x;
            else
                return F(x, y);
        }

    }
}
