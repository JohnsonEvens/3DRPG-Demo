/*-----------------------------------------------------
    文件：StateMove.cs
	作者：Johnson
    日期：2023/8/6 22:55:40
	功能：移动状态
------------------------------------------------------*/


public class StateMove : IState {
    public void Enter(EntityBase entity, params object[] args) {
        entity.currentAniState = AniState.Move;
    }

    public void Exit(EntityBase entity, params object[] args) {
    }

    public void Process(EntityBase entity, params object[] args) {
        entity.SetBlend(Constants.BlendMove);
    }
}

