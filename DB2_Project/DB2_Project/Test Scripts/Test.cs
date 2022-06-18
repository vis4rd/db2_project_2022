using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlTypes;
using System.Reflection;

namespace DB2_Project.Test_Scripts
{
    public class Test
    {
        public void CallAllTests()
        {
            Console.WriteLine("Running all tests...");
            var tests = typeof(Test).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
            var testCount = tests.Length;

            var dict = new Dictionary<string, Func<bool>>();
            foreach (var test in tests)
            {
                if (test.ReturnType == typeof(bool))
                {
                    var func = (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), this, test);
                    dict.Add(test.Name, func);
                }
                else
                {
                    testCount--;
                }
            }

            bool result = false;
            string message = "";
            int testIndex = 1;
            int testsPassed = 0;
            foreach (KeyValuePair<string, Func<bool>> test in dict)
            {
                result = test.Value();
                message = test.Value() ? "passed" : "failed";
                Console.WriteLine("("
                    + testIndex.ToString()
                    + "/"
                    + testCount.ToString()
                    + ") "
                    + test.Key
                    + ": "
                    + message);

                testsPassed += (result ? 1 : 0);
                testIndex++;
            }
            Console.WriteLine("SUMMARY: "
                + testsPassed.ToString()
                + " out of "
                + testCount.ToString()
                + " tests passed.");
        }

        private bool TestPointDefaultConstructor()
        {
            Point point = new Point();

            return (point.X == 0) && (point.Y == 0);
        }

        private bool TestPointXyConstructor()
        {
            Point point = new Point(5, -3);

            return (point.X == 5) && (point.Y == -3);
        }

        private bool TestPointParse()
        {
            Point parsed = Point.Parse("13,7;-9,1");

            return parsed.X.Equals(13.7) && parsed.Y.Equals(-9.1);
        }

        private bool TestPointToString()
        {
            Point input = new Point(0.001, -3.12);
            string parsed = input.ToString();

            return parsed == ("(" + input.X.ToString() + "; " + input.Y.ToString() + ")");
        }

        private bool TestPointDistanceTo()
        {
            Point from = new Point(0.0, 0.0);
            Point to = new Point(3.0, 4.0);
            double expected = 5.0;

            double actual = from.DistanceTo(to);
            double actual2 = to.DistanceTo(from);

            return actual.Equals(expected) && actual2.Equals(expected);
        }

        private bool TestPointCalcA()
        {
            Point one = new Point(3, 3);
            Point two = new Point(11, 11);
            double expected = 1;

            double actual = one.CalcA(two);
            double actual2 = two.CalcA(one);

            return actual.Equals(expected) && actual2.Equals(expected);
        }

        private bool TestPointCalcB()
        {
            Point one = new Point(3, 3);
            Point two = new Point(11, 3);
            double expected = 3;

            double actual = one.CalcB(two);
            double actual2 = two.CalcB(one);

            return actual.Equals(expected) && actual2.Equals(expected);
        }

        private bool TestPointIsOnLine()
        {
            Point one = new Point(3, 3);
            Point two = new Point(7, 7);
            Point three = new Point(9, 9);
            bool expected = true;

            bool actual = one.IsOnLine(two, three);
            bool actual2 = three.IsOnLine(two, one);
            bool actualFalse = two.IsOnLine(one, three);

            return (actual == actual2 == expected) != actualFalse;
        }

        private bool TestPointAreCollinear()
        {
            Point one = new Point(3, 3);
            Point two = new Point(7, 7);
            Point three = new Point(9, 9);
            bool expected = true;

            bool actual = one.AreCollinear(two, three);
            bool actual2 = three.AreCollinear(two, one);
            bool actual3 = two.AreCollinear(one, three);

            return (actual == actual2 == actual3 == expected);
        }


        private bool TestPolygonDefaultConstructor()
        {
            Polygon poly = new Polygon();

            return (poly.Area == 0);
        }

        private bool TestPolygonNewPoint()
        {
            Polygon poly = new Polygon();
            poly.NewPoint = new Point(3.0, 7.1);

            return (poly.GetPointCount() == 1);
        }

        private bool TestPolygonGetPointCount()
        {
            Polygon poly = new Polygon();
            poly.NewPoint = new Point(3.0, 7.1);
            poly.NewPoint = new Point(3.1, 7.2);
            poly.NewPoint = new Point(3.2, 7.3);

            return (poly.GetPointCount() == 3);
        }

        private bool TestPolygonCalculateArea()
        {
            Polygon poly = new Polygon();
            poly.NewPoint = new Point(1.0, 1.0);
            poly.NewPoint = new Point(1.0, -1.0);
            poly.NewPoint = new Point(-1.0, -1.0);

            return (poly.CalculateArea() == 2);
        }

        private bool TestPolygonIsCoordInside()
        {
            Polygon poly = new Polygon();
            poly.NewPoint = new Point(1.0, 1.0);
            poly.NewPoint = new Point(1.0, -1.0);
            poly.NewPoint = new Point(-1.0, -1.0);
            poly.NewPoint = new Point(-1.0, 1.0);
            poly.NewPoint = new Point(1.0, 1.0);

            return (poly.IsCoordInside(0.0, 0.0) == true);
        }

        private bool TestPolygonIsPointInside()
        {
            Polygon poly = new Polygon();
            poly.NewPoint = new Point(1.0, 1.0);
            poly.NewPoint = new Point(1.0, -1.0);
            poly.NewPoint = new Point(-1.0, -1.0);
            poly.NewPoint = new Point(-1.0, 1.0);
            poly.NewPoint = new Point(1.0, 1.0);

            return (poly.IsPointInside(new Point()) == true);
        }
    }
}
