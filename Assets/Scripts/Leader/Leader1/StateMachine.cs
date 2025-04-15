using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T, J> where J : MonoBehaviour
{
    public Dictionary<T, BaseState<T, J>> _posibleStates = new Dictionary<T, BaseState<T, J>>();
    protected BaseState<T, J> _currentState;

    public void ChangeState(T newState)
    {
        if (_currentState != null)
        {
            _currentState.OnExit();
        }

        _currentState = _posibleStates[newState];
        _currentState.OnEnter();
    }

    public void OnUpdate()
    {
        _currentState.OnUpdate();
    }
}

