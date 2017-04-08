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
    [Help("Duration of burst is a combination of ParticleSystems duration and it's start lifetime", UnityEditor.MessageType.None)]

    [Tooltip("Gravity well particles will be affected by")]
    public Transform Target;

    [Tooltip("Amount of gravity towards the target")]
    public float Force = 10f;

    public AnimationCurve ForceOverBurst;

    float timeLeftToBurst;
    float currentForce;
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
        // ensure particles are not reset after the final .Clear()
        if (timeLeftToBurst < 0)
            return;

        if (particles == null || particles.Length < mainModule.maxParticles)
        {
            particles = new ParticleSystem.Particle[mainModule.maxParticles];
        }

        cachedParticleSystem.GetParticles(particles);

        float forceDeltaTime = currentForce * Time.deltaTime;
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

    [ContextMenu("Burst")]
    public void Burst()
    {
        // prep for new burst
        particles = null;
        cachedParticleSystem.Stop();
        cachedParticleSystem.Clear();
        cachedParticleSystem.Play();

        // run new burst
        StopAllCoroutines();
        StartCoroutine(dropForce());
    }

    IEnumerator dropForce()
    {
        float burstLifetime;

        if (cachedParticleSystem.emission.rateOverTime.constantMax == 0)
        {
            // determine when the last burst particle will die
            ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[cachedParticleSystem.emission.burstCount];
            cachedParticleSystem.emission.GetBursts(bursts);
            float lastBurst = bursts.Max(b => b.time);
            burstLifetime = lastBurst + mainModule.startLifetime.constantMax;
        }
        else
        {
            // determine when the last emitted particle will die
            burstLifetime = mainModule.duration + mainModule.startLifetime.constantMax;
        }

        timeLeftToBurst = burstLifetime;

        while (timeLeftToBurst > 0)
        {
            timeLeftToBurst -= Time.deltaTime;
            currentForce = Mathf.Lerp(0, Force, ForceOverBurst.Evaluate(1 - (timeLeftToBurst / burstLifetime)));
            yield return null;
        }

        cachedParticleSystem.Stop();
        cachedParticleSystem.Clear();
    }
}
