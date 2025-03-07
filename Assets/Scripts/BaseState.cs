using UnityEngine;

public class BaseState<T, J> where J : MonoBehaviour
{
    protected StateMachine<T, J> stateMachine;
    protected J avatar;

    public BaseState<T, J> Setup(StateMachine<T, J> newStateMachine)
    {
        stateMachine = newStateMachine;
        return this;
    }

    public BaseState<T, J> SetAvatar(J newAvatar)
    {
        avatar = newAvatar;
        return this;
    }

    public virtual void OnEnter()
    {

    }

    public virtual void OnUpdate()
    {

    }

    public virtual void OnExit()
    {

    }
}
