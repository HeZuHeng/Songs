/// <summary>
/// UI的基类
///  
/// </summary>

using UnityEngine;
using System.Collections.Generic;

namespace Songs{

	public class UIBase : MonoBehaviour {
		/// <summary>
		/// UI层级
		/// </summary>
		public  int UITier=1;

		public UIType Type;

//		public bool isDebug;

		public List<SubUIBase> subUIBase;

		[System.NonSerialized]private bool mutexInterface = true;
		/// <summary>
		/// 是否是互斥界面
		/// </summary>
		public bool MutexInterface{
			get{ 
				return mutexInterface;
			}
			protected set{ 
				mutexInterface = value;
			}
		}

		[System.NonSerialized]private SubUIType curSubUIType = SubUIType.NONE;
		public SubUIType CurSubUIType{
			get{ 
				return curSubUIType;
			}
			set{ 
				curSubUIType = value;
			}
		}

		protected int adddepth = 0;
		public int AddDepth{
			get{ 
				return adddepth;
			}
			set{
				adddepth = value;
			}
		}
		/// <summary>
		/// 是否第一次打开
		/// </summary>
		[System.NonSerialized]private bool IsStart = false;

		private Dictionary<SubUIType,SubUIBase> subWnd = new Dictionary<SubUIType, SubUIBase>();

        protected virtual void Awake(){
			for (int i = 0; i < subUIBase.Count; i++) {
				if (!subWnd.ContainsKey (subUIBase [i].subType) && subUIBase [i] != null) {
					subWnd [subUIBase [i].subType] = subUIBase [i];
				}
			}
		}

		protected virtual void Start(){
			Init ();
			if (CurSubUIType == SubUIType.NONE)
				CurSubUIType = subUIBase.Count > 0 ? subUIBase [0].subType : SubUIType.NONE;
			ToSunUI ();
			IsStart = true;
		}

		protected virtual void Init(){
		}

		void ToSunUI(){
			if (curSubUIType == SubUIType.NONE)
				return;
			if (!subWnd.ContainsKey (curSubUIType)) {
				SubUIBase ui = ObtainSubUI (UIMng.Instance.GetSubUIPath(curSubUIType));
				if (ui == null)
					return;
				subWnd [curSubUIType] = ui;
				ui.Init ();
				subUIBase.Add (ui);
			} 
			SubUIBase curSubbase = null;
			using (var data = subWnd.GetEnumerator ()) {
				while (data.MoveNext ()) {
					if (data.Current.Key == curSubUIType) {
						curSubbase = data.Current.Value;
						curSubbase.AddDepth = AddDepth;
					} else {
						if(data.Current.Value.gameObject.activeSelf)data.Current.Value.gameObject.SetActive (false);
					}
				}
			}
			if (!curSubbase.gameObject.activeSelf) {
				curSubbase.gameObject.SetActive (true);
			}
			OpenSubUIFinishe ();
		}

		protected virtual void OpenSubUIFinishe(){

		}

		/// <summary>
		/// 打开二级界面
		/// </summary>
		public void OpenSubUI(SubUIType _subType){
			CurSubUIType = _subType;
			if(IsStart)ToSunUI ();
		}

		SubUIBase ObtainSubUI(string _path){
			Object obj = ExResources.GetResources (_path,ExResources.PathType.UI);
			if (obj != null) {
				GameObject game = Instantiate (obj) as GameObject;
				game.transform.parent = transform;
				game.transform.localScale = Vector3.one;
				game.transform.localPosition = Vector3.zero;
				SubUIBase uibase = game.GetComponent<SubUIBase> ();
				if (uibase != null) {
					uibase.gameObject.SetActive (false);
				}
				return uibase;
			} 
			return null;
		}

		protected virtual void OnEnable(){
			if (UIMng.Instance != null)
				UIMng.Instance.SetUiTier (this);
		}

		protected virtual void OnDisable(){
			curSubUIType = SubUIType.NONE;
        }

		public  virtual string GetUIData()
		{
			return string.Empty;
		}

//		protected void ToClose(){
//			UIMng.Instance.DestroyUIBase (this.Type);
//		}
	}
}
