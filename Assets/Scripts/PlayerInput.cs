using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
///  Class that handle player input
/// </summary>
public class PlayerInput : MonoBehaviour
{
    /// <summary>
    ///  Event that is invoked when the tile is switched by player.
    /// </summary>
    public static event Action<Point, Side> OnTileSwitched;

    private Vector3 _mouseDownPosition;
    private Camera _camera;
    private Tile _selectedTile;

    private bool _isActive;

    private void Start()
    {
        // cashing the camera
        _camera = Camera.main;
        GameGenerator.OnAvailableForNextMove += OnAvailableForNextMove;
    }

    /// <summary>
    /// If animation is finished then player can make a move.
    /// </summary>
    private void OnAvailableForNextMove()
    {
        _isActive = true;
    }

    private void Update()
    {
        if (!_isActive) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Tile"))
                {
                    _selectedTile = hit.collider.GetComponent<Tile>();
                    _mouseDownPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
                }
            }
        }

        else if (Input.GetMouseButtonUp(0))
        {
            if (_selectedTile == null) return;


            Vector2 direction = (_camera.ScreenToWorldPoint(Input.mousePosition) - _mouseDownPosition);
            direction.x = Mathf.Abs(direction.x) > Mathf.Abs(direction.y) ? direction.x : 0;
            direction.y = Mathf.Abs(direction.y) > Mathf.Abs(direction.x) ? direction.y : 0;

            if (direction.magnitude < _selectedTile.GetDistanceToCompleteSwitch())
            {
                return;
            }

            _isActive = false;
            OnTileSwitched?.Invoke(_selectedTile.GetPoint(), DirectionToSide(direction));
            _selectedTile = null;
        }
    }

    /// <summary>
    ///  Convert direction to side (calculate the side of the tile that should be switched)
    /// </summary>
    private Side DirectionToSide(Vector2 direction)
    {
        direction.Normalize();
        Point point = new Point
        {
            x = Mathf.RoundToInt(direction.x),
            y = Mathf.RoundToInt(direction.y)
        };

        if (point.x == 0 && point.y == 1)
            return Side.Down;
        if (point.x == 0 && point.y == -1)
            return Side.Up;
        if (point.x == 1 && point.y == 0)
            return Side.Right;
        if (point.x == -1 && point.y == 0)
            return Side.Left;

        // if the direction is not valid, return up
        return Side.Up;
    }

    private void OnDestroy()
    {
        GameGenerator.OnAvailableForNextMove -= OnAvailableForNextMove;
    }
}
