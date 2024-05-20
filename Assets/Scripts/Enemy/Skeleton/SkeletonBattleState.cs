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

        player = PlayerManager.instance.player.transform;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (enemy.IsPlayerDetected())
        {
            stateTimer = enemy.battleTime;

            if (enemy.IsPlayerDetected().distance < enemy.attackDistance)
            {
                if (CanAttack())
                {
                    stateMachine.ChangeState(enemy.attackState);
                }
            }
        }
        else
        {
            if (stateTimer < 0 || Vector2.Distance(player.position, enemy.transform.position) > 10)
            {
                stateMachine.ChangeState(enemy.idleState);
            }
        }



        if (player.position.x > enemy.transform.position.x)
        {
            moveDir = 1;
        }
        else if (player.position.x < enemy.transform.position.x)
        {
            moveDir = -1;
        }
        enemy.SetVelocity(moveDir * enemy.moveSpeed, rb.velocity.y);
    }
    private bool CanAttack()
    {
        if (Time.time >= enemy.lastTimeAttack + enemy.attackCoolDown)
        {
            enemy.lastTimeAttack = Time.time;
            return true;
        }
        return false;
    }
}
