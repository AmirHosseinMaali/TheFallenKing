using UnityEngine;

public class PlayerCounterAttackState : PlayerState
{
    private bool canCreateClone;

    public PlayerCounterAttackState(PlayerStateMachine _stateMachine, Player _player, string _animBoolName) : base(_stateMachine, _player, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        canCreateClone = true;
        stateTimer = player.counterAttackDuration;

        player.anim.SetBool("SuccessfulCounterAttack", false);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        player.SetZeroVelocity();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && hit.GetComponent<Enemy>().CanBeStunned())
            {
                stateTimer = 10;
                player.anim.SetBool("SuccessfulCounterAttack", true);
                if (canCreateClone)
                {
                    player.skill.clone.CreateCloneOnCounterAttack(hit.transform);
                    canCreateClone = false;
                }
            }
        }

        if (stateTimer < 0 || triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
