using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MREngine
{
    public class ObjectPool
    {
        private Queue<GameObject> gameObjectQueue = null;
        
        protected Object Asset
        {
            get;private set;
        }

        public ObjectPool(Object asset)
        {
            Asset = asset;
        }

        public Object GetAsset()
        {
            return Asset;
        }

        public GameObject GetGameObject()
        {
            GameObject obj = null;
            if(null == gameObjectQueue)
            {
                gameObjectQueue = new Queue<GameObject>();
            }

            if(gameObjectQueue.Count > 0)
            {
                obj = gameObjectQueue.Dequeue();
                Transform tran = obj.transform;
                tran.SetParent(null);
                tran.rotation = Quaternion.identity;
                tran.position = Vector3.zero;
                return obj; 
            }
            else
            {
                obj = Object.Instantiate(Asset) as GameObject;
                obj.name = Asset.name;                
                return obj;
            }
        }

        public void Destroy(GameObject gameObject)
        {
            if(null == gameObjectQueue)
            {
                Object.Destroy(gameObject);
                Debug.Log("------ gameObjectQueue is null ");
                return;
            }

            gameObject.transform.SetParent(GameDataManager.GetInstance().GetObjectPoolTransform());
            gameObjectQueue.Enqueue(gameObject);
        }

        public void Reset()
        {
            Asset = null;
            if(null != gameObjectQueue)
            {
                while(gameObjectQueue.Count > 0)
                {
                    GameObject obj = gameObjectQueue.Dequeue();
                    Object.Destroy(obj);
                }
            }
            gameObjectQueue = null;
        }

    }
}

