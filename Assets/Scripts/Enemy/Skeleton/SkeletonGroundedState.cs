using UnityEngine;

public class SkeletonGroundedState : EnemyState
{
    protected Skeleton enemy;
    protected Transform player;

    public SkeletonGroundedState(EnemyStateMachine _stateMachine, Enemy _enemyBase, string _animBoolName, Skeleton _enemy) : base(_stateMachine, _enemyBase, _animBoolName)
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

        if (enemy.IsPlayerDetected() || Vector2.Distance(enemy.transform.position, player.transform.position) < 5)
        {
            stateMachine.ChangeState(enemy.battleState);
        }
    }
}
