/*-----------------------------------------------------
    文件：StateDie.cs
	作者：Johnson
    日期：2023/8/11 18:18:05
	功能：死亡状态
------------------------------------------------------*/


public class StateDie : IState {
    public void Enter(EntityBase entity, params object[] args) {
        entity.currentAniState = AniState.Die;

        entity.RmvSkillCB();
    }

    public void Exit(EntityBase entity, params object[] args) {
    }

    public void Process(EntityBase entity, params object[] args) {
        entity.SetAction(Constants.ActionDie);
        if(entity.entityType == EntityType.Monster) {
            entity.GetCC().enabled = false;
            TimerSvc.Instance.AddTimeTask((int tid) => {
                entity.SetActive(false);
            }, Constants.DieAniLength);
        }
    }
}

