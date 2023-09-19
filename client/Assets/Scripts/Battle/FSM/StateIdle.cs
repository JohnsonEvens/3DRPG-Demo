/*-----------------------------------------------------
    文件：StateIdle.cs
	作者：Johnson
    日期：2023/8/6 22:53:40
	功能：待机状态
------------------------------------------------------*/


using UnityEngine;

public class StateIdle : IState {
    public void Enter(EntityBase entity, params object[] args) {
        entity.currentAniState = AniState.Idle;
        entity.SetDir(Vector2.zero);
        entity.skEndCB = -1;
    }

    public void Exit(EntityBase entity, params object[] args) {
    }

    public void Process(EntityBase entity, params object[] args) {
        if(entity.nextSkillID != 0) {
            entity.Attack(entity.nextSkillID);
        }
        else {
            if(entity.entityType == EntityType.Player) {
                entity.canRlsSkill = true;
            }

            if(entity.GetDirInput() != Vector2.zero) {
                entity.Move();
                entity.SetDir(entity.GetDirInput());
            }
            else {
                entity.SetBlend(Constants.BlendIdle);
            }
        }
        

    }
}

