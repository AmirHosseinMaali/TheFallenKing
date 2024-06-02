using UnityEngine;

public class SkeletonStunnedState : EnemyState
{
    protected Skeleton enemy;
    protected Transform player;

    public SkeletonStunnedState(EnemyStateMachine _stateMachine, Enemy _enemyBase, string _animBoolName, Skeleton _enemy) : base(_stateMachine, _enemyBase, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.fX.InvokeRepeating("RedColorBlink", 0, .1f);

        stateTimer = enemy.stunDuration;

        rb.velocity = new Vector2(-enemy.facingDir * enemy.stunDirection.x, enemy.stunDirection.y);
    }

    public override void Exit()
    {
        base.Exit();
        enemy.fX.Invoke("CancelColorChange",0);
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }
}
