using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI for setting the game. It's used to set the initial parameters of the game.
/// </summary>
public class SettingUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField gridSizeX;
    [SerializeField] private TMP_InputField gridSizeY;
    [SerializeField] private TMP_InputField numOfColors;
    [SerializeField] private TMP_Text errorText;

    [SerializeField] private TMP_InputField numOfSimulation;
    [SerializeField] private Toggle withAnimationToggle;


    public void OnButtonStartClicked()
    {
        int x = 0;
        int y = 0;
        int colors = 0;

        try
        {
            x = int.Parse(gridSizeX.text);
            y = int.Parse(gridSizeY.text);
            colors = int.Parse(numOfColors.text);

            if (x < 4 || y < 4 || colors < 3)
            {
                errorText.gameObject.SetActive(true);
                errorText.text = "Invalid input (x and y must be greater than 3, colors must be greater than 2)";
                Debug.Log("Invalid input");
                return;
            }

            if (x > 30 || y > 30 || colors > 10)

            {
                errorText.gameObject.SetActive(true);
                errorText.text = "Invalid input (x and y must be less than 31) and colors must be less than 11)";
                Debug.Log("Invalid input");
                return;
            }

        }
        catch
        {
            errorText.gameObject.SetActive(true);
            errorText.text = "Invalid input";
            Debug.Log("Invalid input");
            return;
        }

        gameObject.SetActive(false);

        // TODO: change to better way
        FindObjectOfType<Scene>().Initialize(new Point(x, y), colors);
    }


    public void OnSimulateButtonClicked()
    {
        int numOfSimulations = 0;
        bool withAnimation = withAnimationToggle.isOn;
        try
        {
            numOfSimulations = int.Parse(numOfSimulation.text);
        }
        catch
        {
            errorText.gameObject.SetActive(true);
            errorText.text = "Invalid input";
            Debug.Log("Invalid input");
            return;
        }

        OnButtonStartClicked();


        // TODO: change to better way
        FindObjectOfType<Simulation>().Initialize(numOfSimulations, withAnimation);

    }
}