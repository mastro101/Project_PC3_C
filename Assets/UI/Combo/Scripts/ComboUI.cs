using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboUI : MonoBehaviour
{
    [SerializeField] Image comboIconImage;
    [SerializeField] Image cooldownBarImage;
    [SerializeField] Image expBarImage;

    SetSequences combo;
    float deltaCooldown, deltaExp;

    void SubscribeEvent()
    {
        combo.onAddExp         += SetExpBar;
        combo.onCooldownChange += SetCooldownBar;
    }

    void UnsubscribeEvent()
    {
        combo.onAddExp         -= SetExpBar;
        combo.onCooldownChange -= SetCooldownBar;
    }

    public void SetCombo(SetSequences _combo)
    {
        if (combo != null)
            UnsubscribeEvent();
        combo = _combo;
        SubscribeEvent();
        comboIconImage.sprite = combo.data.icon;
        deltaCooldown = 1f / (float)combo.data.cooldown;
        if (combo.levelMaxed)
            deltaExp = 1;
        else
            deltaExp = 1f / (float)combo.data.combosData[combo.level].NecessaryExp;

        SetExpBar();
    }

    public void SetCooldownBar(float _time)
    {
        cooldownBarImage.fillAmount = 1 - (deltaCooldown * _time);
    }

    public void SetExpBar()
    {
        expBarImage.fillAmount = deltaExp * (float)combo.exp;
    }
}