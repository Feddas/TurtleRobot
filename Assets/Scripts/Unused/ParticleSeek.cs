using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// optimized using suggestions from https://youtu.be/R3-mKJZD2Ss
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class ParticleSeek : MonoBehaviour
{
    [Tooltip("Gravity well particles will be affected by")]
    public Transform Target;

    [Tooltip("Amount of gravity towards the target")]
    public float Force = 10f;

    ParticleSystem cachedParticleSystem;
    ParticleSystem.Particle[] particles;
    ParticleSystem.MainModule mainModule;

    void Start()
    {
        cachedParticleSystem = this.GetComponent<ParticleSystem>();
        mainModule = cachedParticleSystem.main;
    }

    void LateUpdate()
    {
        if (particles == null || particles.Length < mainModule.maxParticles)
        {
            particles = new ParticleSystem.Particle[mainModule.maxParticles];
        }

        cachedParticleSystem.GetParticles(particles);

        float forceDeltaTime = Force * Time.deltaTime;
        Vector3 targetPosition = Target.position;

        Vector3 targetransformedPosition;
        switch (mainModule.simulationSpace)
        {
            case ParticleSystemSimulationSpace.Local:
                targetransformedPosition = this.transform.InverseTransformPoint(Target.position);
                break;
            case ParticleSystemSimulationSpace.Custom:
                targetransformedPosition = mainModule.customSimulationSpace.InverseTransformPoint(Target.position);
                break;
            case ParticleSystemSimulationSpace.World:
                targetransformedPosition = Target.position;
                break;
            default:
                throw new System.NotSupportedException(mainModule.simulationSpace.ToString() + " is an unsupported simulation space");
        }

        for (int i = 0; i < cachedParticleSystem.particleCount; i++)
        {
            Vector3 directionToTarget = Vector3.Normalize(targetransformedPosition - particles[i].position);
            Vector3 seekForce = directionToTarget * forceDeltaTime;
            particles[i].velocity += seekForce;
        }

        cachedParticleSystem.SetParticles(particles, particles.Length);
    }
}
