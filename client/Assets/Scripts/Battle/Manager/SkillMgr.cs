/*-----------------------------------------------------
    文件：SkillMgr.cs
	作者：Johnson
    日期：2023/8/5 23:49:40
	功能：技能管理器
------------------------------------------------------*/

using System.Collections.Generic;
using UnityEngine;

public class SkillMgr : MonoBehaviour {
    private ResSvc resSvc;
    private TimerSvc timerSvc;

    public void Init() {
        resSvc = ResSvc.Instance;
        timerSvc = TimerSvc.Instance;
        PECommon.Log("Init SkillMgr Done.");

    }

    public void SkillAttack(EntityBase entity, int skillID) {
        entity.skMoveCBLst.Clear();
        entity.skActionCBLst.Clear();

        //技能伤害运算
        AttackDamage(entity, skillID);
        //技能效果表现
        AttackEffect(entity, skillID);
    }

    /// <summary>
    /// 技能伤害运算
    /// </summary>
    public void AttackDamage(EntityBase entity, int skillID) {
        SkillCfg skillData = resSvc.GetSkillCfg(skillID);
        List<int> actionLst = skillData.skillActionLst;

        int sum = 0;
        for (int i = 0; i < actionLst.Count; i++) {
            SkillActionCfg skillAction = resSvc.GetSkillActionCfg(actionLst[i]);
            sum += skillAction.delayTime;
            int index = i;
            if (sum > 0) {
                int actid = timerSvc.AddTimeTask((int tid) => {
                    if (entity != null) {
                        SkillAction(entity, skillData, index);
                        entity.RmvActionCB(tid);
                    }
                }, sum);
                entity.skActionCBLst.Add(actid);
            }
            else {
                //瞬时技能
                SkillAction(entity, skillData, index);
            }
        }
    }

    public void SkillAction(EntityBase caster, SkillCfg skillCfg, int index) {
        SkillActionCfg skillActionCfg = resSvc.GetSkillActionCfg(skillCfg.skillActionLst[index]);

        int damage = skillCfg.skillDamageLst[index];
        if(caster.entityType == EntityType.Monster) {
            EntityPlayer target = caster.battleMgr.entitySelfPlayer;
            if (target == null) {
                return;
            }
            //判断距离，判断角度
            if (InRange(caster.GetPos(), target.GetPos(), skillActionCfg.radius)
                && InAngle(caster.GetTrans(), target.GetPos(), skillActionCfg.angle)) {
                //计算伤害
                CalcDamage(caster, target, skillCfg, damage);
            }
        }
        else if(caster.entityType == EntityType.Player) {
            //获取场景里所有怪物的实体，遍历运算
            List<EntityMonster> monsterLst = caster.battleMgr.GetEntityMonsters();
            for (int i = 0; i < monsterLst.Count; i++) {
                EntityMonster em = monsterLst[i];
                //判断距离，判断角度
                if(InRange(caster.GetPos(), em.GetPos(), skillActionCfg.radius)
                    && InAngle(caster.GetTrans(), em.GetPos(), skillActionCfg.angle)) {
                    //计算伤害
                    CalcDamage(caster, em, skillCfg, damage);
                }
            }
        }
    }

    System.Random rd = new System.Random();
    private void CalcDamage(EntityBase caster, EntityBase target, SkillCfg skillCfg, int damage) {
        int dmgSum = damage;
        if(skillCfg.dmgType == DamageType.AD) {
            /*//计算闪避
            int dodgeNum = PETools.RDInt(1, 100, rd);
            if(dodgeNum <= target.Props.dodge) {
                //UI显示闪避
                //PECommon.Log("闪避Rate: " + dodgeNum + "/" + target.Props.dodge);
                target.SetDodge();
                return;
            }
            //计算属性加成*/
            dmgSum += caster.Props.ad;
            //计算暴击
            int criticalNum = PETools.RDInt(1, 100, rd);
            if (criticalNum <= caster.Props.critical) {
                //计算暴击后的伤害
                float criticalRate = 1 + (PETools.RDInt(1, 100, rd) / 100.0f);
                dmgSum = (int)criticalRate * dmgSum;
                //PECommon.Log("暴击Rate: " + criticalNum + "/" + caster.Props.critical);
                target.SetCritical(dmgSum);
            }
            //计算穿甲
            int addef = (int)((1 - caster.Props.pierce / 100.0f) * target.Props.addef);
            dmgSum -= addef;
        }
        else if(skillCfg.dmgType == DamageType.AP) {
            //计算属性加成
            dmgSum += caster.Props.ap;
            //计算魔法抗性
            dmgSum -= target.Props.apdef;
        }
        else {

        }

        //最终伤害
        if (dmgSum < 0) {
            dmgSum = 0;
            return;
        }
        target.SetHurt(dmgSum);

        if (target.HP < dmgSum) {
            target.HP = 0;
            //目标死亡
            target.Die();
            if (target.entityType == EntityType.Monster) {
                target.battleMgr.RmvMonster(target.Name);
            }
            else if(target.entityType == EntityType.Player) {
                target.battleMgr.EndBattle(false, 0);
                target.battleMgr.entitySelfPlayer = null;
            }
        }
        else {
            target.HP -= dmgSum;
            if (target.entityState == EntityState.None && target.GetBreakState()) {
                target.Hit();
            }
        }
    }

