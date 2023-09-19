/*-----------------------------------------------------
    文件：DynamicWnd.cs
	作者：Johnson
    日期：2023/5/26 18:11:44
	功能：动态UI元素界面
------------------------------------------------------*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicWnd : WindowRoot {
    public Animation tipsAni;
    public GameObject tipsBg;
    public Text txtTips;
    public Transform hpItemRoot;

    public Animation selfDodgeAni;

    private bool isTipsShow = false;
    private Queue<string> tipsQue = new Queue<string>();
    private Dictionary<string, ItemEntityHP> itemDic = new Dictionary<string, ItemEntityHP>();

    protected override void InitWnd() {
        base.InitWnd();

        SetActive(tipsBg, false);
    }

    #region Tips相关
    public void AddTips(string tips) {
        //加锁，设置为临界区
        lock (tipsQue) {
            tipsQue.Enqueue(tips);
        }
    }

    private void Update() {
        //因为update执行太快，第一条tips还没播放完，就取出了下一条播放，因此添加标志位isTipsShow
        if (tipsQue.Count > 0 && isTipsShow == false) {
            lock (tipsQue) {
                string tips = tipsQue.Dequeue();
                isTipsShow = true;
                SetTips(tips);
            }
        }
    }

    private void SetTips(string tips) {
        SetActive(tipsBg);
        SetText(txtTips, tips);

        AnimationClip clip = tipsAni.GetClip("TipsWholeShowAni");
        tipsAni.Play();

        //延时关闭激活状态
        //开启协程，让弹窗动画播放完后，自动关闭
        StartCoroutine(AniPlayDone(clip.length, () => {
            SetActive(tipsBg, false);
            isTipsShow = false;
        }));
    }

    private IEnumerator AniPlayDone(float sec, Action cb) {
        yield return new WaitForSeconds(sec);   //等待指定秒，这里为动画播放的时间
        if (cb != null) {
            cb();
        }
    } 
    #endregion

    public void AddHpItemInfo(string mName, Transform trans, int hp) {
        ItemEntityHP item = null;
        if(itemDic.TryGetValue(mName, out item)) {
            return;
        }
        else {
            GameObject go = resSvc.LoadPrefab(PathDefine.HPItemPrefab, true);
            go.transform.SetParent(hpItemRoot);
            go.transform.localPosition = new Vector3(-1000, 0, 0);
            ItemEntityHP ieh = go.GetComponent<ItemEntityHP>();
            ieh.InitItemInfo(trans, hp);
            itemDic.Add(mName, ieh);
        }
    }
    public void RmvHpItemInfo(string mName) {
        ItemEntityHP item = null;
        if (itemDic.TryGetValue(mName, out item)) {
            Destroy(item.gameObject);
            itemDic.Remove(mName);
        }
    }
    public void RmvAllHpItemInfo() {
        foreach (var item in itemDic) {
            Destroy(item.Value.gameObject);
        }
        itemDic.Clear();
    }

    public void SetDodge(string key) {
        ItemEntityHP item = null;
        if(itemDic.TryGetValue(key, out item)) {
            item.SetDodge();
        }
    }

    public void SetCritical(string key, int critical) {
        ItemEntityHP item = null;
        if (itemDic.TryGetValue(key, out item)) {
            item.SetCritical(critical);
        }
    }

    public void SetHurt(string key, int hurt) {
        ItemEntityHP item = null;
        if (itemDic.TryGetValue(key, out item)) {
            item.SetHurt(hurt);
        }
    }

    public void SetHPVal(string key, int oldVal, int newVal) {
        ItemEntityHP item = null;
        if (itemDic.TryGetValue(key, out item)) {
            item.SetHPVal(oldVal, newVal);
        }
    }

    public void SetSelfDodge() {
        selfDodgeAni.Stop();
        selfDodgeAni.Play();
    }
}