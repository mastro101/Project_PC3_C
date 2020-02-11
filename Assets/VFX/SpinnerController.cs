using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinnerController : MonoBehaviour
{
    ParticleSystem ps;
    ParticleSystemRenderer psr;
    ParticleSystem.VelocityOverLifetimeModule vel;
    float speedVariation = 0.0f;
    public int acceleration = 1;
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        psr = ps.GetComponent<ParticleSystemRenderer>();
        vel = ps.velocityOverLifetime;
        vel.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Horizontal") > 0 && speedVariation <= 2.5f)
        {
            speedVariation += Time.deltaTime * acceleration;
            vel.orbitalY = new ParticleSystem.MinMaxCurve(speedVariation);
        }else if (Input.GetAxis("Horizontal") < 0 && speedVariation >= -2.5f)
        {
            speedVariation -= Time.deltaTime * acceleration;
            vel.orbitalY = new ParticleSystem.MinMaxCurve(speedVariation);
        }
        if (Input.GetAxis("Vertical") > 0)
        {
            psr.sortingOrder = 6;
        }else if (Input.GetAxis("Vertical") < 0)
        {
            psr.sortingOrder = 4;
        }
    }
}
