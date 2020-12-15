/// <summary>
/// 子界面基类
/// (挂该脚本的对象激活与未激活请用该界面的一级界面继承UIBase类的OpenSubUI(SubUIType _subType)方法)
///  
/// </summary>

using UnityEngine;
//using System.Collections;
/// <summary>
/// (挂该脚本的对象激活与未激活请用该界面的一级界面继承UIBase类的OpenSubUI(SubUIType _subType)方法)
/// </summary>
namespace Songs{
	public class SubUIBase : MonoBehaviour {

		public SubUIType subType;

		protected int adddepth = 0;
		public int AddDepth{
			get{ 
				return adddepth;
			}
			set{
				adddepth = value;
			}
		}

		public void Init(){
		}

		protected virtual void OnEnable(){

		}

		protected virtual void OnDisable(){
			
		}

	}

}