/*-----------------------------------------------------
    文件：StateHit.cs
	作者：Johnson
    日期：2023/8/11 19:01:05
	功能：受击状态
------------------------------------------------------*/


using UnityEngine;

public class StateHit : IState {
    public void Enter(EntityBase entity, params object[] args) {
        entity.currentAniState = AniState.Hit;

        entity.RmvSkillCB();
    }

    public void Exit(EntityBase entity, params object[] args) {
    }

    public void Process(EntityBase entity, params object[] args) {
        if(entity.entityType == EntityType.Player) {
            entity.canRlsSkill = false;
        }
        entity.SetDir(Vector2.zero);    //受击停下
        entity.SetAction(Constants.ActionHit);

        //播放受击音效
        if (entity.entityType == EntityType.Player) {
            AudioSource charAudio = entity.GetAudio();
            AudioSvc.Instance.PlayCharAudio(Constants.AssassinHit, charAudio);
        }

        TimerSvc.Instance.AddTimeTask((int tid) => {
            entity.SetAction(Constants.ActionDefault);
            entity.Idle();
        }, (int)(GetHitAniLen(entity)*1000));
    }

    private float GetHitAniLen(EntityBase entity) {
        AnimationClip[] clips = entity.GetAniClips();
        for (int i = 0; i < clips.Length; i++) {
            string clipName = clips[i].name;
            if(clipName.Contains("hit") ||
                clipName.Contains("Hit") ||
                clipName.Contains("HIT")) {
                return clips[i].length;
            }
        }
        //保护值
        return 1;
    }
}
