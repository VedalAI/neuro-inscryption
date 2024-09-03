using System;
using System.Collections.Generic;

namespace NeuroInscryption.API.Input;

internal sealed class ClickHandler
{
    private enum ClickState
    {
        None,
        Down,
        Hold,
        Up,
    }

    private readonly Queue<bool> _clickQueue = [];
    private ClickState _state = ClickState.None;

    public bool Get() => _state is ClickState.Down or ClickState.Hold;
    public bool GetDown() => _state is ClickState.Down;
    public bool GetUp() => _state is ClickState.Up;

    public void Enqueue(bool down)
    {
        _clickQueue.Enqueue(down);
    }

    public void Update()
    {
        if (_clickQueue.Count > 0)
        {
            if (_clickQueue.Dequeue())
            {
                // Want to press down
                _state = _state switch
                {
                    ClickState.None => ClickState.Down,
                    ClickState.Down => ClickState.Hold,
                    ClickState.Hold => ClickState.Down,
                    ClickState.Up => ClickState.Down,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            else
            {
                // Want to let go
                _state = _state switch
                {
                    ClickState.None => ClickState.None,
                    ClickState.Down => ClickState.Up,
                    ClickState.Hold => ClickState.Up,
                    ClickState.Up => ClickState.None,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
        else
        {
            // No input
            _state = _state switch
            {
                ClickState.None => ClickState.None,
                ClickState.Down => ClickState.Hold,
                ClickState.Hold => ClickState.Hold,
                ClickState.Up => ClickState.None,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
