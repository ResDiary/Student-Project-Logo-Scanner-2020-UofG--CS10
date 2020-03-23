using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Obsolete
/// </summary>
[Obsolete("State switching implementation changed to an inheritance implementation, Use StateContext insead", true)]
[RequireComponent(typeof(DontDestroyOnLoad))]
public class StateManager : ScriptableObject
{

    public enum State
    {
        INITIALISING,
        MAIN_MENU,
        HISTORY,
        CAMERA_SCAN_OFF,
        CAMERA_SCAN_ON,
        RESTAURANT,
    }

    public enum Command
    {
        SHOW_MAIN_MENU,
        SHOW_HISTORY,
        SHOW_CAMERA,
        ENABLE_CAMERA,
        DISABLE_CAMERA,
        SHOW_RESTAURANT
    }

    class StateTransition
    {
        readonly State CurrentState;
        readonly Command Command;

        public StateTransition(State currentState, Command command)
        {
            CurrentState = currentState;
            Command = command;
        }

        public override int GetHashCode()
        {
            return 17 + 31 * CurrentState.GetHashCode() + 31 * Command.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            StateTransition other = obj as StateTransition;
            return other != null && this.CurrentState == other.CurrentState;
        }
    }

    Dictionary<StateTransition, State> transitions;
    public State CurrentState { get; private set; }

    public StateManager()
    {
        CurrentState = State.INITIALISING;
        transitions = new Dictionary<StateTransition, State>
        {
            { new StateTransition(State.INITIALISING, Command.SHOW_MAIN_MENU), State.MAIN_MENU },
            { new StateTransition(State.MAIN_MENU, Command.SHOW_HISTORY), State.HISTORY},
            { new StateTransition(State.HISTORY, Command.SHOW_MAIN_MENU), State.MAIN_MENU },
            { new StateTransition(State.MAIN_MENU, Command.SHOW_CAMERA), State.CAMERA_SCAN_OFF},
            { new StateTransition(State.CAMERA_SCAN_OFF, Command.ENABLE_CAMERA), State.CAMERA_SCAN_ON },
            { new StateTransition(State.CAMERA_SCAN_ON, Command.DISABLE_CAMERA), State.CAMERA_SCAN_OFF },
            { new StateTransition(State.CAMERA_SCAN_ON, Command.SHOW_RESTAURANT), State.RESTAURANT },
            { new StateTransition(State.RESTAURANT, Command.SHOW_MAIN_MENU), State.MAIN_MENU },
            { new StateTransition(State.RESTAURANT, Command.SHOW_CAMERA), State.CAMERA_SCAN_OFF },
            { new StateTransition(State.CAMERA_SCAN_OFF, Command.SHOW_MAIN_MENU), State.MAIN_MENU }
        };
    }

    public State GetNext(Command command)
    {
        StateTransition transition = new StateTransition(CurrentState, command);
        State nextState;
        if (!transitions.TryGetValue(transition, out nextState))
        {
            throw new System.Exception("Invalid Transition: " + CurrentState + " -> " + command);
        }
        return nextState;
    }

    public State MoveNext(Command command)
    {
        CurrentState = GetNext(command);
        return CurrentState;
    }
}
