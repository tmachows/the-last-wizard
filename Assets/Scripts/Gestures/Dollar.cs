using System;
using System.Collections.Generic;
using UnityEngine;

#region Helpers

public struct Point
{
    public float x;
    public float y;
}

public struct Rectangle
{
    public float x;
    public float y;
    public float height;
    public float width;
}


public struct Result
{
    public bool matched;
    public String name;
    public float score;
}

#endregion

#region dollar impl
public class DollarUtil
{

    public static LinkedList<Point> Resample(LinkedList<Point> points, int n)
    {
        float I = PathLength(points) / (float)(n - 1);
        float D = 0f;
        var newPoints = new LinkedList<Point>();
        newPoints.AddLast(points.First.Value);
        var p = points.First.Next;
        while (p != points.Last.Next)
        {
            var d = Distance(p.Previous.Value, p.Value);
            if ((D + d) >= I)
            {
                Point q;
                q.x = p.Previous.Value.x + ((I - D) / d) * (p.Value.x - p.Previous.Value.x);
                q.y = p.Previous.Value.y + ((I - D) / d) * (p.Value.y - p.Previous.Value.y);
                newPoints.AddLast(q);
                p = points.AddBefore(p, q);
                D = 0f;
            }
            else
            {
                D += d;
            }
            p = p.Next;

        }
        if (newPoints.Count == n - 1)
        {
            Point q;
            q.x = points.Last.Value.x;
            q.y = points.Last.Value.y;
            newPoints.AddLast(q);
        }
        return newPoints;
    }

    public static float PathLength(LinkedList<Point> points)
    {
        float d = 0f;
        LinkedListNode<Point> p = points.First.Next;
        while (p != null)
        {
            d += Distance(p.Previous.Value, p.Value);
            p = p.Next;
        }
        return d;
    }

    public static float Distance(Point a, Point b)
    {
        return (float)Math.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));
    }

    public static float IndicativeAngle(LinkedList<Point> points)
    {
        var c = Centroid(points);
        return (float)Math.Atan2(c.y - points.First.Value.y, c.x - points.First.Value.x);
    }

    public static Point Centroid(LinkedList<Point> points)
    {
        Point point;
        point.x = 0f;
        point.y = 0f;
        foreach (var p in points)
        {
            point.x += p.x;
            point.y += p.y;
        }
        point.x /= points.Count;
        point.y /= points.Count;
        return point;
    }

    public static LinkedList<Point> RotateBy(LinkedList<Point> points, float angle)
    {
        var c = Centroid(points);
        float cos = (float)Math.Cos(angle);
        float sin = (float)Math.Sin(angle);
        var newPoints = new LinkedList<Point>();
        foreach (var p in points)
        {
            Point q;
            q.x = (p.x - c.x) * cos - (p.y - c.y) * sin + c.x;
            q.y = (p.x - c.x) * sin + (p.y - c.y) * cos + c.y;
            newPoints.AddLast(q);
        }
        return newPoints;
    }

    public static LinkedList<Point> ScaleTo(LinkedList<Point> points, float size)
    {
        var B = BoundingBox(points);
        var newPoints = new LinkedList<Point>();
        foreach (var p in points)
        {
            Point q;
            q.x = p.x * (size / B.width);
            q.y = p.y * (size / B.height);
            newPoints.AddLast(q);
        }
        return newPoints;
    }

    public static Rectangle BoundingBox(LinkedList<Point> points)
    {
        var minX = points.First.Value.x;
        var maxX = points.First.Value.x;
        var minY = points.First.Value.y;
        var maxY = points.First.Value.y;
        foreach (var p in points)
        {
            minX = Math.Min(minX, p.x);
            maxX = Math.Max(maxX, p.x);
            minY = Math.Min(minY, p.y);
            maxY = Math.Max(maxY, p.y);
        }
        Rectangle r;
        r.x = minX;
        r.y = minY;
        r.width = maxX - minX;
        r.height = maxY - minY;
        return r;
    }

    public static LinkedList<Point> TranslateTo(LinkedList<Point> points, Point point)
    {
        var c = Centroid(points);
        var newPoints = new LinkedList<Point>();
        foreach (var p in points)
        {
            Point q;
            q.x = p.x + point.x - c.x;
            q.y = p.y + point.y - c.y;
            newPoints.AddLast(q);
        }
        return newPoints;
    }

    public static LinkedList<Point> Vectorize(LinkedList<Point> points)
    {
        float sum = 0f;
        var vector = new LinkedList<Point>();
        foreach (var p in points)
        {
            Point q;
            q.x = p.x;
            q.y = p.y;
            vector.AddLast(q);
            sum += p.x * p.x + p.y * p.y;
        }
        float magnitude = (float)Math.Sqrt(sum);
        LinkedListNode<Point> p_ = vector.First;
        while (p_ != null)
        {
            Point q;
            q.x = p_.Value.x / magnitude;
            q.y = p_.Value.y / magnitude;
            p_.Value = q;
            p_ = p_.Next;
        }
        return vector;
    }
    public static float OptimalCosineDistance(LinkedList<Point> v, LinkedList<Point> w) {
        float a = 0f;
        float b = 0f;
        LinkedListNode<Point> vNode = v.First;
        LinkedListNode<Point> wNode = w.First;

        while(vNode != null)
        {
            a += vNode.Value.x * wNode.Value.x + vNode.Value.y * wNode.Value.y;
            b += vNode.Value.x * wNode.Value.y - vNode.Value.y * wNode.Value.x;
            vNode = vNode.Next;
            wNode = wNode.Next;
        }

        float angle = (float)Math.Atan(b / a);

        return (float)Math.Acos(a * Math.Cos(angle + b * Math.Sin(angle)));
    }
    public static float DistanceAtBestAngle(LinkedList<Point> points, Unistroke unistroke, float a, float b, float precision)
    {
        float x1 = DollarConst.PHI * a + (1f - DollarConst.PHI) * b;
        float f1 = DistanceAtAngle(points, unistroke, x1);
        float x2 = (1f - DollarConst.PHI) * a + DollarConst.PHI * b;
        float f2 = DistanceAtAngle(points, unistroke, x2);

        while(Math.Abs(b - a) > precision)
        {
            if(f1 < f2)
            {
                b = x2;
                x2 = x1;
                f2 = f1;
                x1 = DollarConst.PHI * a + (1f - DollarConst.PHI) * b;
                f1 = DistanceAtAngle(points, unistroke, x1);
            } else
            {
                a = x1;
                x1 = x2;
                f1 = f2;
                x2 = (1f - DollarConst.PHI) * a + DollarConst.PHI * b;
                f2 = DistanceAtAngle(points, unistroke, x2);
            }
        }
        return Math.Min(f1, f2);
    }
    public static float DistanceAtAngle(LinkedList<Point> points, Unistroke unistroke, float angle)
    {
        var newPoints = RotateBy(points, angle);
        return PathDistance(newPoints, unistroke.getPoints());
    }
    public static float PathDistance(LinkedList<Point> v, LinkedList<Point> w)
    {
        float d = 0f;
        var vNode = v.First;
        var wNode = w.First;
        while(vNode != null)
        {
            d += Distance(vNode.Value, wNode.Value);
            vNode = vNode.Next;
            wNode = wNode.Next;
        }
        return d / v.Count;
    }
}
#endregion

#region Consts
public class DollarConst
{
    public const int NUM_UNISTROKES = 16;
    public const int NUM_POINTS = 64;
    public const float SQUARE_SIZE = 250f;
    public static Point ORIGIN = new Point { x = 0, y = 0 };
    public static float DIAGONAL = (float)(Math.Sqrt(SQUARE_SIZE * SQUARE_SIZE + SQUARE_SIZE * SQUARE_SIZE));
    public static float HALF_DIAGONAL = DIAGONAL / 2;
    public static float ANGLE_RANGE = (float)(Math.PI / 4);
    public static float ANGLE_PRECISION = (float)(Math.PI / 90);
    public static float PHI = (float)(0.5f * (-1f + Math.Sqrt(5d)));
}
#endregion

public class Unistroke
{
    #region members & constructor
    private String _Name;
    private LinkedList<Point> _Points;
    private LinkedList<Point> _Vector;

    public Unistroke(String name, LinkedList<Point> points)
    {
        _Name = name;
        _Points = DollarUtil.Resample(points, DollarConst.NUM_POINTS);
        _Points = DollarUtil.RotateBy(_Points, DollarUtil.IndicativeAngle(_Points) * (-1));
        _Points = DollarUtil.ScaleTo(_Points, DollarConst.SQUARE_SIZE);
        _Points = DollarUtil.TranslateTo(_Points, DollarConst.ORIGIN);
        _Vector = DollarUtil.Vectorize(_Points);
    }
    #endregion

    #region getters
    public String getName()
    {
        return _Name;
    }
    public LinkedList<Point> getPoints()
    {
        return _Points;
    }
    public LinkedList<Point> getVector()
    {
        return _Vector;
    }
    #endregion
}


public class Dollar
{
    #region members & constructor
    private bool _UseProtractor;
    private List<Unistroke> _Unistrokes;

    public Dollar(List<Unistroke> unistrokes, bool useProtractor)
    {
        _Unistrokes = unistrokes;
        _UseProtractor = useProtractor;
    }
    #endregion

