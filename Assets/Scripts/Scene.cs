using UnityEngine;

/// <summary>
/// Class for scene. It's used to initialize the game.
/// </summary>
public class Scene : MonoBehaviour
{
    private Vector2 _gameBorder;
    [SerializeField] private GameGenerator gameGenerator;

    void Awake()
    {
        float cameraSize = Camera.main.orthographicSize;

        // Calculate aspect ratio for the game border. (we can use any resolution)
        float aspectRatioX = (float)Screen.width / (float)Screen.height;
        _gameBorder = new Vector2(cameraSize * aspectRatioX, cameraSize);
    }


    public void Initialize(Point gridSize, int numOfColors)
    {
        gameGenerator.Initialize(gridSize, numOfColors, _gameBorder);
    }
}
