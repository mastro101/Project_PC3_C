using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBullet : BulletBase
{

    public override void Shoot(Vector3 shootPosition, Vector3 direction, IShooter _shooter, CommandSequence _command)
    {
        base.Shoot(shootPosition, direction, _shooter, _command);
        vfx.Play();
    }

    public override void Return()
    {
        base.Return();
        vfx.Stop();
    }
}
