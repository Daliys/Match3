using System;
using System.Collections.Generic;
using DG.Tweening;
using ObjectPool;
using ScriptableObjects;
using UnityEngine;

/// <summary>
/// Class that Generate the game field, and determine the logic of the game.
/// </summary>
public class GameGenerator : MonoBehaviour
{
    /// <summary>
    ///  Event that is invoked when the field is available for the next move. (When the animation is finished)
    /// </summary>
    public static event Action OnAvailableForNextMove;

    // references to the game objects
    [SerializeField] private GameObject gameField;
    [SerializeField] private GameObject fieldTilePrefab1;
    [SerializeField] private GameObject fieldTilePrefab2;
    [SerializeField] private AvailableColorsSO availableColorsSo;

    private Point _gridSize;
    private float _fieldTileSize;
    private Vector2 _gameBorder;
    
    // cashing the tiles (for optimization)
    private Tile[,] _tilesArray;
    private FieldLogic _fieldLogic;

    private bool _shouldPlayAnimation = true;

    // cashing the matched points (for optimization)
    private readonly List<Point> _matchedPoints = new();

    private void Awake()
    {
        PlayerInput.OnTileSwitched += SwitchTiles;
    }

    public void Initialize(Point gridSize, int numOfColors, Vector2 gameBorder)
    {
        _gridSize = gridSize;
        _gameBorder = gameBorder;

        float sizeX = gameBorder.x * 2;
        float sizeY = gameBorder.y * 2;

        _fieldTileSize = Mathf.Min(sizeX / gridSize.x, sizeY / gridSize.y);
        _tilesArray = new Tile[gridSize.x, gridSize.y];
        _fieldLogic = new FieldLogic(gridSize, numOfColors);

        GenerateGameField();
        _fieldLogic.GenerateInitialField();
        ApplyColorToObjects();

        OnAvailableForNextMove?.Invoke();
    }

    /// <summary>
    /// Try to switch the tiles. If the tiles are switched, check if there is any combination after switch. If there is, play the animation
    /// and check if there is any combination after animation. If there is not, switch the tiles back.
    /// </summary>
    /// <param name="point"> Point of the tile</param>
    /// <param name="side"> Side to switch</param>
    public void SwitchTiles(Point point, Side side)
    {
        Point pointToSwitch = point.GetPoint(side);

        // if the tile is not in bounds, return
        if (!_fieldLogic.CheckIfTileIsInBounds(pointToSwitch))
        {
            if (_shouldPlayAnimation) OnAvailableForNextMove?.Invoke();
            return;
        }

        bool isAnyCombinationAfterSwitch = CheckIfAnyCombinationAfterSwitch(point, pointToSwitch);

        if (_shouldPlayAnimation)
        {
            PlaySwitchAnimation(point, pointToSwitch, () =>
            {
                if (isAnyCombinationAfterSwitch)
                {
                    ProcessMatchResult();
                }
                else
                {
                    PlaySwitchAnimation(point, pointToSwitch, () => OnAvailableForNextMove?.Invoke());
                }
            });
        }
        else if (isAnyCombinationAfterSwitch)
        {
            ProcessMatchResult();
        }
    }

    /// <summary>
    ///  Check if there is any combination after switch that might be destroyed after animation.
    /// </summary>
    private bool CheckIfAnyCombinationAfterSwitch(Point firstPoint, Point secondPoint)
    {
        SwitchTile(firstPoint, secondPoint);

        // look for the combination 
        _matchedPoints.Clear();
        AddPointsToMatchedPoints(_fieldLogic.CheckForCombination(firstPoint));
        AddPointsToMatchedPoints(_fieldLogic.CheckForCombination(secondPoint));

        if (_matchedPoints.Count == 0)
        {
            SwitchTile(firstPoint, secondPoint);
        }

        return _matchedPoints.Count != 0;
    }

    private void PlaySwitchAnimation(Point firstPoint, Point secondPoint, TweenCallback callback)
    {
        TileSet tileSet = new TileSet();
        tileSet.Add(GetTile(firstPoint), GetTile(secondPoint).transform.position);
        tileSet.Add(GetTile(secondPoint), GetTile(firstPoint).transform.position);
        tileSet.PlaySwitchAnimation(callback);
    }

