/*-----------------------------------------------------
    文件：MapMgr.cs
	作者：Johnson
    日期：2023/8/5 23:50:40
	功能：地图管理器
------------------------------------------------------*/

using UnityEngine;

public class MapMgr : MonoBehaviour {
    private int waveIndex = 1;  //默认生成第一波怪物
    private BattleMgr battleMgr;
    public TriggerData[] triggerArr;
    public void Init(BattleMgr battle) {
        battleMgr = battle;

        //实例化第一批怪物
        battleMgr.LoadMonsterByWaveID(waveIndex);

        PECommon.Log("Init MapMgr Done.");
    }

    public void TriggerMonsterBorn(TriggerData trigger, int waveIndex) {
        if (battleMgr != null) {
            BoxCollider co = trigger.gameObject.GetComponent<BoxCollider>();
            co.isTrigger = false;

            battleMgr.LoadMonsterByWaveID(waveIndex);   //加载
            battleMgr.ActiveCurrentBatchMonsters(); //激活
            battleMgr.triggerCheck = true;
        }
    }

    public bool SetNextTriggerOn() {
        waveIndex += 1;
        for (int i = 0; i < triggerArr.Length; i++) {
            if (triggerArr[i].triggerWave == waveIndex) {
                BoxCollider co = triggerArr[i].GetComponent<BoxCollider>();
                co.isTrigger = true;
                return true;
            }
        }

        return false;
    }
}


