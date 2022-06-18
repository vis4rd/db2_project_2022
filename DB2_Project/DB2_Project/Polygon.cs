using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

[Serializable]
[Microsoft.SqlServer.Server.SqlUserDefinedType(Format.UserDefined, IsByteOrdered = true, MaxByteSize = -1)]
public class Polygon : INullable, IBinarySerialize
{
    private List<Point> points;
    private double area;

    // Create Nulled Polygon
    public static Polygon Null
    {
        get { return new Polygon(); }
    }

    // Default Polygon constructor
    public Polygon()
    {
        this.area = 0.0;
        this.points = new List<Point>();
    }

    // Property of Polygon's area
    public double Area
    {
        get { return this.area; }
        set { this.area = value; }
    }

    // Get the number of points which form this Polygon
    public int GetPointCount()
    {
        return this.points.Count;
    }

    // Property to add new Point to this Polygon
    public Point NewPoint
    {
        set { this.points.Add(new Point(value.X, value.Y)); }
    }

    // Close Polygon = add the first Point at the end of the list
    private bool ClosePolygon()
    {
        if (!this.IsClosed())
        {
            this.points.Add(this.points[0]);
            return false;
        }
        return true;
    }

    // Check if this Polygon is closed
    private bool IsClosed()
    {
        var first = this.points[0];
        var last = this.points[this.points.Count - 1];
        return (first.X == last.X)
            && (first.Y == last.Y);
    }

    // Find orientation of ordered triplet (this, two, three).
    // The function returns following values
    // 0 --> this, two and three are collinear
    // 1 --> Clockwise
    // 2 --> Counterclockwise
    private static int TriOrientation(Point one, Point two, Point three)
    {
        double triangleArea = (two.Y - one.Y) * (three.X - two.X)
                            - (two.X - one.X) * (three.Y - two.Y);

        if (one.AreCollinear(two, three))
        {
            return 0;
        }
        if (triangleArea > 0)
        {
            return 1;
        }
        else
        {
            return 2;
        }
    }

    // Check if this Polygon is complex = if any of its sides cross
    private bool IsComplex()
    {
        if (!this.IsClosed())
        {
            this.ClosePolygon();
        }

        if (this.points.Count <= 4)
        {
            return false;
        }
        for (int i = 0; i < this.points.Count - 4; i++)
        {
            for (int j = i + 1; j < this.points.Count - 3; j++)
            {
                for (int k = j + 1; k < this.points.Count - 2; k++)
                {
                    for (int l = k + 1; l < this.points.Count - 1; l++)
                    {
                        if (Polygon.AreLinesIntersected(this.points[i], this.points[j], this.points[k], this.points[l]))
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    // Check if two lines contructed with 4 given Points cross
    private static bool AreLinesIntersected(Point oneStart, Point oneEnd, Point twoStart, Point twoEnd)
    {
        int o1 = Polygon.TriOrientation(oneStart, oneEnd, twoStart);
        int o2 = Polygon.TriOrientation(oneStart, oneEnd, twoEnd);
        int o3 = Polygon.TriOrientation(oneEnd, twoEnd, oneStart);
        int o4 = Polygon.TriOrientation(oneEnd, twoEnd, oneEnd);

        if (o1 != o2 && o3 != o4)
        {
            return true;
        }
        if (o1 == 0 && oneStart.IsOnLine(twoStart, oneEnd))
        {
            return true;
        }
        if (o2 == 0 && oneStart.IsOnLine(twoEnd, oneEnd))
        {
            return true;
        }
        if (o3 == 0 && twoStart.IsOnLine(oneStart, twoEnd))
        {
            return true;
        }
        if (o4 == 0 && twoStart.IsOnLine(oneEnd, twoEnd))
        {
            return true;
        }

        return false;
    }

    // Calculate the area of this Polygon
    public double CalculateArea()
    {
        if (!this.IsClosed())
        {
            this.ClosePolygon();
        }

        if (this.IsComplex())
        {
            this.area = 0;
        }
        else
        {
            double sum = 0.0;
            for (int i = 0; i < this.points.Count - 1; i++)
            {
                sum += (this.points[i].X * this.points[i + 1].Y);
                sum -= (this.points[i].Y * this.points[i + 1].X);
            }
            this.area = sum / 2.0;
        }
        return Math.Abs(this.area);
    }

    // Check if given x, y coordinates lie inside this Polygon
    public bool IsCoordInside(double x, double y)
    {
        return IsPointInside(new Point(x, y));
    }

    // Check if given Point lies inside this Polygon
    public bool IsPointInside(Point point)
    {
        int winding_number = 0;

        for (int i = 0; i < this.points.Count - 1; i++)
        {
            if (this.points[i].X <= point.X)
            {
                if (this.points[i + 1].X > point.X)
                {
                    if (IsLeft(this.points[i], this.points[i + 1], point) > 0)
                    {
                        winding_number++;
                    }
                }
            }
            else
            {
                if (this.points[i + 1].X <= point.X)
                {
                    if (IsLeft(this.points[i], this.points[i + 1], point) < 0)
                    {
                        winding_number--;
                    }
                }
            }
        }
        return !(winding_number == 0);
    }

    // Check which of the points is closest to the beggining of cartesian space (0, 0)
    private double IsLeft(Point one, Point two, Point three)
    {
        return ((two.Y - one.Y) * (three.X - one.X))
             - ((three.Y - one.Y) * (two.X - one.X));
    }

    // Parse String into Polygon
    public static Polygon Parse(SqlString s)
    {
        string str = s.ToString();
        var polygon = new Polygon();
        if (str.Equals(""))
        {
            return polygon;
        }

        var parts = str.Split(')');
        foreach (var part in parts)
        {
            if (part.Equals("")) { continue; }
            var pointString = part.Split('(')[1];
            var xy = pointString.Split(';');
            var x = Double.Parse(xy[0]);
            var y = Double.Parse(xy[1]);
            polygon.points.Add(new Point(x, y));
        }
        return polygon;
    }

    // Convert Polygon to String
    public override string ToString()
    {
        string result = "";
        foreach (Point point in this.points)
        {
            result += point.ToString();
            result += '\n';
        }
        return result;
    }

    // Check if this Polygon is a Null, currently always returns false
    public bool IsNull
    {
        get { return false; }
    }

    #region IBinarySerialize Members

    // Decode binary memory block into Polygon
    public void Read(System.IO.BinaryReader reader)
    {
        int j = reader.ReadInt32();
        for (int i = 0; i < j; i++)
        {
            Point buffer = new Point();
            buffer.Read(reader);
            this.points.Add(buffer);
        }
        this.area = reader.ReadDouble();
    }

    // Encode Polygon instance into binary memory block
    public void Write(System.IO.BinaryWriter writer)
    {
        writer.Write(this.points.Count);
        foreach (Point point in this.points)
        {
            point.Write(writer);
        }
        writer.Write(this.area);
    }

    #endregion
}