    /// <summary>
    ///  Destroy the matched tiles and generate new tiles. If there is any combination after animation, play the animation again.
    /// </summary>
    private void ProcessMatchResult()
    {
        // cash the left and right positions to fall (for optimization we won't check the whole field)
        int leftPositionsToFall = int.MaxValue;
        int rightPositionsToFall = int.MinValue;

        foreach (Point matchedPoint in _matchedPoints)
        {
            leftPositionsToFall = Mathf.Min(leftPositionsToFall, matchedPoint.x);
            rightPositionsToFall = Mathf.Max(rightPositionsToFall, matchedPoint.x);
            _fieldLogic.SetColor(matchedPoint, 0);
        }

        // we using 2 different methods for optimization (one for animation and one without animation)

        if (_shouldPlayAnimation)
        {
            PlayDestroyAnimation(() => FindTilesForFallAndPlayAnimation(leftPositionsToFall, rightPositionsToFall));
        }
        else
        {
            FindTilesForFall(leftPositionsToFall, rightPositionsToFall);
        }
    }


    /// <summary>
    ///  Find the tiles that should fall and generate new color.
    /// </summary>
    private void FindTilesForFall(int leftPositionsToFall, int rightPositionsToFall)
    {
        for (int x = leftPositionsToFall; x <= rightPositionsToFall; x++)
        {
            int fallDistance = 0;
            for (int y = 0; y < _gridSize.y; y++)
            {
                if (_fieldLogic.GetColor(x, y) == 0)
                {
                    fallDistance++;
                }
                else if (fallDistance != 0)
                {
                    SwitchTile(new Point(x, y), new Point(x, y - fallDistance));
                }
            }

            // generate colors in the empty positions
            for (int j = 0; j < fallDistance; j++)
            {
                Point point = new Point(x, _gridSize.y - 1 - j);
                _fieldLogic.GenerateColorForTile(point);
            }
        }

        CheckFullFieldForMatch();
    }


    /// <summary>
    ///  Find the tiles that should fall and play the animation of falling.
    /// </summary>
    private void FindTilesForFallAndPlayAnimation(int leftPositionsToFall, int rightPositionsToFall)
    {
        TileSet tileSetForFall = new TileSet();
        // check for the tiles that should fall
        for (int i = leftPositionsToFall; i <= rightPositionsToFall; i++)
        {
            int fallDistance = 0;
            for (int y = 0; y < _gridSize.y; y++)
            {
                if (_tilesArray[i, y] == null)
                {
                    fallDistance++;
                }
                else if (fallDistance != 0)
                {
                    Vector2 moveTo = _tilesArray[i, y].transform.position +
                                     new Vector3(0, -fallDistance * _fieldTileSize);
                    tileSetForFall.Add(_tilesArray[i, y], moveTo, fallDistance);

                    SwitchTile(new Point(i, y), new Point(i, y - fallDistance));
                }
            }

            // generate new tiles above the field and play the animation of falling
            for (int j = 0; j < fallDistance; j++)
            {
                Point point = new Point(i, _gridSize.y - 1 - j);

                _fieldLogic.GenerateColorForTile(point);
                Tile tile = GenerateTile(new Point(i, _gridSize.y + fallDistance - j - 1));
                tile.SetPoint(point);
                _tilesArray[point.x, point.y] = tile;

                ApplyColorForObject(point);
                tileSetForFall.Add(tile,
                    _tilesArray[point.x, point.y].transform.position + new Vector3(0, -fallDistance * _fieldTileSize),
                    fallDistance);
            }
        }

        tileSetForFall.PlayFallAnimation(CheckFullFieldForMatch);
    }

    private void PlayDestroyAnimation(TweenCallback callback)
    {
        TileSet tileSetForDestroy = new TileSet();
        foreach (var matchedPoint in _matchedPoints)
        {
            tileSetForDestroy.Add(_tilesArray[matchedPoint.x, matchedPoint.y]);
        }

        tileSetForDestroy.PlayDestroyAnimation(() =>
        {
            foreach (var point in _matchedPoints)
            {
                ObjectPooling.Instance.ReturnToPool(_tilesArray[point.x, point.y].gameObject);
                _tilesArray[point.x, point.y] = null;
            }

            callback?.Invoke();
        });

    }

    /// <summary>
    ///  Checking the whole field for the combination.
    /// </summary>
    private void CheckFullFieldForMatch()
    {
        _matchedPoints.Clear();
        for (int i = 0; i < _gridSize.x; i++)
        {
            for (int j = 0; j < _gridSize.y; j++)
            {
                AddPointsToMatchedPoints(_fieldLogic.CheckForCombination(new Point(i, j)));
            }
        }

        if (_matchedPoints.Count == 0)
        {
            if (_shouldPlayAnimation)
            {
                OnAvailableForNextMove?.Invoke();
            }

            return;
        }

        ProcessMatchResult();
    }