    #region recognize impl
    public Result Recognize(LinkedList<Point> points)
    {
        points = DollarUtil.Resample(points, DollarConst.NUM_POINTS);
        if(points.Count < DollarConst.NUM_POINTS)
        {
            Result resultFailed;
            resultFailed.matched = false;
            resultFailed.name = "None";
            resultFailed.score = 0f;
            return resultFailed;
        }
        points = DollarUtil.RotateBy(points, DollarUtil.IndicativeAngle(points) * (-1));
        points = DollarUtil.ScaleTo(points, DollarConst.SQUARE_SIZE);
        points = DollarUtil.TranslateTo(points, DollarConst.ORIGIN);
        var vector = DollarUtil.Vectorize(points);
        float b = float.PositiveInfinity;
        Unistroke best = null;
        foreach(var u in _Unistrokes)
        {
            float d;
            if(_UseProtractor)
            {
                d = DollarUtil.OptimalCosineDistance(u.getVector(), vector);
            } else
            {
                d = DollarUtil.DistanceAtBestAngle(points, u, DollarConst.ANGLE_RANGE * (-1), DollarConst.ANGLE_RANGE, DollarConst.ANGLE_PRECISION);
            }
            if (d < b)
            {
                b = d;
                best = u;
            }
        }
        Result r;
        if (best == null)
        {
            r.name = "None";
            r.score = 0f;
            r.matched = false;
        } else
        {
            r.name = best.getName();
            r.matched = true;
            if (_UseProtractor)
            {
                r.score = 1f / b;
            } else
            {
                r.score = 1f - b / DollarConst.HALF_DIAGONAL;
            }
        }
        return r;
    }
    #endregion
}

public class DollarWrapper
{
    private Dollar _Dollar;

    public DollarWrapper()
    {
        var unistrokes = PopulateUnistrokes();
        _Dollar = new Dollar(unistrokes, true);
    }

    public Result Recognize(LinkedList<Point> points)
    {
        return _Dollar.Recognize(points);
    }

