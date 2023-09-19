/*-----------------------------------------------------
    文件：TriggerData.cs
	作者：Johnson
    日期：2023/8/13 17:19:21
	功能：地图触发数据类
------------------------------------------------------*/

using UnityEngine;

public class TriggerData : MonoBehaviour {
    public int triggerWave;
    public MapMgr mapMgr;

    public void OnTriggerExit(Collider other) {
        if(other.gameObject.tag == "Player") {
            if (mapMgr != null) {
                mapMgr.TriggerMonsterBorn(this, triggerWave);
            }
        }
    }
}