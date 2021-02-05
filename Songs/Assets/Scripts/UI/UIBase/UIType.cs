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
		AnswerWnd = 12,
		FantasyWnd = 13,
		ExperienceWnd = 14,
		FantasyCompareWnd = 15,
		RenameWnd = 16,
		QATalkWnd = 17,
        HTMMapPathWnd = 18,
        HTMMapPoint1Wnd = 19,
        HTMMapPoint2Wnd = 20,
        HTMMapPoint3Wnd = 21,
        HTMBoatNameWnd = 22,
        HZHSEndWnd,
		MemoryWnd,
		LookSongWnd,
		HTMEndWnd,
	}
	/// <summary>
	/// 二级界面枚举
	/// </summary>
	public enum SubUIType : byte{
		NONE = 0,
	} 
}
