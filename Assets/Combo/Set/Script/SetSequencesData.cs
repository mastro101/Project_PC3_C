using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSet", menuName = "Combo/Set")]
public class SetSequencesData : ScriptableObject
{
    //public CommandSequenceData[] comboSections;
    public CommandDataList[] combosData;
    public int startingExp;
    public float cooldown;
    public Sprite icon;
}

[System.Serializable]
public class CommandDataList
{
    public CommandSequenceData comboSection;
    public int NecessaryExp;
}