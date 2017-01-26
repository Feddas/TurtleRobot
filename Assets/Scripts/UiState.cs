using UnityEngine;
using System.Collections;

/// <summary>
/// Modifies the UI according to the game state.
/// </summary>
public class UiState : MonoBehaviour
{
    public RunCards Turtle;
    public GameObject StopGraphic;
    public GameObject CardClickBlocker;

    void Start()
    {
        if (Turtle)
            Turtle.FinishedRun += Turtle_FinishedRun;
    }

    void OnDestroy()
    {
        if (Turtle)
            Turtle.FinishedRun -= Turtle_FinishedRun;
    }

    void Turtle_FinishedRun(object sender, System.EventArgs e)
    {
        StopGraphic.SetActive(false);
        CardClickBlocker.SetActive(false);
    }

    public void OnClickRun()
    {
        bool isRunning = Turtle.OnClickRun();

        // show the stop running graphic
        StopGraphic.SetActive(isRunning);

        // enable the click blocker as cards should not be moved while the turtle is running
        CardClickBlocker.SetActive(isRunning);
    }
}