    #region hardcoded data
    public List<Unistroke> PopulateUnistrokes()
    {
        var list = new List<Unistroke>();
        
        #region triangle
        var pointList = new LinkedList<Point>();
        pointList.AddLast(new Point() { x = 137, y = 139 }); pointList.AddLast(new Point() { x = 135, y = 141 }); pointList.AddLast(new Point() { x = 133, y = 144 }); pointList.AddLast(new Point() { x = 132, y = 146 }); pointList.AddLast(new Point() { x = 130, y = 149 }); pointList.AddLast(new Point() { x = 128, y = 151 }); pointList.AddLast(new Point() { x = 126, y = 155 }); pointList.AddLast(new Point() { x = 123, y = 160 }); pointList.AddLast(new Point() { x = 120, y = 166 }); pointList.AddLast(new Point() { x = 116, y = 171 }); pointList.AddLast(new Point() { x = 112, y = 177 }); pointList.AddLast(new Point() { x = 107, y = 183 }); pointList.AddLast(new Point() { x = 102, y = 188 }); pointList.AddLast(new Point() { x = 100, y = 191 }); pointList.AddLast(new Point() { x = 95, y = 195 }); pointList.AddLast(new Point() { x = 90, y = 199 }); pointList.AddLast(new Point() { x = 86, y = 203 }); pointList.AddLast(new Point() { x = 82, y = 206 }); pointList.AddLast(new Point() { x = 80, y = 209 }); pointList.AddLast(new Point() { x = 75, y = 213 }); pointList.AddLast(new Point() { x = 73, y = 213 }); pointList.AddLast(new Point() { x = 70, y = 216 }); pointList.AddLast(new Point() { x = 67, y = 219 }); pointList.AddLast(new Point() { x = 64, y = 221 }); pointList.AddLast(new Point() { x = 61, y = 223 }); pointList.AddLast(new Point() { x = 60, y = 225 }); pointList.AddLast(new Point() { x = 62, y = 226 }); pointList.AddLast(new Point() { x = 65, y = 225 }); pointList.AddLast(new Point() { x = 67, y = 226 }); pointList.AddLast(new Point() { x = 74, y = 226 }); pointList.AddLast(new Point() { x = 77, y = 227 }); pointList.AddLast(new Point() { x = 85, y = 229 }); pointList.AddLast(new Point() { x = 91, y = 230 }); pointList.AddLast(new Point() { x = 99, y = 231 }); pointList.AddLast(new Point() { x = 108, y = 232 }); pointList.AddLast(new Point() { x = 116, y = 233 }); pointList.AddLast(new Point() { x = 125, y = 233 }); pointList.AddLast(new Point() { x = 134, y = 234 }); pointList.AddLast(new Point() { x = 145, y = 233 }); pointList.AddLast(new Point() { x = 153, y = 232 }); pointList.AddLast(new Point() { x = 160, y = 233 }); pointList.AddLast(new Point() { x = 170, y = 234 }); pointList.AddLast(new Point() { x = 177, y = 235 }); pointList.AddLast(new Point() { x = 179, y = 236 }); pointList.AddLast(new Point() { x = 186, y = 237 }); pointList.AddLast(new Point() { x = 193, y = 238 }); pointList.AddLast(new Point() { x = 198, y = 239 }); pointList.AddLast(new Point() { x = 200, y = 237 }); pointList.AddLast(new Point() { x = 202, y = 239 }); pointList.AddLast(new Point() { x = 204, y = 238 }); pointList.AddLast(new Point() { x = 206, y = 234 }); pointList.AddLast(new Point() { x = 205, y = 230 }); pointList.AddLast(new Point() { x = 202, y = 222 }); pointList.AddLast(new Point() { x = 197, y = 216 }); pointList.AddLast(new Point() { x = 192, y = 207 }); pointList.AddLast(new Point() { x = 186, y = 198 }); pointList.AddLast(new Point() { x = 179, y = 189 }); pointList.AddLast(new Point() { x = 174, y = 183 }); pointList.AddLast(new Point() { x = 170, y = 178 }); pointList.AddLast(new Point() { x = 164, y = 171 }); pointList.AddLast(new Point() { x = 161, y = 168 }); pointList.AddLast(new Point() { x = 154, y = 160 }); pointList.AddLast(new Point() { x = 148, y = 155 }); pointList.AddLast(new Point() { x = 143, y = 150 }); pointList.AddLast(new Point() { x = 138, y = 148 }); pointList.AddLast(new Point() { x = 136, y = 148 });
        list.Add(new Unistroke("triangle", pointList));
        #endregion
        
        #region x
        pointList = new LinkedList<Point>();
        pointList.AddLast(new Point() { x = 87, y = 142 }); pointList.AddLast(new Point() { x = 89, y = 145 }); pointList.AddLast(new Point() { x = 91, y = 148 }); pointList.AddLast(new Point() { x = 93, y = 151 }); pointList.AddLast(new Point() { x = 96, y = 155 }); pointList.AddLast(new Point() { x = 98, y = 157 }); pointList.AddLast(new Point() { x = 100, y = 160 }); pointList.AddLast(new Point() { x = 102, y = 162 }); pointList.AddLast(new Point() { x = 106, y = 167 }); pointList.AddLast(new Point() { x = 108, y = 169 }); pointList.AddLast(new Point() { x = 110, y = 171 }); pointList.AddLast(new Point() { x = 115, y = 177 }); pointList.AddLast(new Point() { x = 119, y = 183 }); pointList.AddLast(new Point() { x = 123, y = 189 }); pointList.AddLast(new Point() { x = 127, y = 193 }); pointList.AddLast(new Point() { x = 129, y = 196 }); pointList.AddLast(new Point() { x = 133, y = 200 }); pointList.AddLast(new Point() { x = 137, y = 206 }); pointList.AddLast(new Point() { x = 140, y = 209 }); pointList.AddLast(new Point() { x = 143, y = 212 }); pointList.AddLast(new Point() { x = 146, y = 215 }); pointList.AddLast(new Point() { x = 151, y = 220 }); pointList.AddLast(new Point() { x = 153, y = 222 }); pointList.AddLast(new Point() { x = 155, y = 223 }); pointList.AddLast(new Point() { x = 157, y = 225 }); pointList.AddLast(new Point() { x = 158, y = 223 }); pointList.AddLast(new Point() { x = 157, y = 218 }); pointList.AddLast(new Point() { x = 155, y = 211 }); pointList.AddLast(new Point() { x = 154, y = 208 }); pointList.AddLast(new Point() { x = 152, y = 200 }); pointList.AddLast(new Point() { x = 150, y = 189 }); pointList.AddLast(new Point() { x = 148, y = 179 }); pointList.AddLast(new Point() { x = 147, y = 170 }); pointList.AddLast(new Point() { x = 147, y = 158 }); pointList.AddLast(new Point() { x = 147, y = 148 }); pointList.AddLast(new Point() { x = 147, y = 141 }); pointList.AddLast(new Point() { x = 147, y = 136 }); pointList.AddLast(new Point() { x = 144, y = 135 }); pointList.AddLast(new Point() { x = 142, y = 137 }); pointList.AddLast(new Point() { x = 140, y = 139 }); pointList.AddLast(new Point() { x = 135, y = 145 }); pointList.AddLast(new Point() { x = 131, y = 152 }); pointList.AddLast(new Point() { x = 124, y = 163 }); pointList.AddLast(new Point() { x = 116, y = 177 }); pointList.AddLast(new Point() { x = 108, y = 191 }); pointList.AddLast(new Point() { x = 100, y = 206 }); pointList.AddLast(new Point() { x = 94, y = 217 }); pointList.AddLast(new Point() { x = 91, y = 222 }); pointList.AddLast(new Point() { x = 89, y = 225 }); pointList.AddLast(new Point() { x = 87, y = 226 }); pointList.AddLast(new Point() { x = 87, y = 224 });
        list.Add(new Unistroke("x", pointList));
        #endregion
        #region rectangle
        pointList = new LinkedList<Point>();
        pointList.AddLast(new Point() { x = 78, y = 149 }); pointList.AddLast(new Point() { x = 78, y = 153 }); pointList.AddLast(new Point() { x = 78, y = 157 }); pointList.AddLast(new Point() { x = 78, y = 160 }); pointList.AddLast(new Point() { x = 79, y = 162 }); pointList.AddLast(new Point() { x = 79, y = 164 }); pointList.AddLast(new Point() { x = 79, y = 167 }); pointList.AddLast(new Point() { x = 79, y = 169 }); pointList.AddLast(new Point() { x = 79, y = 173 }); pointList.AddLast(new Point() { x = 79, y = 178 }); pointList.AddLast(new Point() { x = 79, y = 183 }); pointList.AddLast(new Point() { x = 80, y = 189 }); pointList.AddLast(new Point() { x = 80, y = 193 }); pointList.AddLast(new Point() { x = 80, y = 198 }); pointList.AddLast(new Point() { x = 80, y = 202 }); pointList.AddLast(new Point() { x = 81, y = 208 }); pointList.AddLast(new Point() { x = 81, y = 210 }); pointList.AddLast(new Point() { x = 81, y = 216 }); pointList.AddLast(new Point() { x = 82, y = 222 }); pointList.AddLast(new Point() { x = 82, y = 224 }); pointList.AddLast(new Point() { x = 82, y = 227 }); pointList.AddLast(new Point() { x = 83, y = 229 }); pointList.AddLast(new Point() { x = 83, y = 231 }); pointList.AddLast(new Point() { x = 85, y = 230 }); pointList.AddLast(new Point() { x = 88, y = 232 }); pointList.AddLast(new Point() { x = 90, y = 233 }); pointList.AddLast(new Point() { x = 92, y = 232 }); pointList.AddLast(new Point() { x = 94, y = 233 }); pointList.AddLast(new Point() { x = 99, y = 232 }); pointList.AddLast(new Point() { x = 102, y = 233 }); pointList.AddLast(new Point() { x = 106, y = 233 }); pointList.AddLast(new Point() { x = 109, y = 234 }); pointList.AddLast(new Point() { x = 117, y = 235 }); pointList.AddLast(new Point() { x = 123, y = 236 }); pointList.AddLast(new Point() { x = 126, y = 236 }); pointList.AddLast(new Point() { x = 135, y = 237 }); pointList.AddLast(new Point() { x = 142, y = 238 }); pointList.AddLast(new Point() { x = 145, y = 238 }); pointList.AddLast(new Point() { x = 152, y = 238 }); pointList.AddLast(new Point() { x = 154, y = 239 }); pointList.AddLast(new Point() { x = 165, y = 238 }); pointList.AddLast(new Point() { x = 174, y = 237 }); pointList.AddLast(new Point() { x = 179, y = 236 }); pointList.AddLast(new Point() { x = 186, y = 235 }); pointList.AddLast(new Point() { x = 191, y = 235 }); pointList.AddLast(new Point() { x = 195, y = 233 }); pointList.AddLast(new Point() { x = 197, y = 233 }); pointList.AddLast(new Point() { x = 200, y = 233 }); pointList.AddLast(new Point() { x = 201, y = 235 }); pointList.AddLast(new Point() { x = 201, y = 233 }); pointList.AddLast(new Point() { x = 199, y = 231 }); pointList.AddLast(new Point() { x = 198, y = 226 }); pointList.AddLast(new Point() { x = 198, y = 220 }); pointList.AddLast(new Point() { x = 196, y = 207 }); pointList.AddLast(new Point() { x = 195, y = 195 }); pointList.AddLast(new Point() { x = 195, y = 181 }); pointList.AddLast(new Point() { x = 195, y = 173 }); pointList.AddLast(new Point() { x = 195, y = 163 }); pointList.AddLast(new Point() { x = 194, y = 155 }); pointList.AddLast(new Point() { x = 192, y = 145 }); pointList.AddLast(new Point() { x = 192, y = 143 }); pointList.AddLast(new Point() { x = 192, y = 138 }); pointList.AddLast(new Point() { x = 191, y = 135 }); pointList.AddLast(new Point() { x = 191, y = 133 }); pointList.AddLast(new Point() { x = 191, y = 130 }); pointList.AddLast(new Point() { x = 190, y = 128 }); pointList.AddLast(new Point() { x = 188, y = 129 }); pointList.AddLast(new Point() { x = 186, y = 129 }); pointList.AddLast(new Point() { x = 181, y = 132 }); pointList.AddLast(new Point() { x = 173, y = 131 }); pointList.AddLast(new Point() { x = 162, y = 131 }); pointList.AddLast(new Point() { x = 151, y = 132 }); pointList.AddLast(new Point() { x = 149, y = 132 }); pointList.AddLast(new Point() { x = 138, y = 132 }); pointList.AddLast(new Point() { x = 136, y = 132 }); pointList.AddLast(new Point() { x = 122, y = 131 }); pointList.AddLast(new Point() { x = 120, y = 131 }); pointList.AddLast(new Point() { x = 109, y = 130 }); pointList.AddLast(new Point() { x = 107, y = 130 }); pointList.AddLast(new Point() { x = 90, y = 132 }); pointList.AddLast(new Point() { x = 81, y = 133 }); pointList.AddLast(new Point() { x = 76, y = 133 });
        list.Add(new Unistroke("rectangle", pointList));
        #endregion
        #region circle
        pointList = new LinkedList<Point>();
        pointList.AddLast(new Point() { x = 127, y = 141 }); pointList.AddLast(new Point() { x = 124, y = 140 }); pointList.AddLast(new Point() { x = 120, y = 139 }); pointList.AddLast(new Point() { x = 118, y = 139 }); pointList.AddLast(new Point() { x = 116, y = 139 }); pointList.AddLast(new Point() { x = 111, y = 140 }); pointList.AddLast(new Point() { x = 109, y = 141 }); pointList.AddLast(new Point() { x = 104, y = 144 }); pointList.AddLast(new Point() { x = 100, y = 147 }); pointList.AddLast(new Point() { x = 96, y = 152 }); pointList.AddLast(new Point() { x = 93, y = 157 }); pointList.AddLast(new Point() { x = 90, y = 163 }); pointList.AddLast(new Point() { x = 87, y = 169 }); pointList.AddLast(new Point() { x = 85, y = 175 }); pointList.AddLast(new Point() { x = 83, y = 181 }); pointList.AddLast(new Point() { x = 82, y = 190 }); pointList.AddLast(new Point() { x = 82, y = 195 }); pointList.AddLast(new Point() { x = 83, y = 200 }); pointList.AddLast(new Point() { x = 84, y = 205 }); pointList.AddLast(new Point() { x = 88, y = 213 }); pointList.AddLast(new Point() { x = 91, y = 216 }); pointList.AddLast(new Point() { x = 96, y = 219 }); pointList.AddLast(new Point() { x = 103, y = 222 }); pointList.AddLast(new Point() { x = 108, y = 224 }); pointList.AddLast(new Point() { x = 111, y = 224 }); pointList.AddLast(new Point() { x = 120, y = 224 }); pointList.AddLast(new Point() { x = 133, y = 223 }); pointList.AddLast(new Point() { x = 142, y = 222 }); pointList.AddLast(new Point() { x = 152, y = 218 }); pointList.AddLast(new Point() { x = 160, y = 214 }); pointList.AddLast(new Point() { x = 167, y = 210 }); pointList.AddLast(new Point() { x = 173, y = 204 }); pointList.AddLast(new Point() { x = 178, y = 198 }); pointList.AddLast(new Point() { x = 179, y = 196 }); pointList.AddLast(new Point() { x = 182, y = 188 }); pointList.AddLast(new Point() { x = 182, y = 177 }); pointList.AddLast(new Point() { x = 178, y = 167 }); pointList.AddLast(new Point() { x = 170, y = 150 }); pointList.AddLast(new Point() { x = 163, y = 138 }); pointList.AddLast(new Point() { x = 152, y = 130 }); pointList.AddLast(new Point() { x = 143, y = 129 }); pointList.AddLast(new Point() { x = 140, y = 131 }); pointList.AddLast(new Point() { x = 129, y = 136 }); pointList.AddLast(new Point() { x = 126, y = 139 });
        list.Add(new Unistroke("circle", pointList));
        #endregion
        #region check
        pointList = new LinkedList<Point>();
        pointList.AddLast(new Point() { x = 91, y = 185 }); pointList.AddLast(new Point() { x = 93, y = 185 }); pointList.AddLast(new Point() { x = 95, y = 185 }); pointList.AddLast(new Point() { x = 97, y = 185 }); pointList.AddLast(new Point() { x = 100, y = 188 }); pointList.AddLast(new Point() { x = 102, y = 189 }); pointList.AddLast(new Point() { x = 104, y = 190 }); pointList.AddLast(new Point() { x = 106, y = 193 }); pointList.AddLast(new Point() { x = 108, y = 195 }); pointList.AddLast(new Point() { x = 110, y = 198 }); pointList.AddLast(new Point() { x = 112, y = 201 }); pointList.AddLast(new Point() { x = 114, y = 204 }); pointList.AddLast(new Point() { x = 115, y = 207 }); pointList.AddLast(new Point() { x = 117, y = 210 }); pointList.AddLast(new Point() { x = 118, y = 212 }); pointList.AddLast(new Point() { x = 120, y = 214 }); pointList.AddLast(new Point() { x = 121, y = 217 }); pointList.AddLast(new Point() { x = 122, y = 219 }); pointList.AddLast(new Point() { x = 123, y = 222 }); pointList.AddLast(new Point() { x = 124, y = 224 }); pointList.AddLast(new Point() { x = 126, y = 226 }); pointList.AddLast(new Point() { x = 127, y = 229 }); pointList.AddLast(new Point() { x = 129, y = 231 }); pointList.AddLast(new Point() { x = 130, y = 233 }); pointList.AddLast(new Point() { x = 129, y = 231 }); pointList.AddLast(new Point() { x = 129, y = 228 }); pointList.AddLast(new Point() { x = 129, y = 226 }); pointList.AddLast(new Point() { x = 129, y = 224 }); pointList.AddLast(new Point() { x = 129, y = 221 }); pointList.AddLast(new Point() { x = 129, y = 218 }); pointList.AddLast(new Point() { x = 129, y = 212 }); pointList.AddLast(new Point() { x = 129, y = 208 }); pointList.AddLast(new Point() { x = 130, y = 198 }); pointList.AddLast(new Point() { x = 132, y = 189 }); pointList.AddLast(new Point() { x = 134, y = 182 }); pointList.AddLast(new Point() { x = 137, y = 173 }); pointList.AddLast(new Point() { x = 143, y = 164 }); pointList.AddLast(new Point() { x = 147, y = 157 }); pointList.AddLast(new Point() { x = 151, y = 151 }); pointList.AddLast(new Point() { x = 155, y = 144 }); pointList.AddLast(new Point() { x = 161, y = 137 }); pointList.AddLast(new Point() { x = 165, y = 131 }); pointList.AddLast(new Point() { x = 171, y = 122 }); pointList.AddLast(new Point() { x = 174, y = 118 }); pointList.AddLast(new Point() { x = 176, y = 114 }); pointList.AddLast(new Point() { x = 177, y = 112 }); pointList.AddLast(new Point() { x = 177, y = 114 }); pointList.AddLast(new Point() { x = 175, y = 116 }); pointList.AddLast(new Point() { x = 173, y = 118 });
        list.Add(new Unistroke("check", pointList));
        #endregion
        #region caret
        pointList = new LinkedList<Point>();
        pointList.AddLast(new Point() { x = 79, y = 245 }); pointList.AddLast(new Point() { x = 79, y = 242 }); pointList.AddLast(new Point() { x = 79, y = 239 }); pointList.AddLast(new Point() { x = 80, y = 237 }); pointList.AddLast(new Point() { x = 80, y = 234 }); pointList.AddLast(new Point() { x = 81, y = 232 }); pointList.AddLast(new Point() { x = 82, y = 230 }); pointList.AddLast(new Point() { x = 84, y = 224 }); pointList.AddLast(new Point() { x = 86, y = 220 }); pointList.AddLast(new Point() { x = 86, y = 218 }); pointList.AddLast(new Point() { x = 87, y = 216 }); pointList.AddLast(new Point() { x = 88, y = 213 }); pointList.AddLast(new Point() { x = 90, y = 207 }); pointList.AddLast(new Point() { x = 91, y = 202 }); pointList.AddLast(new Point() { x = 92, y = 200 }); pointList.AddLast(new Point() { x = 93, y = 194 }); pointList.AddLast(new Point() { x = 94, y = 192 }); pointList.AddLast(new Point() { x = 96, y = 189 }); pointList.AddLast(new Point() { x = 97, y = 186 }); pointList.AddLast(new Point() { x = 100, y = 179 }); pointList.AddLast(new Point() { x = 102, y = 173 }); pointList.AddLast(new Point() { x = 105, y = 165 }); pointList.AddLast(new Point() { x = 107, y = 160 }); pointList.AddLast(new Point() { x = 109, y = 158 }); pointList.AddLast(new Point() { x = 112, y = 151 }); pointList.AddLast(new Point() { x = 115, y = 144 }); pointList.AddLast(new Point() { x = 117, y = 139 }); pointList.AddLast(new Point() { x = 119, y = 136 }); pointList.AddLast(new Point() { x = 119, y = 134 }); pointList.AddLast(new Point() { x = 120, y = 132 }); pointList.AddLast(new Point() { x = 121, y = 129 }); pointList.AddLast(new Point() { x = 122, y = 127 }); pointList.AddLast(new Point() { x = 124, y = 125 }); pointList.AddLast(new Point() { x = 126, y = 124 }); pointList.AddLast(new Point() { x = 129, y = 125 }); pointList.AddLast(new Point() { x = 131, y = 127 }); pointList.AddLast(new Point() { x = 132, y = 130 }); pointList.AddLast(new Point() { x = 136, y = 139 }); pointList.AddLast(new Point() { x = 141, y = 154 }); pointList.AddLast(new Point() { x = 145, y = 166 }); pointList.AddLast(new Point() { x = 151, y = 182 }); pointList.AddLast(new Point() { x = 156, y = 193 }); pointList.AddLast(new Point() { x = 157, y = 196 }); pointList.AddLast(new Point() { x = 161, y = 209 }); pointList.AddLast(new Point() { x = 162, y = 211 }); pointList.AddLast(new Point() { x = 167, y = 223 }); pointList.AddLast(new Point() { x = 169, y = 229 }); pointList.AddLast(new Point() { x = 170, y = 231 }); pointList.AddLast(new Point() { x = 173, y = 237 }); pointList.AddLast(new Point() { x = 176, y = 242 }); pointList.AddLast(new Point() { x = 177, y = 244 }); pointList.AddLast(new Point() { x = 179, y = 250 }); pointList.AddLast(new Point() { x = 181, y = 255 }); pointList.AddLast(new Point() { x = 182, y = 257 });
        list.Add(new Unistroke("caret", pointList));
        #endregion
        #region zigzag
        pointList = new LinkedList<Point>();
        pointList.AddLast(new Point() { x = 307, y = 216 }); pointList.AddLast(new Point() { x = 333, y = 186 }); pointList.AddLast(new Point() { x = 356, y = 215 }); pointList.AddLast(new Point() { x = 375, y = 186 }); pointList.AddLast(new Point() { x = 399, y = 216 }); pointList.AddLast(new Point() { x = 418, y = 186 });
        list.Add(new Unistroke("zig-zag", pointList));
        #endregion
        #region arrow
        pointList = new LinkedList<Point>();
        pointList.AddLast(new Point() { x = 68, y = 222 }); pointList.AddLast(new Point() { x = 70, y = 220 }); pointList.AddLast(new Point() { x = 73, y = 218 }); pointList.AddLast(new Point() { x = 75, y = 217 }); pointList.AddLast(new Point() { x = 77, y = 215 }); pointList.AddLast(new Point() { x = 80, y = 213 }); pointList.AddLast(new Point() { x = 82, y = 212 }); pointList.AddLast(new Point() { x = 84, y = 210 }); pointList.AddLast(new Point() { x = 87, y = 209 }); pointList.AddLast(new Point() { x = 89, y = 208 }); pointList.AddLast(new Point() { x = 92, y = 206 }); pointList.AddLast(new Point() { x = 95, y = 204 }); pointList.AddLast(new Point() { x = 101, y = 201 }); pointList.AddLast(new Point() { x = 106, y = 198 }); pointList.AddLast(new Point() { x = 112, y = 194 }); pointList.AddLast(new Point() { x = 118, y = 191 }); pointList.AddLast(new Point() { x = 124, y = 187 }); pointList.AddLast(new Point() { x = 127, y = 186 }); pointList.AddLast(new Point() { x = 132, y = 183 }); pointList.AddLast(new Point() { x = 138, y = 181 }); pointList.AddLast(new Point() { x = 141, y = 180 }); pointList.AddLast(new Point() { x = 146, y = 178 }); pointList.AddLast(new Point() { x = 154, y = 173 }); pointList.AddLast(new Point() { x = 159, y = 171 }); pointList.AddLast(new Point() { x = 161, y = 170 }); pointList.AddLast(new Point() { x = 166, y = 167 }); pointList.AddLast(new Point() { x = 168, y = 167 }); pointList.AddLast(new Point() { x = 171, y = 166 }); pointList.AddLast(new Point() { x = 174, y = 164 }); pointList.AddLast(new Point() { x = 177, y = 162 }); pointList.AddLast(new Point() { x = 180, y = 160 }); pointList.AddLast(new Point() { x = 182, y = 158 }); pointList.AddLast(new Point() { x = 183, y = 156 }); pointList.AddLast(new Point() { x = 181, y = 154 }); pointList.AddLast(new Point() { x = 178, y = 153 }); pointList.AddLast(new Point() { x = 171, y = 153 }); pointList.AddLast(new Point() { x = 164, y = 153 }); pointList.AddLast(new Point() { x = 160, y = 153 }); pointList.AddLast(new Point() { x = 150, y = 154 }); pointList.AddLast(new Point() { x = 147, y = 155 }); pointList.AddLast(new Point() { x = 141, y = 157 }); pointList.AddLast(new Point() { x = 137, y = 158 }); pointList.AddLast(new Point() { x = 135, y = 158 }); pointList.AddLast(new Point() { x = 137, y = 158 }); pointList.AddLast(new Point() { x = 140, y = 157 }); pointList.AddLast(new Point() { x = 143, y = 156 }); pointList.AddLast(new Point() { x = 151, y = 154 }); pointList.AddLast(new Point() { x = 160, y = 152 }); pointList.AddLast(new Point() { x = 170, y = 149 }); pointList.AddLast(new Point() { x = 179, y = 147 }); pointList.AddLast(new Point() { x = 185, y = 145 }); pointList.AddLast(new Point() { x = 192, y = 144 }); pointList.AddLast(new Point() { x = 196, y = 144 }); pointList.AddLast(new Point() { x = 198, y = 144 }); pointList.AddLast(new Point() { x = 200, y = 144 }); pointList.AddLast(new Point() { x = 201, y = 147 }); pointList.AddLast(new Point() { x = 199, y = 149 }); pointList.AddLast(new Point() { x = 194, y = 157 }); pointList.AddLast(new Point() { x = 191, y = 160 }); pointList.AddLast(new Point() { x = 186, y = 167 }); pointList.AddLast(new Point() { x = 180, y = 176 }); pointList.AddLast(new Point() { x = 177, y = 179 }); pointList.AddLast(new Point() { x = 171, y = 187 }); pointList.AddLast(new Point() { x = 169, y = 189 }); pointList.AddLast(new Point() { x = 165, y = 194 }); pointList.AddLast(new Point() { x = 164, y = 196 });
        list.Add(new Unistroke("arrow", pointList));
        #endregion
        #region left square bracket
        pointList = new LinkedList<Point>();
        pointList.AddLast(new Point() { x = 140, y = 124 }); pointList.AddLast(new Point() { x = 138, y = 123 }); pointList.AddLast(new Point() { x = 135, y = 122 }); pointList.AddLast(new Point() { x = 133, y = 123 }); pointList.AddLast(new Point() { x = 130, y = 123 }); pointList.AddLast(new Point() { x = 128, y = 124 }); pointList.AddLast(new Point() { x = 125, y = 125 }); pointList.AddLast(new Point() { x = 122, y = 124 }); pointList.AddLast(new Point() { x = 120, y = 124 }); pointList.AddLast(new Point() { x = 118, y = 124 }); pointList.AddLast(new Point() { x = 116, y = 125 }); pointList.AddLast(new Point() { x = 113, y = 125 }); pointList.AddLast(new Point() { x = 111, y = 125 }); pointList.AddLast(new Point() { x = 108, y = 124 }); pointList.AddLast(new Point() { x = 106, y = 125 }); pointList.AddLast(new Point() { x = 104, y = 125 }); pointList.AddLast(new Point() { x = 102, y = 124 }); pointList.AddLast(new Point() { x = 100, y = 123 }); pointList.AddLast(new Point() { x = 98, y = 123 }); pointList.AddLast(new Point() { x = 95, y = 124 }); pointList.AddLast(new Point() { x = 93, y = 123 }); pointList.AddLast(new Point() { x = 90, y = 124 }); pointList.AddLast(new Point() { x = 88, y = 124 }); pointList.AddLast(new Point() { x = 85, y = 125 }); pointList.AddLast(new Point() { x = 83, y = 126 }); pointList.AddLast(new Point() { x = 81, y = 127 }); pointList.AddLast(new Point() { x = 81, y = 129 }); pointList.AddLast(new Point() { x = 82, y = 131 }); pointList.AddLast(new Point() { x = 82, y = 134 }); pointList.AddLast(new Point() { x = 83, y = 138 }); pointList.AddLast(new Point() { x = 84, y = 141 }); pointList.AddLast(new Point() { x = 84, y = 144 }); pointList.AddLast(new Point() { x = 85, y = 148 }); pointList.AddLast(new Point() { x = 85, y = 151 }); pointList.AddLast(new Point() { x = 86, y = 156 }); pointList.AddLast(new Point() { x = 86, y = 160 }); pointList.AddLast(new Point() { x = 86, y = 164 }); pointList.AddLast(new Point() { x = 86, y = 168 }); pointList.AddLast(new Point() { x = 87, y = 171 }); pointList.AddLast(new Point() { x = 87, y = 175 }); pointList.AddLast(new Point() { x = 87, y = 179 }); pointList.AddLast(new Point() { x = 87, y = 182 }); pointList.AddLast(new Point() { x = 87, y = 186 }); pointList.AddLast(new Point() { x = 88, y = 188 }); pointList.AddLast(new Point() { x = 88, y = 195 }); pointList.AddLast(new Point() { x = 88, y = 198 }); pointList.AddLast(new Point() { x = 88, y = 201 }); pointList.AddLast(new Point() { x = 88, y = 207 }); pointList.AddLast(new Point() { x = 89, y = 211 }); pointList.AddLast(new Point() { x = 89, y = 213 }); pointList.AddLast(new Point() { x = 89, y = 217 }); pointList.AddLast(new Point() { x = 89, y = 222 }); pointList.AddLast(new Point() { x = 88, y = 225 }); pointList.AddLast(new Point() { x = 88, y = 229 }); pointList.AddLast(new Point() { x = 88, y = 231 }); pointList.AddLast(new Point() { x = 88, y = 233 }); pointList.AddLast(new Point() { x = 88, y = 235 }); pointList.AddLast(new Point() { x = 89, y = 237 }); pointList.AddLast(new Point() { x = 89, y = 240 }); pointList.AddLast(new Point() { x = 89, y = 242 }); pointList.AddLast(new Point() { x = 91, y = 241 }); pointList.AddLast(new Point() { x = 94, y = 241 }); pointList.AddLast(new Point() { x = 96, y = 240 }); pointList.AddLast(new Point() { x = 98, y = 239 }); pointList.AddLast(new Point() { x = 105, y = 240 }); pointList.AddLast(new Point() { x = 109, y = 240 }); pointList.AddLast(new Point() { x = 113, y = 239 }); pointList.AddLast(new Point() { x = 116, y = 240 }); pointList.AddLast(new Point() { x = 121, y = 239 }); pointList.AddLast(new Point() { x = 130, y = 240 }); pointList.AddLast(new Point() { x = 136, y = 237 }); pointList.AddLast(new Point() { x = 139, y = 237 }); pointList.AddLast(new Point() { x = 144, y = 238 }); pointList.AddLast(new Point() { x = 151, y = 237 }); pointList.AddLast(new Point() { x = 157, y = 236 }); pointList.AddLast(new Point() { x = 159, y = 237 });
        list.Add(new Unistroke("left square bracket", pointList));
        #endregion
        #region right square bracket
        pointList = new LinkedList<Point>();
        pointList.AddLast(new Point() { x = 112, y = 138 }); pointList.AddLast(new Point() { x = 112, y = 136 }); pointList.AddLast(new Point() { x = 115, y = 136 }); pointList.AddLast(new Point() { x = 118, y = 137 }); pointList.AddLast(new Point() { x = 120, y = 136 }); pointList.AddLast(new Point() { x = 123, y = 136 }); pointList.AddLast(new Point() { x = 125, y = 136 }); pointList.AddLast(new Point() { x = 128, y = 136 }); pointList.AddLast(new Point() { x = 131, y = 136 }); pointList.AddLast(new Point() { x = 134, y = 135 }); pointList.AddLast(new Point() { x = 137, y = 135 }); pointList.AddLast(new Point() { x = 140, y = 134 }); pointList.AddLast(new Point() { x = 143, y = 133 }); pointList.AddLast(new Point() { x = 145, y = 132 }); pointList.AddLast(new Point() { x = 147, y = 132 }); pointList.AddLast(new Point() { x = 149, y = 132 }); pointList.AddLast(new Point() { x = 152, y = 132 }); pointList.AddLast(new Point() { x = 153, y = 134 }); pointList.AddLast(new Point() { x = 154, y = 137 }); pointList.AddLast(new Point() { x = 155, y = 141 }); pointList.AddLast(new Point() { x = 156, y = 144 }); pointList.AddLast(new Point() { x = 157, y = 152 }); pointList.AddLast(new Point() { x = 158, y = 161 }); pointList.AddLast(new Point() { x = 160, y = 170 }); pointList.AddLast(new Point() { x = 162, y = 182 }); pointList.AddLast(new Point() { x = 164, y = 192 }); pointList.AddLast(new Point() { x = 166, y = 200 }); pointList.AddLast(new Point() { x = 167, y = 209 }); pointList.AddLast(new Point() { x = 168, y = 214 }); pointList.AddLast(new Point() { x = 168, y = 216 }); pointList.AddLast(new Point() { x = 169, y = 221 }); pointList.AddLast(new Point() { x = 169, y = 223 }); pointList.AddLast(new Point() { x = 169, y = 228 }); pointList.AddLast(new Point() { x = 169, y = 231 }); pointList.AddLast(new Point() { x = 166, y = 233 }); pointList.AddLast(new Point() { x = 164, y = 234 }); pointList.AddLast(new Point() { x = 161, y = 235 }); pointList.AddLast(new Point() { x = 155, y = 236 }); pointList.AddLast(new Point() { x = 147, y = 235 }); pointList.AddLast(new Point() { x = 140, y = 233 }); pointList.AddLast(new Point() { x = 131, y = 233 }); pointList.AddLast(new Point() { x = 124, y = 233 }); pointList.AddLast(new Point() { x = 117, y = 235 }); pointList.AddLast(new Point() { x = 114, y = 238 }); pointList.AddLast(new Point() { x = 112, y = 238 });
        list.Add(new Unistroke("left square bracket", pointList));
        #endregion
        #region v
        pointList = new LinkedList<Point>();
        pointList.AddLast(new Point() { x = 89, y = 164 }); pointList.AddLast(new Point() { x = 90, y = 162 }); pointList.AddLast(new Point() { x = 92, y = 162 }); pointList.AddLast(new Point() { x = 94, y = 164 }); pointList.AddLast(new Point() { x = 95, y = 166 }); pointList.AddLast(new Point() { x = 96, y = 169 }); pointList.AddLast(new Point() { x = 97, y = 171 }); pointList.AddLast(new Point() { x = 99, y = 175 }); pointList.AddLast(new Point() { x = 101, y = 178 }); pointList.AddLast(new Point() { x = 103, y = 182 }); pointList.AddLast(new Point() { x = 106, y = 189 }); pointList.AddLast(new Point() { x = 108, y = 194 }); pointList.AddLast(new Point() { x = 111, y = 199 }); pointList.AddLast(new Point() { x = 114, y = 204 }); pointList.AddLast(new Point() { x = 117, y = 209 }); pointList.AddLast(new Point() { x = 119, y = 214 }); pointList.AddLast(new Point() { x = 122, y = 218 }); pointList.AddLast(new Point() { x = 124, y = 222 }); pointList.AddLast(new Point() { x = 126, y = 225 }); pointList.AddLast(new Point() { x = 128, y = 228 }); pointList.AddLast(new Point() { x = 130, y = 229 }); pointList.AddLast(new Point() { x = 133, y = 233 }); pointList.AddLast(new Point() { x = 134, y = 236 }); pointList.AddLast(new Point() { x = 136, y = 239 }); pointList.AddLast(new Point() { x = 138, y = 240 }); pointList.AddLast(new Point() { x = 139, y = 242 }); pointList.AddLast(new Point() { x = 140, y = 244 }); pointList.AddLast(new Point() { x = 142, y = 242 }); pointList.AddLast(new Point() { x = 142, y = 240 }); pointList.AddLast(new Point() { x = 142, y = 237 }); pointList.AddLast(new Point() { x = 143, y = 235 }); pointList.AddLast(new Point() { x = 143, y = 233 }); pointList.AddLast(new Point() { x = 145, y = 229 }); pointList.AddLast(new Point() { x = 146, y = 226 }); pointList.AddLast(new Point() { x = 148, y = 217 }); pointList.AddLast(new Point() { x = 149, y = 208 }); pointList.AddLast(new Point() { x = 149, y = 205 }); pointList.AddLast(new Point() { x = 151, y = 196 }); pointList.AddLast(new Point() { x = 151, y = 193 }); pointList.AddLast(new Point() { x = 153, y = 182 }); pointList.AddLast(new Point() { x = 155, y = 172 }); pointList.AddLast(new Point() { x = 157, y = 165 }); pointList.AddLast(new Point() { x = 159, y = 160 }); pointList.AddLast(new Point() { x = 162, y = 155 }); pointList.AddLast(new Point() { x = 164, y = 150 }); pointList.AddLast(new Point() { x = 165, y = 148 }); pointList.AddLast(new Point() { x = 166, y = 146 });
        list.Add(new Unistroke("v", pointList));
        #endregion
        #region delete
        pointList = new LinkedList<Point>();
        pointList.AddLast(new Point() { x = 123, y = 129 }); pointList.AddLast(new Point() { x = 123, y = 131 }); pointList.AddLast(new Point() { x = 124, y = 133 }); pointList.AddLast(new Point() { x = 125, y = 136 }); pointList.AddLast(new Point() { x = 127, y = 140 }); pointList.AddLast(new Point() { x = 129, y = 142 }); pointList.AddLast(new Point() { x = 133, y = 148 }); pointList.AddLast(new Point() { x = 137, y = 154 }); pointList.AddLast(new Point() { x = 143, y = 158 }); pointList.AddLast(new Point() { x = 145, y = 161 }); pointList.AddLast(new Point() { x = 148, y = 164 }); pointList.AddLast(new Point() { x = 153, y = 170 }); pointList.AddLast(new Point() { x = 158, y = 176 }); pointList.AddLast(new Point() { x = 160, y = 178 }); pointList.AddLast(new Point() { x = 164, y = 183 }); pointList.AddLast(new Point() { x = 168, y = 188 }); pointList.AddLast(new Point() { x = 171, y = 191 }); pointList.AddLast(new Point() { x = 175, y = 196 }); pointList.AddLast(new Point() { x = 178, y = 200 }); pointList.AddLast(new Point() { x = 180, y = 202 }); pointList.AddLast(new Point() { x = 181, y = 205 }); pointList.AddLast(new Point() { x = 184, y = 208 }); pointList.AddLast(new Point() { x = 186, y = 210 }); pointList.AddLast(new Point() { x = 187, y = 213 }); pointList.AddLast(new Point() { x = 188, y = 215 }); pointList.AddLast(new Point() { x = 186, y = 212 }); pointList.AddLast(new Point() { x = 183, y = 211 }); pointList.AddLast(new Point() { x = 177, y = 208 }); pointList.AddLast(new Point() { x = 169, y = 206 }); pointList.AddLast(new Point() { x = 162, y = 205 }); pointList.AddLast(new Point() { x = 154, y = 207 }); pointList.AddLast(new Point() { x = 145, y = 209 }); pointList.AddLast(new Point() { x = 137, y = 210 }); pointList.AddLast(new Point() { x = 129, y = 214 }); pointList.AddLast(new Point() { x = 122, y = 217 }); pointList.AddLast(new Point() { x = 118, y = 218 }); pointList.AddLast(new Point() { x = 111, y = 221 }); pointList.AddLast(new Point() { x = 109, y = 222 }); pointList.AddLast(new Point() { x = 110, y = 219 }); pointList.AddLast(new Point() { x = 112, y = 217 }); pointList.AddLast(new Point() { x = 118, y = 209 }); pointList.AddLast(new Point() { x = 120, y = 207 }); pointList.AddLast(new Point() { x = 128, y = 196 }); pointList.AddLast(new Point() { x = 135, y = 187 }); pointList.AddLast(new Point() { x = 138, y = 183 }); pointList.AddLast(new Point() { x = 148, y = 167 }); pointList.AddLast(new Point() { x = 157, y = 153 }); pointList.AddLast(new Point() { x = 163, y = 145 }); pointList.AddLast(new Point() { x = 165, y = 142 }); pointList.AddLast(new Point() { x = 172, y = 133 }); pointList.AddLast(new Point() { x = 177, y = 127 }); pointList.AddLast(new Point() { x = 179, y = 127 }); pointList.AddLast(new Point() { x = 180, y = 125 });
        list.Add(new Unistroke("delete", pointList));
        #endregion
        #region left curly brace
        pointList = new LinkedList<Point>();
        pointList.AddLast(new Point() { x = 150, y = 116 }); pointList.AddLast(new Point() { x = 147, y = 117 }); pointList.AddLast(new Point() { x = 145, y = 116 }); pointList.AddLast(new Point() { x = 142, y = 116 }); pointList.AddLast(new Point() { x = 139, y = 117 }); pointList.AddLast(new Point() { x = 136, y = 117 }); pointList.AddLast(new Point() { x = 133, y = 118 }); pointList.AddLast(new Point() { x = 129, y = 121 }); pointList.AddLast(new Point() { x = 126, y = 122 }); pointList.AddLast(new Point() { x = 123, y = 123 }); pointList.AddLast(new Point() { x = 120, y = 125 }); pointList.AddLast(new Point() { x = 118, y = 127 }); pointList.AddLast(new Point() { x = 115, y = 128 }); pointList.AddLast(new Point() { x = 113, y = 129 }); pointList.AddLast(new Point() { x = 112, y = 131 }); pointList.AddLast(new Point() { x = 113, y = 134 }); pointList.AddLast(new Point() { x = 115, y = 134 }); pointList.AddLast(new Point() { x = 117, y = 135 }); pointList.AddLast(new Point() { x = 120, y = 135 }); pointList.AddLast(new Point() { x = 123, y = 137 }); pointList.AddLast(new Point() { x = 126, y = 138 }); pointList.AddLast(new Point() { x = 129, y = 140 }); pointList.AddLast(new Point() { x = 135, y = 143 }); pointList.AddLast(new Point() { x = 137, y = 144 }); pointList.AddLast(new Point() { x = 139, y = 147 }); pointList.AddLast(new Point() { x = 141, y = 149 }); pointList.AddLast(new Point() { x = 140, y = 152 }); pointList.AddLast(new Point() { x = 139, y = 155 }); pointList.AddLast(new Point() { x = 134, y = 159 }); pointList.AddLast(new Point() { x = 131, y = 161 }); pointList.AddLast(new Point() { x = 124, y = 166 }); pointList.AddLast(new Point() { x = 121, y = 166 }); pointList.AddLast(new Point() { x = 117, y = 166 }); pointList.AddLast(new Point() { x = 114, y = 167 }); pointList.AddLast(new Point() { x = 112, y = 166 }); pointList.AddLast(new Point() { x = 114, y = 164 }); pointList.AddLast(new Point() { x = 116, y = 163 }); pointList.AddLast(new Point() { x = 118, y = 163 }); pointList.AddLast(new Point() { x = 120, y = 162 }); pointList.AddLast(new Point() { x = 122, y = 163 }); pointList.AddLast(new Point() { x = 125, y = 164 }); pointList.AddLast(new Point() { x = 127, y = 165 }); pointList.AddLast(new Point() { x = 129, y = 166 }); pointList.AddLast(new Point() { x = 130, y = 168 }); pointList.AddLast(new Point() { x = 129, y = 171 }); pointList.AddLast(new Point() { x = 127, y = 175 }); pointList.AddLast(new Point() { x = 125, y = 179 }); pointList.AddLast(new Point() { x = 123, y = 184 }); pointList.AddLast(new Point() { x = 121, y = 190 }); pointList.AddLast(new Point() { x = 120, y = 194 }); pointList.AddLast(new Point() { x = 119, y = 199 }); pointList.AddLast(new Point() { x = 120, y = 202 }); pointList.AddLast(new Point() { x = 123, y = 207 }); pointList.AddLast(new Point() { x = 127, y = 211 }); pointList.AddLast(new Point() { x = 133, y = 215 }); pointList.AddLast(new Point() { x = 142, y = 219 }); pointList.AddLast(new Point() { x = 148, y = 220 }); pointList.AddLast(new Point() { x = 151, y = 221 });
        list.Add(new Unistroke("left curly brace", pointList));
        #endregion
        #region right curly brace
        pointList = new LinkedList<Point>();
        pointList.AddLast(new Point() { x = 117, y = 132 }); pointList.AddLast(new Point() { x = 115, y = 132 }); pointList.AddLast(new Point() { x = 115, y = 129 }); pointList.AddLast(new Point() { x = 117, y = 129 }); pointList.AddLast(new Point() { x = 119, y = 128 }); pointList.AddLast(new Point() { x = 122, y = 127 }); pointList.AddLast(new Point() { x = 125, y = 127 }); pointList.AddLast(new Point() { x = 127, y = 127 }); pointList.AddLast(new Point() { x = 130, y = 127 }); pointList.AddLast(new Point() { x = 133, y = 129 }); pointList.AddLast(new Point() { x = 136, y = 129 }); pointList.AddLast(new Point() { x = 138, y = 130 }); pointList.AddLast(new Point() { x = 140, y = 131 }); pointList.AddLast(new Point() { x = 143, y = 134 }); pointList.AddLast(new Point() { x = 144, y = 136 }); pointList.AddLast(new Point() { x = 145, y = 139 }); pointList.AddLast(new Point() { x = 145, y = 142 }); pointList.AddLast(new Point() { x = 145, y = 145 }); pointList.AddLast(new Point() { x = 145, y = 147 }); pointList.AddLast(new Point() { x = 145, y = 149 }); pointList.AddLast(new Point() { x = 144, y = 152 }); pointList.AddLast(new Point() { x = 142, y = 157 }); pointList.AddLast(new Point() { x = 141, y = 160 }); pointList.AddLast(new Point() { x = 139, y = 163 }); pointList.AddLast(new Point() { x = 137, y = 166 }); pointList.AddLast(new Point() { x = 135, y = 167 }); pointList.AddLast(new Point() { x = 133, y = 169 }); pointList.AddLast(new Point() { x = 131, y = 172 }); pointList.AddLast(new Point() { x = 128, y = 173 }); pointList.AddLast(new Point() { x = 126, y = 176 }); pointList.AddLast(new Point() { x = 125, y = 178 }); pointList.AddLast(new Point() { x = 125, y = 180 }); pointList.AddLast(new Point() { x = 125, y = 182 }); pointList.AddLast(new Point() { x = 126, y = 184 }); pointList.AddLast(new Point() { x = 128, y = 187 }); pointList.AddLast(new Point() { x = 130, y = 187 }); pointList.AddLast(new Point() { x = 132, y = 188 }); pointList.AddLast(new Point() { x = 135, y = 189 }); pointList.AddLast(new Point() { x = 140, y = 189 }); pointList.AddLast(new Point() { x = 145, y = 189 }); pointList.AddLast(new Point() { x = 150, y = 187 }); pointList.AddLast(new Point() { x = 155, y = 186 }); pointList.AddLast(new Point() { x = 157, y = 185 }); pointList.AddLast(new Point() { x = 159, y = 184 }); pointList.AddLast(new Point() { x = 156, y = 185 }); pointList.AddLast(new Point() { x = 154, y = 185 }); pointList.AddLast(new Point() { x = 149, y = 185 }); pointList.AddLast(new Point() { x = 145, y = 187 }); pointList.AddLast(new Point() { x = 141, y = 188 }); pointList.AddLast(new Point() { x = 136, y = 191 }); pointList.AddLast(new Point() { x = 134, y = 191 }); pointList.AddLast(new Point() { x = 131, y = 192 }); pointList.AddLast(new Point() { x = 129, y = 193 }); pointList.AddLast(new Point() { x = 129, y = 195 }); pointList.AddLast(new Point() { x = 129, y = 197 }); pointList.AddLast(new Point() { x = 131, y = 200 }); pointList.AddLast(new Point() { x = 133, y = 202 }); pointList.AddLast(new Point() { x = 136, y = 206 }); pointList.AddLast(new Point() { x = 139, y = 211 }); pointList.AddLast(new Point() { x = 142, y = 215 }); pointList.AddLast(new Point() { x = 145, y = 220 }); pointList.AddLast(new Point() { x = 147, y = 225 }); pointList.AddLast(new Point() { x = 148, y = 231 }); pointList.AddLast(new Point() { x = 147, y = 239 }); pointList.AddLast(new Point() { x = 144, y = 244 }); pointList.AddLast(new Point() { x = 139, y = 248 }); pointList.AddLast(new Point() { x = 134, y = 250 }); pointList.AddLast(new Point() { x = 126, y = 253 }); pointList.AddLast(new Point() { x = 119, y = 253 }); pointList.AddLast(new Point() { x = 115, y = 253 });
        list.Add(new Unistroke("right curly brace", pointList));
        #endregion
        #region star
        pointList = new LinkedList<Point>();
        pointList.AddLast(new Point() { x = 75, y = 250 }); pointList.AddLast(new Point() { x = 75, y = 247 }); pointList.AddLast(new Point() { x = 77, y = 244 }); pointList.AddLast(new Point() { x = 78, y = 242 }); pointList.AddLast(new Point() { x = 79, y = 239 }); pointList.AddLast(new Point() { x = 80, y = 237 }); pointList.AddLast(new Point() { x = 82, y = 234 }); pointList.AddLast(new Point() { x = 82, y = 232 }); pointList.AddLast(new Point() { x = 84, y = 229 }); pointList.AddLast(new Point() { x = 85, y = 225 }); pointList.AddLast(new Point() { x = 87, y = 222 }); pointList.AddLast(new Point() { x = 88, y = 219 }); pointList.AddLast(new Point() { x = 89, y = 216 }); pointList.AddLast(new Point() { x = 91, y = 212 }); pointList.AddLast(new Point() { x = 92, y = 208 }); pointList.AddLast(new Point() { x = 94, y = 204 }); pointList.AddLast(new Point() { x = 95, y = 201 }); pointList.AddLast(new Point() { x = 96, y = 196 }); pointList.AddLast(new Point() { x = 97, y = 194 }); pointList.AddLast(new Point() { x = 98, y = 191 }); pointList.AddLast(new Point() { x = 100, y = 185 }); pointList.AddLast(new Point() { x = 102, y = 178 }); pointList.AddLast(new Point() { x = 104, y = 173 }); pointList.AddLast(new Point() { x = 104, y = 171 }); pointList.AddLast(new Point() { x = 105, y = 164 }); pointList.AddLast(new Point() { x = 106, y = 158 }); pointList.AddLast(new Point() { x = 107, y = 156 }); pointList.AddLast(new Point() { x = 107, y = 152 }); pointList.AddLast(new Point() { x = 108, y = 145 }); pointList.AddLast(new Point() { x = 109, y = 141 }); pointList.AddLast(new Point() { x = 110, y = 139 }); pointList.AddLast(new Point() { x = 112, y = 133 }); pointList.AddLast(new Point() { x = 113, y = 131 }); pointList.AddLast(new Point() { x = 116, y = 127 }); pointList.AddLast(new Point() { x = 117, y = 125 }); pointList.AddLast(new Point() { x = 119, y = 122 }); pointList.AddLast(new Point() { x = 121, y = 121 }); pointList.AddLast(new Point() { x = 123, y = 120 }); pointList.AddLast(new Point() { x = 125, y = 122 }); pointList.AddLast(new Point() { x = 125, y = 125 }); pointList.AddLast(new Point() { x = 127, y = 130 }); pointList.AddLast(new Point() { x = 128, y = 133 }); pointList.AddLast(new Point() { x = 131, y = 143 }); pointList.AddLast(new Point() { x = 136, y = 153 }); pointList.AddLast(new Point() { x = 140, y = 163 }); pointList.AddLast(new Point() { x = 144, y = 172 }); pointList.AddLast(new Point() { x = 145, y = 175 }); pointList.AddLast(new Point() { x = 151, y = 189 }); pointList.AddLast(new Point() { x = 156, y = 201 }); pointList.AddLast(new Point() { x = 161, y = 213 }); pointList.AddLast(new Point() { x = 166, y = 225 }); pointList.AddLast(new Point() { x = 169, y = 233 }); pointList.AddLast(new Point() { x = 171, y = 236 }); pointList.AddLast(new Point() { x = 174, y = 243 }); pointList.AddLast(new Point() { x = 177, y = 247 }); pointList.AddLast(new Point() { x = 178, y = 249 }); pointList.AddLast(new Point() { x = 179, y = 251 }); pointList.AddLast(new Point() { x = 180, y = 253 }); pointList.AddLast(new Point() { x = 180, y = 255 }); pointList.AddLast(new Point() { x = 179, y = 257 }); pointList.AddLast(new Point() { x = 177, y = 257 }); pointList.AddLast(new Point() { x = 174, y = 255 }); pointList.AddLast(new Point() { x = 169, y = 250 }); pointList.AddLast(new Point() { x = 164, y = 247 }); pointList.AddLast(new Point() { x = 160, y = 245 }); pointList.AddLast(new Point() { x = 149, y = 238 }); pointList.AddLast(new Point() { x = 138, y = 230 }); pointList.AddLast(new Point() { x = 127, y = 221 }); pointList.AddLast(new Point() { x = 124, y = 220 }); pointList.AddLast(new Point() { x = 112, y = 212 }); pointList.AddLast(new Point() { x = 110, y = 210 }); pointList.AddLast(new Point() { x = 96, y = 201 }); pointList.AddLast(new Point() { x = 84, y = 195 }); pointList.AddLast(new Point() { x = 74, y = 190 }); pointList.AddLast(new Point() { x = 64, y = 182 }); pointList.AddLast(new Point() { x = 55, y = 175 }); pointList.AddLast(new Point() { x = 51, y = 172 }); pointList.AddLast(new Point() { x = 49, y = 170 }); pointList.AddLast(new Point() { x = 51, y = 169 }); pointList.AddLast(new Point() { x = 56, y = 169 }); pointList.AddLast(new Point() { x = 66, y = 169 }); pointList.AddLast(new Point() { x = 78, y = 168 }); pointList.AddLast(new Point() { x = 92, y = 166 }); pointList.AddLast(new Point() { x = 107, y = 164 }); pointList.AddLast(new Point() { x = 123, y = 161 }); pointList.AddLast(new Point() { x = 140, y = 162 }); pointList.AddLast(new Point() { x = 156, y = 162 }); pointList.AddLast(new Point() { x = 171, y = 160 }); pointList.AddLast(new Point() { x = 173, y = 160 }); pointList.AddLast(new Point() { x = 186, y = 160 }); pointList.AddLast(new Point() { x = 195, y = 160 }); pointList.AddLast(new Point() { x = 198, y = 161 }); pointList.AddLast(new Point() { x = 203, y = 163 }); pointList.AddLast(new Point() { x = 208, y = 163 }); pointList.AddLast(new Point() { x = 206, y = 164 }); pointList.AddLast(new Point() { x = 200, y = 167 }); pointList.AddLast(new Point() { x = 187, y = 172 }); pointList.AddLast(new Point() { x = 174, y = 179 }); pointList.AddLast(new Point() { x = 172, y = 181 }); pointList.AddLast(new Point() { x = 153, y = 192 }); pointList.AddLast(new Point() { x = 137, y = 201 }); pointList.AddLast(new Point() { x = 123, y = 211 }); pointList.AddLast(new Point() { x = 112, y = 220 }); pointList.AddLast(new Point() { x = 99, y = 229 }); pointList.AddLast(new Point() { x = 90, y = 237 }); pointList.AddLast(new Point() { x = 80, y = 244 }); pointList.AddLast(new Point() { x = 73, y = 250 }); pointList.AddLast(new Point() { x = 69, y = 254 }); pointList.AddLast(new Point() { x = 69, y = 252 });
        list.Add(new Unistroke("star", pointList));
        #endregion
        #region pigtail
        pointList = new LinkedList<Point>();
        pointList.AddLast(new Point() { x = 81, y = 219 }); pointList.AddLast(new Point() { x = 84, y = 218 }); pointList.AddLast(new Point() { x = 86, y = 220 }); pointList.AddLast(new Point() { x = 88, y = 220 }); pointList.AddLast(new Point() { x = 90, y = 220 }); pointList.AddLast(new Point() { x = 92, y = 219 }); pointList.AddLast(new Point() { x = 95, y = 220 }); pointList.AddLast(new Point() { x = 97, y = 219 }); pointList.AddLast(new Point() { x = 99, y = 220 }); pointList.AddLast(new Point() { x = 102, y = 218 }); pointList.AddLast(new Point() { x = 105, y = 217 }); pointList.AddLast(new Point() { x = 107, y = 216 }); pointList.AddLast(new Point() { x = 110, y = 216 }); pointList.AddLast(new Point() { x = 113, y = 214 }); pointList.AddLast(new Point() { x = 116, y = 212 }); pointList.AddLast(new Point() { x = 118, y = 210 }); pointList.AddLast(new Point() { x = 121, y = 208 }); pointList.AddLast(new Point() { x = 124, y = 205 }); pointList.AddLast(new Point() { x = 126, y = 202 }); pointList.AddLast(new Point() { x = 129, y = 199 }); pointList.AddLast(new Point() { x = 132, y = 196 }); pointList.AddLast(new Point() { x = 136, y = 191 }); pointList.AddLast(new Point() { x = 139, y = 187 }); pointList.AddLast(new Point() { x = 142, y = 182 }); pointList.AddLast(new Point() { x = 144, y = 179 }); pointList.AddLast(new Point() { x = 146, y = 174 }); pointList.AddLast(new Point() { x = 148, y = 170 }); pointList.AddLast(new Point() { x = 149, y = 168 }); pointList.AddLast(new Point() { x = 151, y = 162 }); pointList.AddLast(new Point() { x = 152, y = 160 }); pointList.AddLast(new Point() { x = 152, y = 157 }); pointList.AddLast(new Point() { x = 152, y = 155 }); pointList.AddLast(new Point() { x = 152, y = 151 }); pointList.AddLast(new Point() { x = 152, y = 149 }); pointList.AddLast(new Point() { x = 152, y = 146 }); pointList.AddLast(new Point() { x = 149, y = 142 }); pointList.AddLast(new Point() { x = 148, y = 139 }); pointList.AddLast(new Point() { x = 145, y = 137 }); pointList.AddLast(new Point() { x = 141, y = 135 }); pointList.AddLast(new Point() { x = 139, y = 135 }); pointList.AddLast(new Point() { x = 134, y = 136 }); pointList.AddLast(new Point() { x = 130, y = 140 }); pointList.AddLast(new Point() { x = 128, y = 142 }); pointList.AddLast(new Point() { x = 126, y = 145 }); pointList.AddLast(new Point() { x = 122, y = 150 }); pointList.AddLast(new Point() { x = 119, y = 158 }); pointList.AddLast(new Point() { x = 117, y = 163 }); pointList.AddLast(new Point() { x = 115, y = 170 }); pointList.AddLast(new Point() { x = 114, y = 175 }); pointList.AddLast(new Point() { x = 117, y = 184 }); pointList.AddLast(new Point() { x = 120, y = 190 }); pointList.AddLast(new Point() { x = 125, y = 199 }); pointList.AddLast(new Point() { x = 129, y = 203 }); pointList.AddLast(new Point() { x = 133, y = 208 }); pointList.AddLast(new Point() { x = 138, y = 213 }); pointList.AddLast(new Point() { x = 145, y = 215 }); pointList.AddLast(new Point() { x = 155, y = 218 }); pointList.AddLast(new Point() { x = 164, y = 219 }); pointList.AddLast(new Point() { x = 166, y = 219 }); pointList.AddLast(new Point() { x = 177, y = 219 }); pointList.AddLast(new Point() { x = 182, y = 218 }); pointList.AddLast(new Point() { x = 192, y = 216 }); pointList.AddLast(new Point() { x = 196, y = 213 }); pointList.AddLast(new Point() { x = 199, y = 212 }); pointList.AddLast(new Point() { x = 201, y = 211 });
        list.Add(new Unistroke("pigtail", pointList));
        #endregion
        
        return list;
    }
    #endregion
}