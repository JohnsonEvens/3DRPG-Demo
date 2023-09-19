/*-----------------------------------------------------
    文件：EntityBase.cs
	作者：Johnson
    日期：2023/8/6 22:52:40
	功能：逻辑实体基类
------------------------------------------------------*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityBase {
    public AniState currentAniState = AniState.None;

    public BattleMgr battleMgr = null;
    public StateMgr stateMgr = null;
    public SkillMgr skillMgr = null;
    protected Controller controller = null;
    private string name;
    public string Name { get => name; set => name = value; }

    public bool canControl = true;

    public bool canRlsSkill = true; //能否放技能

    public EntityType entityType = EntityType.None;
    public EntityState entityState = EntityState.None;


    private BattleProps props;
    public BattleProps Props {
        get {
            return props;
        }
        protected set {
            props = value;
        } 
    }

    private int hp;
    public int HP {
        get {
            return hp;
        }
        set {
            //通知UI层
            PECommon.Log("hp changge: " + hp + "to " + value);
            SetHPVal(hp, value);
            hp = value;
        }
    }

    public Queue<int> comboQue = new Queue<int>();
    public int nextSkillID = 0;

    public SkillCfg curtSkillCfg;

    //技能位移的回调ID
    public List<int> skMoveCBLst = new List<int>();

    //技能伤害计算的回调ID
    public List<int> skActionCBLst = new List<int>();

    public int skEndCB = -1;

    public void Born() {
        stateMgr.ChangeState(this, AniState.Born, null);
    }

    public void Move() {
        stateMgr.ChangeState(this, AniState.Move, null);
    }

    public void Idle() {
        stateMgr.ChangeState(this, AniState.Idle, null);
    }

    public void Attack(int skillID) {
        stateMgr.ChangeState(this, AniState.Attack, skillID);
    }

    public void Hit() {
        stateMgr.ChangeState(this, AniState.Hit, null);
    }

    public void Die() {
        stateMgr.ChangeState(this, AniState.Die, null);
    }

    public virtual void TickAILogic() {

    }

    public void SetCrtl(Controller ctrl) {
        controller = ctrl;
    }
    public void SetActive(bool active = true) {
        if (controller != null) {
            controller.gameObject.SetActive(active);
        }
    }

    public virtual void SetBattleProps(BattleProps props) {
        HP = props.hp;
        Props = props;
    }

    public virtual void SetBlend(float blend) {
        if(controller != null) {
            controller.SetBlend(blend);
        }
    }

    public virtual void SetDir(Vector2 dir) {
        if (controller != null) {
            controller.Dir = dir;
        }
    }

    public virtual void SetAction(int act) {
        if (controller != null) {
            controller.SetAction(act);
        }
    }

    public virtual void SetFX(string name, float destroy) {
        if (controller != null) {
            controller.SetFX(name, destroy);
        }
    }
    public virtual void SetSkillMoveState(bool move, float speed = 0f) {
        if (controller != null) {
            controller.SetSkillMoveState(move, speed);
        }
    }
    public virtual void SetAtkRotation(Vector2 dir, bool offset=false) {
        if (controller != null) {
            if (offset) {
                controller.SetAtkRotationCam(dir);
            }
            else {
                controller.SetAtkRotationLocal(dir);
            }
        }
    }

    #region 战斗信息显示
    public virtual void SetDodge() {
        if (controller != null) {
            GameRoot.Instance.dynamicWnd.SetDodge(Name);
        }
    }
    public virtual void SetCritical(int critical) {
        if (controller != null) {
            GameRoot.Instance.dynamicWnd.SetCritical(Name, critical);
        }
    }
    public virtual void SetHurt(int hurt) {
        if (controller != null) {
            GameRoot.Instance.dynamicWnd.SetHurt(Name, hurt);
        }
    }
    public virtual void SetHPVal(int oldVal, int newVal) {
        if (controller != null) {
            GameRoot.Instance.dynamicWnd.SetHPVal(Name, oldVal, newVal);
        }
    } 
    #endregion

    public virtual void SkillAttack(int skillID) {
        skillMgr.SkillAttack(this, skillID);
    }

    public virtual Vector2 GetDirInput() {
        return Vector2.zero;
    }

    public virtual Vector3 GetPos() {
        return controller.transform.position;
    }

    public virtual Transform GetTrans() {
        return controller.transform;
    }

    public AnimationClip[] GetAniClips() {
        if (controller != null) {
            return controller.ani.runtimeAnimatorController.animationClips;
        }
        return null;
    }

    public AudioSource GetAudio() {
        return controller.GetComponent<AudioSource>();
    }

    public CharacterController GetCC() {
        return controller.GetComponent<CharacterController>();
    }

    public virtual bool GetBreakState() {
        return true;
    }

    public virtual Vector2 CalcTargetDir() {
        return Vector2.zero;
    }

    public void ExitCurtSkill() {
        canControl = true;

        if (curtSkillCfg != null) {
            if (!curtSkillCfg.isBreak) {
                entityState = EntityState.None;
            }

            //连招数据更新
            if (curtSkillCfg.isCombo) {
                if (comboQue.Count > 0) {
                    nextSkillID = comboQue.Dequeue();
                }
                else {
                    nextSkillID = 0;
                }
            }
            curtSkillCfg = null;
        }
        SetAction(Constants.ActionDefault);
    }

    public void RmvActionCB(int tid) {
        int index = -1;
        for (int i = 0; i < skActionCBLst.Count; i++) {
            if(skActionCBLst[i] == tid) {
                index = i;
                break;
            }
        }
        if(index != -1) {
            skActionCBLst.RemoveAt(index);
        }
    }

    public void RmvMoveCB(int tid) {
        int index = -1;
        for (int i = 0; i < skMoveCBLst.Count; i++) {
            if (skMoveCBLst[i] == tid) {
                index = i;
                break;
            }
        }
        if (index != -1) {
            skMoveCBLst.RemoveAt(index);
        }
    }

    public void RmvSkillCB() {
        SetDir(Vector2.zero);
        SetSkillMoveState(false);

        //受击需要把位移回调和伤害回调去掉，避免技能释放被打断时还能延迟造成伤害和位移
        for (int i = 0; i < skMoveCBLst.Count; i++) {
            int tid = skMoveCBLst[i];
            TimerSvc.Instance.DelTask(tid);
        }
        for (int i = 0; i < skActionCBLst.Count; i++) {
            int tid = skActionCBLst[i];
            TimerSvc.Instance.DelTask(tid);
        }

        //攻击被中断，删除定时回调
        if (skEndCB != -1) {
            TimerSvc.Instance.DelTask(skEndCB);
            skEndCB = -1;
        }
        skMoveCBLst.Clear();
        skActionCBLst.Clear();

        //清空连招队列
        if (nextSkillID != 0 || comboQue.Count > 0) {
            nextSkillID = 0;
            comboQue.Clear();

            battleMgr.lastAtkTime = 0;
            battleMgr.comboIndex = 0;
        }
    }
}

