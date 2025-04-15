using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidStateMachine : StateMachine<BoidStates, Boid>
{
    public BoidStateMachine(Boid boid)
    {
        _posibleStates = new Dictionary<BoidStates, BaseState<BoidStates, Boid>>
        {
            { BoidStates.FollowingLeader, new FollowingLeaderState().Setup(this).SetAvatar(boid) },
            { BoidStates.SearchingLeader, new SearchingLeaderState().Setup(this).SetAvatar(boid) },
            { BoidStates.AttackingEnemies, new AttackingEnemiesState().Setup(this).SetAvatar(boid) },
            { BoidStates.BoidEscaping, new BoidEscapingState().Setup(this).SetAvatar(boid) },
            { BoidStates.BoidIdle, new BoidIdle().Setup(this).SetAvatar(boid) }
        };

        _currentState = _posibleStates[BoidStates.FollowingLeader];
        _currentState.OnEnter();
    }
}
