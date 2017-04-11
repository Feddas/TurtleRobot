using UnityEngine;
using System.Collections;
using System;

public class CollideTurtle : MonoBehaviour
{
    public event EventHandler<EventArgs> GemReached;

    public ErrorText ErrorText;

    void Start()
    {
    }

    void Update()
    {
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("turtle collided with " + collision.gameObject.name);

        // Test if hit a Gem
        var gem = collision.gameObject.GetComponent<Gem>();
        if (gem != null)
        {
            onWin(gem);

            if (this.GemReached != null)
                this.GemReached(this, new EventArgs());
        }
    }

    void onWin(Gem winningGem)
    {
        winningGem.ShootFireworks();
        StartCoroutine(ReloadLevel(8));
    }

    private IEnumerator ReloadLevel(float inSeconds)
    {
        yield return new WaitForSeconds(inSeconds);

        try
        {
            // use the new Application.LoadLevel(Application.loadedLevel);
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetSceneAt(0).buildIndex,
                UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
        catch (Exception e)
        {
            ErrorText.SetException(e);
        }
    }
}
