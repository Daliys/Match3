using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Class that contains the logic of the field. It's used to determine the colors of the tiles. Callculate the combinations.
/// </summary>
public class FieldLogic
{
    private readonly int[,] _colorsArray;
    private readonly int[] _cashedColors;

    // cashing the colors to pick up for the tile (for optimization)
    private readonly List<int> _availableColorsToPickUp = new List<int>();
    private readonly int _numOfColors;
    private readonly Point _gridSize;

    public FieldLogic(Point gridSize, int numOfColors)
    {
        _gridSize = gridSize;
        _numOfColors = numOfColors;
        _colorsArray = new int[_gridSize.x, _gridSize.y];
        _cashedColors = new int[_numOfColors];

        for (int i = 0; i < _numOfColors; i++)
        {
            _cashedColors[i] = i + 1;
        }
    }

    /// <summary>
    ///  Generate the color for the tile. It's used to determine the colors of the tiles.
    /// </summary>
    /// <param name="point"> Point of the tile</param>
    private int GenerateUnmatchableColorForTile(Point point)
    {
        _availableColorsToPickUp.Clear();
        _availableColorsToPickUp.AddRange(_cashedColors);


        if (CheckIfTileIsSameColor(point.Up(), point.Down()))
        {
            _availableColorsToPickUp.Remove(_colorsArray[point.Up().x, point.Up().y]);
        }

        if (CheckIfTileIsSameColor(point.Right(), point.Left()))
        {
            _availableColorsToPickUp.Remove(_colorsArray[point.Right().x, point.Right().y]);
        }

        if (CheckIfTileIsSameColor(point.Up(), point.DUp()))
        {
            _availableColorsToPickUp.Remove(_colorsArray[point.Up().x, point.Up().y]);
        }

        if (CheckIfTileIsSameColor(point.Right(), point.DRight()))
        {
            _availableColorsToPickUp.Remove(_colorsArray[point.Right().x, point.Right().y]);
        }

        if (CheckIfTileIsSameColor(point.Down(), point.DDown()))
        {
            _availableColorsToPickUp.Remove(_colorsArray[point.Down().x, point.Down().y]);
        }

        if (CheckIfTileIsSameColor(point.Left(), point.DLeft()))
        {
            _availableColorsToPickUp.Remove(_colorsArray[point.Left().x, point.Left().y]);
        }

        // if there is no available color to pick up, return random color
        return _availableColorsToPickUp.Count == 0
            ? Random.Range(1, _numOfColors + 1)
            : _availableColorsToPickUp[Random.Range(0, _availableColorsToPickUp.Count)];
    }


    /// <summary>
    /// Switch 2 colors of the tiles
    /// </summary>
    public void SwitchColors(Point first, Point second)
    {
        (_colorsArray[first.x, first.y], _colorsArray[second.x, second.y]) =
            (_colorsArray[second.x, second.y], _colorsArray[first.x, first.y]);
    }


    /// <summary>
    ///  Check if there is any combination on the field and return the list of the tiles that should be destroyed
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public List<Point> CheckForCombination(Point point)
    {
        List<Point> returnList = CheckIfAnyCombination(point);

        if (returnList.Count > 0)
        {
            returnList.Add(point);
        }

        return returnList;
    }

    /// <summary>
    /// Check if there is any combination on the field and return the list of the tiles that should be destroyed
    /// </summary>
    /// <param name="point"> Point of the tile that should be checked</param>
    /// <returns> List of the tiles that should be destroyed</returns>
    private List<Point> CheckIfAnyCombination(Point point)
    {
        List<Point> returnList = new List<Point>();

        List<Point> pointsThatMatchVerticalUp = new List<Point>();
        List<Point> pointsThatMatchVerticalDown = new List<Point>();
        List<Point> pointsThatMatchHorizontalLeft = new List<Point>();
        List<Point> pointsThatMatchHorizontalRight = new List<Point>();

        CheckDirection(point, point.Up(), point.DUp(), ref pointsThatMatchVerticalUp);
        CheckDirection(point, point.Down(), point.DDown(), ref pointsThatMatchVerticalDown);

        CheckDirection(point, point.Right(), point.DRight(), ref pointsThatMatchHorizontalRight);
        CheckDirection(point, point.Left(), point.DLeft(), ref pointsThatMatchHorizontalLeft);

        if (pointsThatMatchVerticalUp.Count + pointsThatMatchVerticalDown.Count < 2)
        {
            pointsThatMatchVerticalUp.Clear();
            pointsThatMatchVerticalDown.Clear();
        }

        if (pointsThatMatchHorizontalLeft.Count + pointsThatMatchHorizontalRight.Count < 2)
        {
            pointsThatMatchHorizontalLeft.Clear();
            pointsThatMatchHorizontalRight.Clear();
        }

        CheckForPattern(pointsThatMatchVerticalUp, pointsThatMatchVerticalDown, pointsThatMatchHorizontalLeft,
            pointsThatMatchHorizontalRight, returnList);

        return returnList;
    }

