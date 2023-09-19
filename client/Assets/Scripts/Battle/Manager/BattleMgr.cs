/*-----------------------------------------------------
    文件：BattleMgr.cs
	作者：Johnson
    日期：2023/8/5 23:45:40
	功能：战场管理器
------------------------------------------------------*/


using PEProtocol;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleMgr : MonoBehaviour {
    private ResSvc resSvc;
    private AudioSvc audioSvc;

    private StateMgr stateMgr;
    private SkillMgr skillMgr;
    private MapMgr mapMgr;

    public EntityPlayer entitySelfPlayer;
    private MapCfg mapCfg;

    private Dictionary<string, EntityMonster> monsterDic = new Dictionary<string, EntityMonster>();
    public void Init(int mapid, Action cb = null) {

        resSvc = ResSvc.Instance;
        audioSvc = AudioSvc.Instance;

        //初始化各管理器
        stateMgr = gameObject.AddComponent<StateMgr>();
        stateMgr.Init();
        skillMgr = gameObject.AddComponent<SkillMgr>();
        skillMgr.Init();

        //加载战斗地图
        mapCfg = resSvc.GetMapCfg(mapid);
        resSvc.AsyncLoadScene(mapCfg.sceneName, () => {
            //回调，初始化地图数据
            GameObject map = GameObject.FindGameObjectWithTag("MapRoot");
            mapMgr =  map.GetComponent<MapMgr>();
            mapMgr.Init(this);

            map.transform.localPosition = Vector3.zero;
            map.transform.localScale = Vector3.one;

            Camera.main.transform.position = mapCfg.mainCamPos;
            Camera.main.transform.localEulerAngles = mapCfg.mainCamRote;

            LoadPlayer(mapCfg);
            entitySelfPlayer.Idle();

            //指定主摄像机需要围绕的对象
            GameObject go = GameObject.FindGameObjectWithTag("Player");
            Camera.main.GetComponent<CameraRotateAround>().CenObj = go.transform;
            //Camera.main.transform.parent = go.transform;
            //Camera.main.GetComponent<CameraRotateAround>().Rotion_Transform = go.transform.position;

            //激活第一批怪物
            ActiveCurrentBatchMonsters();

            audioSvc.PlayBGMusic(Constants.BGDesertedVillage);
            if (cb != null) {
                cb();
            }
        });
    }

    public bool triggerCheck = true;
    public bool isPauseGame = false;
    public void Update() {
        foreach (var item in monsterDic) {
            EntityMonster em = item.Value;
            em.TickAILogic();
        }

        //检测当前批次怪物是否全部死亡
        if (mapMgr != null) {
            if (triggerCheck && monsterDic.Count == 0) {
                bool isExist = mapMgr.SetNextTriggerOn();
                triggerCheck = false;
                if (!isExist) {
                    //副本结束，战斗胜利，结算
                    EndBattle(true, entitySelfPlayer.HP);
                }
            }
        }
    }

    public void EndBattle(bool isWin, int restHP) {
        isPauseGame = true;
        //audioSvc.StopBGMusic();
        BattleSys.Instance.EndBattle(isWin, restHP);
    }

    private void LoadPlayer(MapCfg mapData) {
        GameObject player = resSvc.LoadPrefab(PathDefine.AssissnBattlePlayerPrefab);

        player.transform.position = mapData.playerBornPos;
        player.transform.localEulerAngles = mapData.playerBornRote;
        player.transform.localScale = Vector3.one;

        PlayerData pd = GameRoot.Instance.PlayerData;
        BattleProps props = new BattleProps {
            hp = pd.hp,
            ad = pd.ad,
            ap = pd.ap,
            addef = pd.addef,
            apdef = pd.apdef,
            dodge = pd.dodge,
            pierce = pd.pierce,
            critical = pd.critical,

        };

        entitySelfPlayer = new EntityPlayer {
            battleMgr = this,
            stateMgr = stateMgr,
            skillMgr = skillMgr,
        };
        entitySelfPlayer.Name = "AssassinBattle";
        entitySelfPlayer.SetBattleProps(props);

        PlayerController playerCtrl = player.GetComponent<PlayerController>();
        playerCtrl.Init();
        entitySelfPlayer.SetCrtl(playerCtrl);
    }

    public void LoadMonsterByWaveID(int wave) {
        for (int i = 0; i < mapCfg.monsterLst.Count; i++) {
            MonsterData md = mapCfg.monsterLst[i];
            if(md.mWave == wave) {
                GameObject m = resSvc.LoadPrefab(md.mCfg.resPath, true);
                m.transform.localPosition = md.mBornPos;
                m.transform.localEulerAngles = md.mBornRote;
                m.transform.localScale = Vector3.one;

                m.name = "m" + md.mWave + "_" + md.mIndex;
                //生成怪物
                EntityMonster em = new EntityMonster {
                    battleMgr = this,
                    stateMgr = stateMgr,
                    skillMgr = skillMgr,
                };
                //设置初始属性
                em.md = md;
                em.SetBattleProps(md.mCfg.bps);
                em.Name = m.name;

                MonsterController mc = m.GetComponent<MonsterController>();
                mc.Init();
                em.SetCrtl(mc);

                m.SetActive(false);
                monsterDic.Add(m.name, em);
                if(md.mCfg.mType == MonsterType.Normal) {
                    GameRoot.Instance.dynamicWnd.AddHpItemInfo(m.name, mc.hpRoot, em.HP);
                }
                else if(md.mCfg.mType == MonsterType.Boss) {
                    BattleSys.Instance.playerCtrlWnd.SetBossHPBarState(true);
                }
            }
        }
    }

    public void ActiveCurrentBatchMonsters() {
        TimerSvc.Instance.AddTimeTask((int tid) => {
            foreach (var item in monsterDic) {
                item.Value.SetActive(true);
                item.Value.Born();
                TimerSvc.Instance.AddTimeTask((int id) => {
                    //出生1秒后进入Idle
                    item.Value.Idle();
                }, 1000);
            }
        }, 500);
    }

    public List<EntityMonster> GetEntityMonsters() {
        List<EntityMonster> monsterLst = new List<EntityMonster>();
        foreach (var item in monsterDic) {
            monsterLst.Add(item.Value);
        }
        return monsterLst;
    }

    public void RmvMonster(string key) {
        EntityMonster entityMonster;
        if(monsterDic.TryGetValue(key,out entityMonster)) {
            monsterDic.Remove(key);
            GameRoot.Instance.dynamicWnd.RmvHpItemInfo(key);
        }

    }

    #region 技能释放与角色控制
    public void SetSelfPlayerMoveDir(Vector2 dir) {
        if(entitySelfPlayer != null) {
            //设置玩家移动
            if (entitySelfPlayer.canControl == false) {
                return;
            }

            if(entitySelfPlayer.currentAniState==AniState.Idle || entitySelfPlayer.currentAniState == AniState.Move) {
                if (dir == Vector2.zero) {
                    entitySelfPlayer.Idle();
                }
                else {
                    entitySelfPlayer.Move();
                    entitySelfPlayer.SetDir(dir);

                }
            }
        }
    }
    public void ReqReleaseSkill(int index) {
        switch (index) {
            case 0:
                ReleaseNormalAtk();
                break;
            case 1:
                ReleaseSkill1();
                break;
            case 2:
                ReleaseSkill2();
                break;
            case 3:
                ReleaseSkill3();
                break;

        }
    }

    public double lastAtkTime = 0;
    private int[] comboArr = new int[] { 111, 112, 113, 114, 115 };
    public int comboIndex = 0;
    private void ReleaseNormalAtk() {
        //PECommon.Log("Click Normal Atk");
        if(entitySelfPlayer.currentAniState == AniState.Attack) {
            //在500ms以内进行第二次点击，存数据
            double nowAtkTime = TimerSvc.Instance.GetNowTime();
            if(nowAtkTime - lastAtkTime < Constants.ComboSpace && lastAtkTime != 0 && entitySelfPlayer.comboQue.Count < 4) {
                if (comboArr[comboIndex] != comboArr[comboArr.Length - 1]) {
                    comboIndex += 1;
                    entitySelfPlayer.comboQue.Enqueue(comboArr[comboIndex]);
                    lastAtkTime = nowAtkTime;
                }
            }
        }
        else if(entitySelfPlayer.currentAniState == AniState.Idle || entitySelfPlayer.currentAniState == AniState.Move) {
            comboIndex = 0;
            lastAtkTime = TimerSvc.Instance.GetNowTime();
            entitySelfPlayer.Attack(comboArr[comboIndex]);
        }
    }
    private void ReleaseSkill1() {
        //PECommon.Log("Click Skill 1");
        entitySelfPlayer.Attack(101);
    }
    private void ReleaseSkill2() {
        //PECommon.Log("Click Skill 2");
        entitySelfPlayer.Attack(102);
    }
    private void ReleaseSkill3() {
        //PECommon.Log("Click Skill 3");
        entitySelfPlayer.Attack(103);
    }
    public Vector2 GetDirInput() {
        return BattleSys.Instance.GetDirInput();
    } 

    public bool CanRlsSkill() {
        return entitySelfPlayer.canRlsSkill;
    }
    #endregion
}

