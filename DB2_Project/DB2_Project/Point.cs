using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using Microsoft.SqlServer.Server;

[Serializable]
[Microsoft.SqlServer.Server.SqlUserDefinedType(Format.UserDefined, IsByteOrdered = true, MaxByteSize = 64)]
public class Point : INullable, IBinarySerialize
{
    private double x;
    private double y;

    // Property of x coordinate
    public double X
    {
        get { return this.x; }
        set { this.x = value; }
    }
    
    // Property of y coordinate
    public double Y
    {
        get { return this.y; }
        set { this.y = value; }
    }

    // Check if this Point is Null
    public bool IsNull
    {
        get { return (Double.IsNaN(this.X) || Double.IsNaN(this.Y)); }
    }

    // Create Nulled Point
    public static Point Null
    {
        get { return new Point(); }
    }

    // Default Point constructor
    public Point()
    {
        this.x = 0.0;
        this.y = 0.0;
    }

    // Point constructor taking x and y coordinates as arguments
    public Point(double x, double y)
    {
        this.x = x;
        this.y = y;
    }

    // Calculate the distance to the given Point
    public double DistanceTo(Point other)
    {
        if (other.IsNull || this.IsNull)
        {
            return 0.0;
        }
        return Math.Sqrt(Math.Pow(this.X - other.X, 2) + Math.Pow(this.Y - other.Y, 2));
    }

    // Calculate A-coefficient of linear function constructed of this Point and the given one
    public double CalcA(Point other)
    {
        return (this.Y - other.Y) / (this.X - other.X);
    }

    // Calculate B-coefficient of linear function constructed of this Point and the given one
    public double CalcB(Point other)
    {
        double A = this.CalcA(other);
        return this.Y - (A * this.X);
    }

    // Given three points this, two, three, check if
    // point two lies on line segment 'this-three'
    public bool IsOnLine(Point two, Point three)
    {
        return this.AreCollinear(two, three)
            && (two.X <= Math.Max(this.X, three.X)
            && two.X >= Math.Min(this.X, three.X)
            && two.Y <= Math.Max(this.Y, three.Y)
            && two.Y >= Math.Min(this.Y, three.Y));
    }

    // Given three points this, two, three, check if they are collinear
    public bool AreCollinear(Point two, Point three)
    {
        var area = this.X * (two.Y - three.Y) +
                   two.X * (three.Y - this.Y) +
                   three.X * (this.Y - two.Y);
        if (area == 0)
            return true;
        else
            return false;
    }

    // Convert Point to its String representation
    public override string ToString()
    {
        return '(' + this.X.ToString() + "; " + this.Y.ToString() + ')';
    }

    // Parse String to Point
    public static Point Parse(SqlString s)
    {
        Point u = new Point();
        try
        {
            string[] parts = s.ToString().Split(';');
            if (parts.Length != 2)
            {
                return u;
            }
            double x;
            double y;
            if (!double.TryParse(parts[0], out x)
             || !double.TryParse(parts[1], out y))
            {
                return u;
            }
            u.X = x;
            u.Y = y;
        }
        catch (Exception e)
        {
            Console.Write(e.Message);
            return Null;
        }

        return u;
    }

    #region IBinarySerialize Members

    // Decode binary memory block into Point
    public void Read(System.IO.BinaryReader reader)
    {
        this.X = reader.ReadDouble();
        this.Y = reader.ReadDouble();
    }

    // Encode Point instance into binary memory block
    public void Write(System.IO.BinaryWriter writer)
    {
        writer.Write(this.X);
        writer.Write(this.Y);
    }

    #endregion
}
