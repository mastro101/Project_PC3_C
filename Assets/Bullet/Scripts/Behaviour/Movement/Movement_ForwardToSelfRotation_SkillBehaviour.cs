using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_ForwardToSelfRotation_SkillBehaviour : BaseSkillBehaviour
{
    [SerializeField] float speed;

    protected override void Tick()
    {
        base.Tick();
        skill.transform.position += transform.forward * Time.deltaTime * speed;
    }
}