using System;
using System.Collections.Generic;
using DSS.Common;
using Godot;

namespace DSS.Game;

public class InputManager
{
    private static readonly Dictionary<string, Enums.Direction9> DirectionInputActions = new()
    {
        {"Stall", Enums.Direction9.Neutral},
        {"UpLeft", Enums.Direction9.UpLeft},
        {"UpRight", Enums.Direction9.UpRight},
        {"DownLeft", Enums.Direction9.DownLeft},
        {"DownRight", Enums.Direction9.DownRight},
        {"Right", Enums.Direction9.Right},
        {"Left", Enums.Direction9.Left},
        {"Up", Enums.Direction9.Up},
        {"Down", Enums.Direction9.Down},
    };
    
    public Action<Enums.Direction9> OnDirectionInput;
    public Action<Enums.Direction9> OnHoldDirectionInput;
    public Action<Enums.ActionCode> OnActionPressed;
    
    protected Dictionary<Enums.Direction9, ulong> HoldDue = new();

    public void Init()
    {
        
    }

    public void OnInput(InputEvent @event)
    {
        HandleDirectionPressed(@event);
    }

    public void OnPhysicsProcess()
    {
        // HandleDirectionHold();
    }

    private void HandleDirectionPressed(InputEvent @event)
    {
        foreach (var (eventName, dir) in DirectionInputActions)
        {
            if (@event.IsActionPressed(eventName))
            {
                OnDirectionInput?.Invoke(dir);
                HoldDue[dir] = Time.GetTicksMsec() + (ulong)Constants.HoldTime;
            }

            if (@event.IsActionReleased(eventName))
            {
                HoldDue[dir] = 0;
            }
        }
    }

    private void HandleDirectionHold()
    {
        var currTime = Time.GetTicksMsec();
        foreach (var (dir, dueTime) in HoldDue)
        {
            if (dueTime != 0 && dueTime < currTime)
            {
                OnHoldDirectionInput?.Invoke(dir);
            }
        }
    }
}