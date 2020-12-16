using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Songs{
	/// <summary>
	/// UI枚举 
	/// </summary>
	public enum UIType : byte{
		NONE = 0,
        //MainWnd = 1,
        StartWnd = 2,
        IntroductionWnd  = 3,
        HZHSWnd = 4,
        HTMWnd = 5,
        SettingWnd = 6,
        LoadingWnd = 8,
		SelectPlotWnd = 9,
		MainDialogueWnd = 10,
		LeftDialogueWnd = 11,
	}
	/// <summary>
	/// 二级界面枚举
	/// </summary>
	public enum SubUIType : byte{
		NONE = 0,
	} 
}
