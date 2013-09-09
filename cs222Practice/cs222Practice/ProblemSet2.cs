using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs222Practice
{
    public class ProblemSet2
    {
        private double earthMass = 5.97e24;// kg
        private double earthRadius = 6.378e6;// m (at equator)
        private double gravitationalConstant = 6.67e-11;// m3 / kg s2
        private double moonMass = 7.35e22;// kg
        private double moonRadius = 1.74e6;// m
        private double moonDistance = 400.5e6;// m (actually, not at all a constant)
        private double moonPeriod = 27.3 * 24.0 * 3600.0;// s
        private double moonInitialAngle = Math.PI / 1800.0 * -61.0;// radian 
        private double totalDuration = 12.0 * 24.0 * 3600.0;// s
        private double markerTime = 0.5 * 3600.0;// s
        private double tolerance = 100000.0;// m 

        public ProblemSet2()
        {
            List<Vector2D> positionList;
            List<Vector2D> velocityList;
            List<double> timeList;
            double boost;
            ApplyBoost(out positionList, out velocityList, out timeList, out boost);

            PlotPath(positionList, timeList);
        }

        private void PlotPath(List<Vector2D> positionList, List<double> timeList)
        {
           
        }

        private Vector2D MoonPosition(double time)
        {
            double moonAngle = moonInitialAngle + 2.0 * Math.PI * time / moonPeriod;
            Vector2D position = new Vector2D();
            position.x = moonDistance * Math.Cos(moonAngle);
            position.y = moonDistance * Math.Sin(moonAngle);
            return position;
        }

        private Vector2D Acceleration(double time, Vector2D position)
        {
            Vector2D moonPos = MoonPosition(time);

            Vector2D vectorFromMoon = position - moonPos;
            Vector2D vectorFromEarth = position;
            Vector2D acc = -gravitationalConstant * (earthMass * vectorFromEarth / Math.Pow(vectorFromEarth.Length(), 3) + moonMass * vectorFromMoon / Math.Pow(vectorFromMoon.Length(), 3));
            return acc;
        }

        private void ApplyBoost(out List<Vector2D> positionList, out List<Vector2D> velocityList, out List<double> timesList, out double boost)
        {
            boost = 10.0; // m/s Change this to the correct value from the list above after everything else is done. 
            positionList = new List<Vector2D>() { new Vector2D(-6.701e6, 0.0) };// m
            velocityList = new List<Vector2D>() { new Vector2D(0.0, -10.818e3) }; // m/s
            timesList = new List<double>() { 0.0 };
            Vector2D position = positionList[0];
            Vector2D velocity = velocityList[0];
            double currentTime = 0.0;
            double h = 0.1;// s, set as initial step size right now but will store current step size
            double hNew = h;// s, will store the adaptive step size of the next step
            bool mcc2BurnDone = false;
            bool dps1BurnDone = false;

            while (currentTime < totalDuration)
            {
                if (!mcc2BurnDone && currentTime >= 101104.0)
                {
                    velocity -= 7.04 * velocity / velocity.Length();
                    //plot
                    dps1BurnDone = true;
                }

                if (!dps1BurnDone && currentTime >= 212100.0)
                {
                    velocity += boost * velocity / velocity.Length();
                }

                Vector2D acceleration0 = Acceleration(currentTime, position);
                Vector2D velocityE = velocity + h * acceleration0;
                Vector2D positionE = position + h * velocity;
                Vector2D velocityH = velocity + h * 0.5 * (acceleration0 + Acceleration(currentTime + h, positionE));
                Vector2D positionH = position + h * 0.5 * (velocity + velocityE);

                velocity = velocityH;
                position = positionH;

                double error = (positionE - positionH).Length() + totalDuration * ((velocityE - velocityH).Length());
                hNew = h * Math.Sqrt(tolerance / (error + 1e-50));
                hNew = Math.Min(0.5 * markerTime, Math.Max(0.1, hNew));// restrict step size to reasonable range

                currentTime += h;
                h = hNew;
                positionList.Add((Vector2D)position.Clone());
                velocityList.Add((Vector2D)velocity.Clone());
                timesList.Add(currentTime);
            }
        }
    }
}
