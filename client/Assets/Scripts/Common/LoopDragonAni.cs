/*-----------------------------------------------------
    文件：LoopDragonAni.cs
	作者：Johnson
    日期：2023/5/26 17:54:12
	功能：飞龙循环动画
------------------------------------------------------*/

using UnityEngine;

public class LoopDragonAni : MonoBehaviour {
    private Animation ani;

    private void Awake() {
        ani = transform.GetComponent<Animation>();
    }

    private void Start() {
        if(ani != null) {
            InvokeRepeating("PlayDragonAni", 0, 20); //方法名，运行到此时等待多久执行，重复的频率
        }
    }

    private void PlayDragonAni() {
        if(ani != null) {
            ani.Play();
        }
    }
}