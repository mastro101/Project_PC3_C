using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinnerController : MonoBehaviour
{
    ParticleSystem ps;
    ParticleSystem.VelocityOverLifetimeModule vel;
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        vel = ps.velocityOverLifetime;
        vel.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Horizontal") > 0)
        {
            vel.orbitalY = new ParticleSystem.MinMaxCurve(2.5f);
        }else if (Input.GetAxis("Horizontal") < 0)
        {
            vel.orbitalY = new ParticleSystem.MinMaxCurve(-2.5f);
        }
    }
}
