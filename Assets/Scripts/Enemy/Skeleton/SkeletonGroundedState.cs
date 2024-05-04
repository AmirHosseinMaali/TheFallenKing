using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonGroundedState : EnemyState
{
    protected Skeleton enemy;
    public SkeletonGroundedState(EnemyStateMachine _stateMachine, Enemy _enemyBase, string _animBoolName, Skeleton _enemy) : base(_stateMachine, _enemyBase, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if(enemy.IsPlayerDetected())
        {
            stateMachine.ChangeState(enemy.battleState);
        }
    }
}
