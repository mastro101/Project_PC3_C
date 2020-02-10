using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinnerController : MonoBehaviour
{
    ParticleSystem ps;
    ParticleSystem.VelocityOverLifetimeModule vel;
    float speedVariation = 0.0f;
    public int acceleration = 1;
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
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
    }
}
