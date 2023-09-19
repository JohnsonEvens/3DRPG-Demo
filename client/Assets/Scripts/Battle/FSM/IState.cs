/*-----------------------------------------------------
    文件：IState.cs
	作者：Johnson
    日期：2023/8/6 22:50:40
	功能：状态接口
------------------------------------------------------*/


public interface IState {
    void Enter(EntityBase entity, params object[] args);    //可变参数

    void Process(EntityBase entity, params object[] args);

    void Exit(EntityBase entity, params object[] args);
}

public enum AniState {
    None,
    Born,
    Idle,
    Move,
    Attack,
    Hit,
    Die,
}
