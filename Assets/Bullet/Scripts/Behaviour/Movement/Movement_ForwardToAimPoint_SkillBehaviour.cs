using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[HelpURL("https://docs.google.com/document/d/1likHtetpfsZksF92lwV5sW-NUmKy3_Zl1zG15DzDZ6k/edit?usp=sharing")]
public class Movement_ForwardToAimPoint_SkillBehaviour : BaseSkillBehaviour
{
    [SerializeField] float speed;

    Vector3 direction;

    protected override void OnShoot()
    {
        base.OnShoot();
        direction = skill.shooter.aimDirection;
    }

    protected override void Tick()
    {
        base.Tick();
        skill.transform.position += direction.normalized * Time.deltaTime * speed;
    }
}