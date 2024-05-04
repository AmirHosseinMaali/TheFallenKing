using UnityEngine;

public class PlayerWallSlideState : PlayerState
{
    public PlayerWallSlideState(PlayerStateMachine _stateMachine, Player _player, string _animBoolName) : base(_stateMachine, _player, _animBoolName)
    {
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            stateMachine.ChangeState(player.wallJumpState);
            return;
        }
        if (xInput != 0 && player.facingDir != xInput)
        {
            stateMachine.ChangeState(player.idleState);
        }
        else if (player.facingDir == xInput && player.IsWallDetected())
        {
            rb.velocity = new Vector2(0, rb.velocity.y * .6f);
        }
        if (player.IsGroundDetected() || !player.IsWallDetected())
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
