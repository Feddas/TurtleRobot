using UnityEngine;
using System.Collections;

public class Gem : MonoBehaviour
{
    [Tooltip("degrees the gem rotates every second")]
    public float rotationAmount = 30.0f;
    public int PlacementRange;
    public ParticleSystem Fireworks;

    void Start()
    {
        if (PlacementRange == 0)
            PlacementRange = 2;

        // randomly place gem
        this.transform.localPosition = RandomIntDistance(PlacementRange);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(0, 0, rotationAmount * Time.deltaTime);
    }

    public void ShootFireworks()
    {
        Fireworks.Play();
    }

    /// <summary>
    /// A random interger snapped distance from 0,0 that can be anywhere between
    /// (-distanceFromZero, -distanceFromZero) to (distanceFromZero, distanceFromZero)
    /// </summary>
    /// <param name="distanceFromZero"></param>
    /// <returns></returns>
    Vector3 RandomIntDistance(int distanceFromZero)
    {
        Vector3 randomPosition;
        do
        {
            randomPosition = new Vector3(
                Random.Range(-distanceFromZero, distanceFromZero + 1),
                Random.Range(-distanceFromZero, distanceFromZero + 1),
                0);
        } while (randomPosition == Vector3.zero);

        return randomPosition;
    }
}
