using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboUIInstantier : MonoBehaviour
{
    [SerializeField] PlayerControllerInput player;
    [SerializeField] Transform parentTransform;
    [Space]
    [SerializeField] ComboUI comboUIPrefab;

    private void Start()
    {
        foreach (var combo in player.sequences)
        {
            InstantiateComboUI(combo);
        }
    }

    void InstantiateComboUI(SetSequences _combo)
    {
        ComboUI combo = Instantiate(comboUIPrefab, parentTransform).GetComponent<ComboUI>();
        combo.SetCombo(_combo);
    }
}