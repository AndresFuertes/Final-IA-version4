using System.Collections.Generic;
using UnityEngine;

public enum EnemyStates
{
    Idle,
    Chase,
    Patrol
}
public class StateMachine<T, J> where J : MonoBehaviour
{
    public BaseState<T, J> _actualState;
    public Dictionary<T, BaseState<T, J>> _posibleStates = new Dictionary<T, BaseState<T, J>>();

    public void OnUpdate()
    {
        _actualState?.OnUpdate();
    }

    public void ChangeState(T newState)
    {
        if (!_posibleStates.ContainsKey(newState)) return;

        _actualState?.OnExit();
        _actualState = _posibleStates[newState];
        _actualState.OnEnter();
    }
}
