using UnityEngine;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(PlayerStateMachine _stateMachine, Player _player, string _animBoolName) : base(_stateMachine, _player, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.skill.clone.CreateCloneOnDashStart();
        stateTimer = player.dashDuration;
    }

    public override void Exit()
    {
        base.Exit();

        player.skill.clone.CreateCloneOnDashOver();
        player.SetVelocity(0, rb.velocity.y);
    }

    public override void Update()
    {
        base.Update();

        if (!player.IsGroundDetected() && player.IsWallDetected())
        {
            stateMachine.ChangeState(player.wallSlideState);
        }

        rb.velocity = new Vector2(player.dashDir * player.dashSpeed, 0);

        if (stateTimer < 0)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