    private void CheckForPattern(List<Point> up, List<Point> down, List<Point> left, List<Point> right,
        List<Point> returnList)
    {
        int sum = up.Count + down.Count + left.Count + right.Count;
        int sumHorizontal = left.Count + right.Count;
        int sumVertical = up.Count + down.Count;

        // Below are the patterns that we are looking for (you can read about them in the documentation readme.md)

        // Pattern 1 
        if (sum == 8)
        {
            returnList.AddRange(up);
            returnList.AddRange(down);
            returnList.AddRange(left);
            returnList.AddRange(right);
        }
        else if (sum is 7 or 6) // Pattern 2
        {
            if (sumHorizontal == 4)
            {
                returnList.AddRange(right);
                returnList.AddRange(left);

                if (up.Count != down.Count)
                {
                    returnList.AddRange(up.Count > down.Count ? up : down);
                }
            }
            else
            {
                returnList.AddRange(up);
                returnList.AddRange(down);

                if (left.Count == right.Count)
                {
                    returnList.AddRange(left.Count > right.Count ? left : right);
                }
            }
        }
        else if (sum is 5 or 4) // Pattern 3, 4, 5, 6 (depends of items location)
        {
            if (sumHorizontal == 4) // Pattern 3
            {
                returnList.AddRange(right);
                returnList.AddRange(left);
                return;
            }

            if (sumVertical == 4) // Pattern 3
            {
                returnList.AddRange(up);
                returnList.AddRange(down);
                return;
            }

            if (sum == 4)
            {
                returnList.AddRange(up);
                returnList.AddRange(down);
                returnList.AddRange(left);
                returnList.AddRange(right);
                return;
            }

            if (up.Count == 2) returnList.AddRange(up);
            if (down.Count == 2) returnList.AddRange(down);
            if (left.Count == 2) returnList.AddRange(left);
            if (right.Count == 2) returnList.AddRange(right);

        }
        else if (sum is 3 or 2) // Pattern 7, 8
        {
            returnList.AddRange(up);
            returnList.AddRange(down);
            returnList.AddRange(left);
            returnList.AddRange(right);
        }
    }

    /// <summary>
    /// Check if 2 tiles have the same color
    /// </summary>
    private bool CheckIfTileIsSameColor(Point point1, Point point2)
    {
        if (!CheckIfTileIsInBounds(point1) || !CheckIfTileIsInBounds(point2)) return false;

        return _colorsArray[point1.x, point1.y] != 0 &&
               _colorsArray[point1.x, point1.y] == _colorsArray[point2.x, point2.y];
    }

    /// <summary>
    ///  Check if the tile is in the field
    /// </summary>
    public bool CheckIfTileIsInBounds(Point point)
    {
        return point.x >= 0 && point.x < _gridSize.x && point.y >= 0 && point.y < _gridSize.y;
    }

    /// <summary>
    ///  Check if there is any combination on the field and return the list of the tiles that should be destroyed
    /// </summary>
    /// <param name="origin"> Point of the tile that should be checked</param>
    /// <param name="firstPoint"> Neighbour of the origin tile</param>
    /// <param name="secondPoint"> Double neighbour of the origin tile</param>
    /// <param name="pointsThatMatch"> List of the tiles that should be destroyed</param>
    private void CheckDirection(Point origin, Point firstPoint, Point secondPoint, ref List<Point> pointsThatMatch)
    {
        if (CheckIfTileIsSameColor(origin, firstPoint))
        {
            pointsThatMatch.Add(firstPoint);
            if (CheckIfTileIsSameColor(origin, secondPoint))
            {
                pointsThatMatch.Add(secondPoint);
            }
        }
    }

    /// <summary>
    ///  Generate the color for the tile. It's used to determine the colors of the tiles.
    /// </summary>
    public void GenerateColorForTile(Point point)
    {
        _colorsArray[point.x, point.y] = GenerateUnmatchableColorForTile(point);
    }

    /// <summary>
    ///  Generate for initial field off all tiles
    /// </summary>
    public void GenerateInitialField()
    {
        for (int x = 0; x < _gridSize.x; x++)
        {
            for (int y = 0; y < _gridSize.y; y++)
            {
                GenerateColorForTile(new Point(x, y));
            }
        }
    }

    public int GetColor(Point point)
    {
        return _colorsArray[point.x, point.y];
    }

    public int GetColor(int x, int y)
    {
        return _colorsArray[x, y];
    }

    public void SetColor(Point point, int color)
    {
        _colorsArray[point.x, point.y] = color;
    }
}