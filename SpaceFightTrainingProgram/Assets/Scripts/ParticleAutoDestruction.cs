using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAutoDestruction : MonoBehaviour
{
    ParticleSystem[] particles;
    // Start is called before the first frame update
    void Start()
    {
        particles = GetComponentsInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        bool noParticleAlive = true;
        foreach (ParticleSystem particle in particles) {
            if(particle.IsAlive())
            {
                noParticleAlive = false;
                break;
            }
        }
        if (noParticleAlive) {
            Destroy(gameObject);
        }
    }
}
