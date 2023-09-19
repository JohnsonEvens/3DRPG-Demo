/*-----------------------------------------------------
    文件：AnimationDuration.cs
	作者：Johnson
    日期：2023/8/22 17:24:8
	功能：获取骨骼动画持续时长
------------------------------------------------------*/

using UnityEngine;

public class AnimationDuration : MonoBehaviour {
    public AnimationClip animationClip;

    void Start() {
        if (animationClip != null) {
            float durationSeconds = animationClip.length;
            float durationMilliseconds = durationSeconds * 1000;
            Debug.Log("Animation Duration (ms): " + durationMilliseconds);
        }
        else {
            Debug.LogError("Animation Clip not assigned!");
        }
    }
}