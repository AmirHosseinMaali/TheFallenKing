using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonBattleState : EnemyState
{
    private Transform player;
    private Skeleton enemy;
    private int moveDir;

    public SkeletonBattleState(EnemyStateMachine _stateMachine, Enemy _enemyBase, string _animBoolName, Skeleton _enemy) : base(_stateMachine, _enemyBase, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        player = GameObject.Find("Player").transform;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if(enemy.IsPlayerDetected().distance<enemy.attackDistance)
        {
            Debug.Log("Attack");
            enemy.ZeroVelocity();
            return;
        }

        if(player.position.x>enemy.transform.position.x)
        {
            moveDir = 1;
        }
        else if(player.position.x < enemy.transform.position.x)
        {
            moveDir = -1;
        }
        enemy.SetVelocity(moveDir * enemy.moveSpeed, rb.velocity.y);
    }
}
