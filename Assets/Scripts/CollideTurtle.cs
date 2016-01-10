using UnityEngine;
using System.Collections;

public class CollideTurtle : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
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
        }
    }

    //TODO: refactor onWin into a function in RunCards that's executed when OnCollisionEnter raises a GemReached event
    void onWin(Gem winningGem)
    {
        winningGem.ShootFireworks();
        StartCoroutine(ReloadLevel(3));
    }

    private IEnumerator ReloadLevel(float inSeconds)
    {
        yield return new WaitForSeconds(inSeconds);

        // use the new Application.LoadLevel(Application.loadedLevel);
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetSceneAt(0).buildIndex,
            UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
