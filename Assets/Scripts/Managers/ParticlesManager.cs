using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static ParticleSystem InstantiateParticule(ParticleSystem particles, Vector3 position)
    {
        ParticleSystem newParticles = Instantiate(particles);

        newParticles.transform.localPosition = position;

        Destroy(newParticles.gameObject, newParticles.main.startLifetime.constant);

        return newParticles;
    }
}
