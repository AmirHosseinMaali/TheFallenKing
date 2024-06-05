using Unity.VisualScripting;
using UnityEngine;

public class PlayerBlackholeState : PlayerState
{
    private float flyTime = .4f;
    private bool SkillUsed;
    private float defaultGravity;

    public PlayerBlackholeState(PlayerStateMachine _stateMachine, Player _player, string _animBoolName) : base(_stateMachine, _player, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        defaultGravity = player.rb.gravityScale;

        SkillUsed = false;
        stateTimer = flyTime;
        rb.gravityScale = 0;
    }

    public override void Exit()
    {
        base.Exit();

        rb.gravityScale = defaultGravity;
        player.fx.MakeTransparent(false);
    }

    public override void Update()
    {
        base.Update();
        if (stateTimer > 0)
        {
            rb.velocity = new Vector2(0, 15);
        }
        if (stateTimer < 0)
        {
            rb.velocity = new Vector2(0, -.1f);
            if (!SkillUsed)
            {
                if (player.skill.blackhole.CanUseSkill())
                {
                    SkillUsed = true;
                }
            }
        }
        if (player.skill.blackhole.SkillCompleted())
        {
            stateMachine.ChangeState(player.airState);
        }
    }
}
