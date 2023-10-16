using System;

/// <summary>
///  Class for points. Used to determine the position of objects. (Easier to use than Vector2)
/// </summary>
[Serializable]
public class Point
{
    public int x;
    public int y;

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Point()
    {
        x = 0;
        y = 0;
    }
    
    public override bool Equals(object obj)
    {
        if (obj is not Point point) return false;
        return this.x == point.x && this.y == point.y;
    }

    public static Point operator -(Point a, Point b) => new Point(a.x - b.x, a.y - b.y);

    /// <summary>
    ///  Returns the point of the side of the current point. 
    /// </summary>
    /// <param name="side">Side of the point</param>
    /// <returns></returns>
    public Point GetPoint(Side side)
    {
        return side switch
        {
            Side.Up => Up(),
            Side.DUp => DUp(),
            Side.Right => Right(),
            Side.DRight => DRight(),
            Side.Down => Down(),
            Side.DDown => DDown(),
            Side.Left => Left(),
            Side.DLeft => DLeft(),
            _ => this
        };
    }
    

    /// <summary>
    ///  Returns positive values of the point.
    /// </summary>
    public void Abs()
    {
        x = Math.Abs(x);
        y = Math.Abs(y);
    }
    
    public Point Up() => new Point(x, y - 1);
    public Point DUp() => new Point(x, y - 2);
    
    public Point Right() => new Point(x + 1, y);
    public Point DRight() => new Point(x + 2, y);
    
    public Point Down() => new Point(x, y + 1);
    public Point DDown() => new Point(x, y + 2);
    
    public Point Left() => new Point(x - 1, y);
    public Point DLeft() => new Point(x - 2, y);
    
    
    public override string ToString()
    {
        return "x: " + x + " y: " + y;
    }
}