    private bool InRange(Vector3 from, Vector3 to, float range) {
        float dis = Vector3.Distance(from, to);
        if(dis <= range) {
            return true;
        }
        return false;
    }

    private bool InAngle(Transform trans, Vector3 to, float angle) {
        if(angle == 360) {
            return true;
        }
        else {
            Vector3 start = trans.forward;
            Vector3 dir = (to - trans.position).normalized;

            float ang = Vector3.Angle(start, dir);

            if (ang <= angle / 2) {
                return true;
            }
            return true;
        }
    }

    /// <summary>
    /// 技能效果表现
    /// </summary>
    public void AttackEffect(EntityBase entity, int skillID) {
        SkillCfg skillData = resSvc.GetSkillCfg(skillID);

        //是否需要忽略碰撞体
        /*if (!skillData.isCollide) {
            //忽略掉刚体碰撞
            Physics.IgnoreLayerCollision(6, 7);
            timerSvc.AddTimeTask((int tid) => {
                Physics.IgnoreLayerCollision(6, 7, false);

            }, skillData.skillTime);
        }*/

        //是玩家才自动锁敌
        if(entity.entityType == EntityType.Player) {
            //if(entity.GetDirInput() == Vector2.zero) {
            //    //搜索最近的怪物
            //    Vector2 dir = entity.CalcTargetDir();
            //    if (dir != Vector2.zero) {
            //        entity.SetAtkRotation(dir, false);
            //    }
            //}
            //else {
                entity.SetAtkRotation(entity.GetDirInput(), true);
            //}
        }

        entity.SetAction(skillData.aniAction);
        //entity.SetFX(skillData.fx, skillData.skillTime);

        CalcSkillMove(entity, skillData);

        entity.canControl = false;  //避免方向键影响技能释放时人物的移动方向
        entity.SetDir(Vector2.zero);    //避免技能释放时方向键带来的额外位移

        if (!skillData.isBreak) {
            entity.entityState = EntityState.BatiState;
        }

        entity.skEndCB =  timerSvc.AddTimeTask((int tid) => {
            entity.Idle();

        }, skillData.skillTime);
    }

    private void CalcSkillMove(EntityBase entity, SkillCfg skillData) {
        List<int> skillMoveLst = skillData.skillMoveLst;

        int sum = 0;
        for (int i = 0; i < skillMoveLst.Count; i++) {
            SkillMoveCfg skillMoveCfg = resSvc.GetSkillMoveCfg(skillData.skillMoveLst[i]);
            float speed = skillMoveCfg.moveDis / (skillMoveCfg.moveTime / 1000f);
            sum += skillMoveCfg.delayTime;
            if (sum > 0) {
                int moveid = timerSvc.AddTimeTask((int tid) => {
                    entity.SetSkillMoveState(true, speed);
                    entity.RmvMoveCB(tid);
                }, sum);

                entity.skMoveCBLst.Add(moveid);
            }
            else {
                entity.SetSkillMoveState(true, speed);
            }

            sum += skillMoveCfg.moveTime;
            int stopid = timerSvc.AddTimeTask((int tid) => {
                entity.SetSkillMoveState(false);
                entity.RmvMoveCB(tid);
            }, sum);
            entity.skMoveCBLst.Add(stopid);
        }
    }
}

