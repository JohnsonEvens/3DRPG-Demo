/*-----------------------------------------------------
    文件：GameStart.cs
	作者：Johnson
    日期：2023/5/29 18:30:30
	功能：服务器入口
------------------------------------------------------*/


using System.Threading;

class ServerStart {
    static void Main(string[] args) {
        ServerRoot.Instance.Init();

        while (true) {
            ServerRoot.Instance.Update();
            Thread.Sleep(20);   //降低服务器帧率
        }
    }
}