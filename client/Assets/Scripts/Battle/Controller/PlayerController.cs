/*-----------------------------------------------------
    文件：PlayerController.cs
	作者：Johnson
    日期：2023/7/2 17:10:5
	功能：主角表现实体角色控制器
------------------------------------------------------*/

using UnityEngine;

public class PlayerController : Controller {
    public GameObject daggerskill1fx;
    public GameObject daggerskill2fx;
    public GameObject daggerskill3fx;

    public GameObject daggeratk1fx;
    public GameObject daggeratk2fx;
    public GameObject daggeratk3fx;
    public GameObject daggeratk4fx;
    public GameObject daggeratk5fx;


    private Vector3 camOffset;
    private Vector3 oldPos;

    private float targetBlend;
    private float currentBlend;

    public override void Init() {
        base.Init();

        camTrans = Camera.main.transform;
        camOffset = transform.position - camTrans.position;

        if (daggerskill1fx != null) {
            fxDic.Add(daggerskill1fx.name, daggerskill1fx);
        }
        if (daggerskill2fx != null) {
            fxDic.Add(daggerskill2fx.name, daggerskill2fx);
        }
        if (daggerskill3fx != null) {
            fxDic.Add(daggerskill3fx.name, daggerskill3fx);
        }

        if (daggeratk1fx != null) {
            fxDic.Add(daggeratk1fx.name, daggeratk1fx);
        }
        if (daggeratk2fx != null) {
            fxDic.Add(daggeratk2fx.name, daggeratk2fx);
        }
        if (daggeratk3fx != null) {
            fxDic.Add(daggeratk3fx.name, daggeratk3fx);
        }
        if (daggeratk4fx != null) {
            fxDic.Add(daggeratk4fx.name, daggeratk4fx);
        }
        if (daggeratk5fx != null) {
            fxDic.Add(daggeratk5fx.name, daggeratk5fx);
        }
    }

    private void Update() {
        #region Input
        /*float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector2 _dir = new Vector2(h, v).normalized;

        if (_dir != Vector2.zero) {
            Dir = _dir;
            SetBlend(Constants.BlendWalk);
        }
        else {
            Dir = Vector2.zero;
            SetBlend(Constants.BlendIdle);
        }*/
        #endregion

        if (Input.GetKeyDown(KeyCode.Tab)) {
            Camera.main.GetComponent<CameraRotateAround>().isMouseControlEnabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        if (Input.GetKeyUp(KeyCode.Tab) && !Camera.main.GetComponent<CameraRotateAround>().isWndOpen) {
            Camera.main.GetComponent<CameraRotateAround>().isMouseControlEnabled = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (currentBlend != targetBlend) {
            UpdateMixBlend();
        }

        oldPos = transform.position;
        if (isMove) {
            //设置方向
            SetDir();
            //产生移动

            SetMove();
            //相机跟随
            SetCam();
        }

        if (skillMove) {
            SetSkillMove();
            //相机跟随
            SetCam();
        }
    }

    private void SetDir() {
        float angle = Vector2.SignedAngle(Dir, new Vector2(1, 0)) + camTrans.eulerAngles.y;
        Vector3 eulerAngles = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngles;
    }

    private void SetMove() {
        //oldPos = transform.position;
        ctrl.Move(transform.forward * Time.deltaTime * Constants.PlayerMoveSpeed);
    }

    private void SetSkillMove() {
        //oldPos = transform.position;
        ctrl.Move(transform.forward * Time.deltaTime * skillMoveSpeed);
    }

    public void SetCam() {
        if(camTrans != null) {
            Vector3 newOffset = camTrans.position - (oldPos - camOffset);
            camTrans.position = transform.position - camOffset + newOffset;
        }
    }
    private void UpdateMixBlend() {
        if(Mathf.Abs(currentBlend - targetBlend) < Constants.AccelerSpeed * Time.deltaTime) {
            currentBlend = targetBlend;
        }
        else if(currentBlend > targetBlend) {
            currentBlend -= Constants.AccelerSpeed * Time.deltaTime;
        }
        else {
            currentBlend += Constants.AccelerSpeed * Time.deltaTime;
        }
        ani.SetFloat("Blend", currentBlend);
    }


    //////////////////////////////////////////////////////////////

    public override void SetBlend(float blend) {
        targetBlend = blend;
    }

    public override void SetFX(string name, float destroy) {
        GameObject go;
        if(fxDic.TryGetValue(name,out go)) {
            go.SetActive(true);
            timerSvc.AddTimeTask((int tid) => {
                go.SetActive(false);
            }, destroy);
        }
    }

}