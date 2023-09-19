/*-----------------------------------------------------
    文件：PETools.cs
	作者：Johnson
    日期：2023/5/27 16:50:6
	功能：工具类
------------------------------------------------------*/


public class PETools {
    public static int RDInt(int min, int max, System.Random rd = null) {
        if(rd == null) {
            rd = new System.Random();
        }
        int val = rd.Next(min, max + 1);
        return val;
    }
}