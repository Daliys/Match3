using System.Collections;
using UnityEngine;

/// <summary>
///  Class that handle the simulation. Here is 2 ways to simulate the game. With animation and without animation.
/// </summary>
public class Simulation : MonoBehaviour
{
    [SerializeField] private GameGenerator gameGenerator;
    [SerializeField] private PlayerInput playerInput;
    private int _numOfSimulation;
    private bool _withAnimation;

    private Point _gridSize;
    private Side[] _availableSideToMove;

    int _counter;

    public void Initialize(int numOfSimulation, bool withAnimation)
    {
        _numOfSimulation = numOfSimulation;
        _withAnimation = withAnimation;

        StartCoroutine(WaitForInitialization());
    }

    private void StartSimulation()
    {
        _gridSize = gameGenerator.GetGridSize();
        _availableSideToMove = new[] { Side.Up, Side.Down, Side.Right, Side.Left };
        
        if (_withAnimation)
        {
            // with animation simulation game will be just playing by itself and will be waiting for the end of the animation
            GameGenerator.OnAvailableForNextMove += OnAvailableForNextMove;
            playerInput.enabled = false;

            OnAvailableForNextMove();
        }
        else
        {
            // without animation simulation game will just switch tiles without animation and will show how much time it took to simulate N moves
            
            // but honestly it is kinda useless. Because it woking using only 1 thread and it's not really fast. (it might be multithreaded)
            // the game not really optimized for this kind of simulation. // I didn't get the point of this task. so decided to do add 2 ways of simulation.
            
            Point randomPoint = new Point();
            gameGenerator.SetShouldPlayAnimation(false);

            long startTime = System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;

            for (int i = 0; i < _numOfSimulation; i++)
            {
                randomPoint.x = Random.Range(0, _gridSize.x);
                randomPoint.y = Random.Range(0, _gridSize.y);

                gameGenerator.SwitchTiles(randomPoint,
                    _availableSideToMove[Random.Range(0, _availableSideToMove.Length)]);
            }

            long finishTime = System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;

            Debug.Log("Time " + (finishTime - startTime) + " ms");

            gameGenerator.ApplyColorToObjects();
        }
    }

    /// <summary>
    ///  Callback that will be called when the game is available for next move (when the animation is finished) start the next move.
    /// </summary>
    private void OnAvailableForNextMove()
    {
        _counter++;
        if (_counter < _numOfSimulation)
        {
            gameGenerator.SwitchTiles(new Point(Random.Range(0, _gridSize.x), Random.Range(0, _gridSize.y)),
                _availableSideToMove[Random.Range(0, _availableSideToMove.Length)]);
        }
        else
        {
            Debug.Log(" Simulation finished");
        }
    }

    /// <summary>
    ///  Wait for initialization of the game generator (it's needed to wait for the initialization of the game generator because it's initialize the grid size and number of colors)
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForInitialization()
    {
        yield return null;
        StartSimulation();
    }
}