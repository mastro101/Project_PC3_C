using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSequences
{
    public SetSequencesData data { get; private set; }
    [HideInInspector] public int exp;
    [HideInInspector] public int level;

    IShooter controller;
    PlayerControllerInput playerController;

    public IEnumerator cooldownCorutine;

    public List<CommandSequence> commands;
    public CommandSequence currentSection { get; private set; }
    public System.Action<SetSequences> onStartSequence;
    public System.Action<SetSequences> onCompletedSection;
    public System.Action<SetSequences> onCompletedSet;
    public System.Action<SetSequences> onResetSequence;
    public System.Action<SetSequences> onWrongInput;
    public System.Action<SetSequences> onExecute;

    public bool canExecute { get; private set; }

    int currentSectionIndex = 0;
    bool completed = false;

    public SetSequences(SetSequencesData data, PlayerControllerInput playerController, IShooter controller)
    {
        this.data = data;
        this.playerController = playerController;
        this.controller = controller;

        exp = data.startingExp;
        level = 0;
        commands = new List<CommandSequence>();
        foreach (CommandDataList sequences in data.combosData)
        {
            CommandSequence command = new CommandSequence(sequences.comboSection, this.playerController, this.controller, this);
            command.onCompletedSequence += NextSection;
            command.onWrongInput += RemoveSequence;
            if (exp >= sequences.NecessaryExp)
                level++;
            commands.Add(command);
        }

        commands[0].onStartSequence += StartSection;

        this.playerController.OnInputPressed += CheckPressedInput;
        this.controller.OnDestroy += UnsubscribeEvent;

        currentSection = commands[0];
        canExecute = true;

        cooldownCorutine = CooldownCorutine();
    }

    void CheckPressedInput(InputData input)
    {
        if (currentSectionIndex < commands.Count)
            commands[currentSectionIndex].CheckPressedInput(input);
    }

    void RemoveSequence()
    {
        ResetSequence();
        playerController.sequencesToRemove.Add(this);
    }

    void UnsubscribeEvent()
    {
        commands[0].onStartSequence -= StartSection;
        foreach (CommandSequence sequences in commands)
        {
            sequences.onCompletedSequence -= NextSection;
            sequences.onWrongInput -= RemoveSequence;
        }
        this.playerController.OnInputPressed -= CheckPressedInput;
        this.controller.OnDestroy -= UnsubscribeEvent;
    }

    void StartSection(CommandSequence sequence)
    {
        onStartSequence?.Invoke(this);
    }

    void NextSection(CommandSequence sequence)
    {
        currentSection = commands[currentSectionIndex];
        onCompletedSection?.Invoke(this);
        currentSectionIndex++;
        if (currentSectionIndex == level)
        {
            completed = true;
            onCompletedSet?.Invoke(this);
        }
    }

    public void ResetSequence()
    {
        foreach (CommandSequence sequence in commands)
        {
            sequence.ResetSequence();
        }
        completed = false;
        currentSectionIndex = 0;
        onResetSequence?.Invoke(this);
    }

    public void HandleSetSequences()
    {
        if (currentSectionIndex < data.combosData.Length && currentSectionIndex < level && !completed)
        {
            commands[currentSectionIndex].HandleInputSequence();
        }
    }

    public void Execute()
    {
        currentSection.Execute();
        canExecute = false;
        onExecute?.Invoke(this);
    }

    public void RestartCooldownCorutine()
    {
        cooldownCorutine = CooldownCorutine();
    }

    public void OnCooldownEnd()
    {
        canExecute = true;
    }

    IEnumerator CooldownCorutine()
    {
        yield return new WaitForSeconds(data.cooldown);
        OnCooldownEnd();
        yield return null;
    }

    public void AddExp(int _exp)
    {
        exp += _exp;
        if (exp < 0)
            exp = 0;
        Debug.Log(exp);
        if (data.combosData[level - 1].NecessaryExp >= exp)
        {
            level++;
        }
    }
}