    /// <summary>
    ///  Generate new tile in the empty position.
    /// </summary>
    /// <param name="point"> Point of the tile that should be generated</param>
    private Tile GenerateTile(Point point)
    {
        GameObject tile = ObjectPooling.Instance.GetObject(TileType.Simple);

        tile.transform.position = new Vector2(-_gameBorder.x + _fieldTileSize / 2 + point.x * _fieldTileSize,
            -_gameBorder.y + _fieldTileSize / 2 + point.y * _fieldTileSize);
        tile.transform.localScale = new Vector2(_fieldTileSize, _fieldTileSize);
        return tile.GetComponent<Tile>();
    }

    /// <summary>
    ///  Switch the colors and the tiles.
    /// </summary>
    private void SwitchTile(Point firstPoint, Point secondPoint)
    {
        _fieldLogic.SwitchColors(firstPoint, secondPoint);

        // if we don't need to play the animation, we don't need to switch the tiles
        if (!_shouldPlayAnimation) return;

        (_tilesArray[firstPoint.x, firstPoint.y], _tilesArray[secondPoint.x, secondPoint.y]) = (
            _tilesArray[secondPoint.x, secondPoint.y], _tilesArray[firstPoint.x, firstPoint.y]);

        if (_tilesArray[firstPoint.x, firstPoint.y])
        {
            _tilesArray[firstPoint.x, firstPoint.y].SetPoint(firstPoint);
        }

        if (_tilesArray[secondPoint.x, secondPoint.y])
        {
            _tilesArray[secondPoint.x, secondPoint.y].SetPoint(secondPoint);
        }
    }

    /// <summary>
    ///  Generate the game field with the tiles. (background tiles and tiles that can be switched)
    /// </summary>
    private void GenerateGameField()
    {
        float startX = -_gameBorder.x + _fieldTileSize / 2;
        float startY = -_gameBorder.y + _fieldTileSize / 2;

        for (int i = 0; i < _gridSize.x; i++)
        {
            for (int j = 0; j < _gridSize.y; j++)
            {
                GameObject fieldTile = Instantiate(GetFieldTilePrefab(i, j), gameField.transform);
                GameObject tile = ObjectPooling.Instance.GetObject(TileType.Simple);

                fieldTile.transform.position = tile.transform.position =
                    new Vector2(startX + i * _fieldTileSize, startY + j * _fieldTileSize);
                fieldTile.transform.localScale =
                    tile.transform.localScale = new Vector2(_fieldTileSize, _fieldTileSize);

                _tilesArray[i, j] = tile.GetComponent<Tile>();
                _tilesArray[i, j].SetPoint(new Point(i, j));
            }
        }
    }

    /// <summary>
    ///  Apply for each tile the color that is stored in the logic.
    /// </summary>
    public void ApplyColorToObjects()
    {
        for (int i = 0; i < _gridSize.x; i++)
        {
            for (int j = 0; j < _gridSize.y; j++)
            {
                ApplyColorForObject(new Point(i, j));
            }
        }
    }

    /// <summary>
    /// Cashing tiles that matched. (for optimization)
    /// </summary>
    private void AddPointsToMatchedPoints(List<Point> points)
    {
        foreach (Point point in points)
        {
            AddPointToMatchedPoints(point);
        }
    }

    /// <summary>
    /// Cashing tiles that matched. (for optimization)
    /// </summary>
    private void AddPointToMatchedPoints(Point point)
    {
        foreach (Point matchPoint in _matchedPoints)
        {
            if (matchPoint.Equals(point)) return;
        }

        _matchedPoints.Add(point);
    }


    private void ApplyColorForObject(Point point)
    {
        _tilesArray[point.x, point.y]
            .ChangeColor(availableColorsSo.colors[_fieldLogic.GetColor(new Point(point.x, point.y)) - 1]);
    }

    /// <summary>
    /// Using for generation background tiles ( 2 different colors in the chess order)
    /// </summary>
    private GameObject GetFieldTilePrefab(int i, int j)
    {
        return (i % 2 == j % 2) ? fieldTilePrefab1 : fieldTilePrefab2;
    }

    private Tile GetTile(Point point)
    {
        return _tilesArray[point.x, point.y];
    }

    private void OnDestroy()
    {
        PlayerInput.OnTileSwitched -= SwitchTiles;
    }

    public Point GetGridSize() => _gridSize;

    public void SetShouldPlayAnimation(bool shouldPlayAnimation)
    {
        _shouldPlayAnimation = shouldPlayAnimation;
    }
}
