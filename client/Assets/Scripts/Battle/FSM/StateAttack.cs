/*-----------------------------------------------------
    文件：StateAttack.cs
	作者：Johnson
    日期：2023/8/8 22:30:05
	功能：攻击状态
------------------------------------------------------*/


public class StateAttack : IState {
    public void Enter(EntityBase entity, params object[] args) {
        entity.currentAniState = AniState.Attack;
        entity.curtSkillCfg = ResSvc.Instance.GetSkillCfg((int)args[0]);
    }

    public void Exit(EntityBase entity, params object[] args) {
        entity.ExitCurtSkill();
    }

    public void Process(EntityBase entity, params object[] args) {
        if(entity.entityType == EntityType.Player) {
            entity.canRlsSkill = false;
        }

        entity.SkillAttack((int)args[0]);

    }
}

