using UnityEngine;

public class PlayerAimSwordState : PlayerState
{
    public PlayerAimSwordState(PlayerStateMachine _stateMachine, Player _player, string _animBoolName) : base(_stateMachine, _player, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.skill.sword.DotsActive(true);
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine(player.BusyFor(.2f));
    }

    public override void Update()
    {
        base.Update();

        player.SetZeroVelocity();

        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            stateMachine.ChangeState(player.idleState);
        }

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (player.transform.position.x > mousePosition.x && player.facingDir == 1) { player.Flip(); }
        else if (player.transform.position.x < mousePosition.x && player.facingDir == -1) { player.Flip(); }
    }
}
