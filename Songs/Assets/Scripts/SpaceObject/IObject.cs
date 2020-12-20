using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public delegate void OnLoadProgressDelegate(float progress);

    public abstract class IObject 
    {
        public int TargetId { get; protected set; }
        public string Name { get; protected set; }
        public virtual bool ActiveSelf { get; set; }
    }

public abstract class IAssetObject : IObject
{
    public Transform Tran { get; protected set; }
    public string URL { get; protected set; }
    public Vector3 Position { get; protected set; }
    public Vector3 Euler { get; protected set; }
    public Vector3 Scale { get; protected set; }

    public virtual void InitAssetObject(Transform asset)
    {
        if (asset == null) return;
        Tran = asset;

        //Tran.name = SpaceType + "_" + TargetId;
        Tran.localPosition = Position;
        Tran.localEulerAngles = Euler;
        Tran.localScale = Vector3.one;
    }

    public virtual void Destroy()
    {
        ActiveSelf = false;
        if (Tran != null)
        {
            Transform transform = Tran;
            Tran = null;
            //该类型的对象，是根据场景内的对象自定义的，上层自行删除
            GameObject.DestroyImmediate(transform.gameObject);
        }
    }
}