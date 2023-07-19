using UnityEngine;

namespace IzumiTools
{
    /// <summary>
    /// Controls frequently be instantiated / destroyed objects to only switches its activeSelf.<br/>
    /// Used for bullets, UIs, etc.
    /// </summary>
    /// <typeparam name="T">Component type</typeparam>
    [System.Serializable]
    public class ReuseNest<T> where T : Component
    {
        public Transform nest;
        public T prefab;
        public int ActiveCount
        {
            get
            {
                int count = 0;
                foreach(Transform t in nest)
                {
                    if(t.gameObject.activeSelf)
                        ++count;
                }
                return count;
            }
        }
        public int LastActiveSiblingIndex
        {
            get
            {
                for(int i = nest.childCount - 1; i >= 0; --i)
                {
                    if (nest.GetChild(i).gameObject.activeSelf)
                        return i;
                }
                return 0;
            }
        }
        public T Get(out bool isNewGenerated)
        {
            T returnObject;
            foreach (Transform childTf in nest)
            {
                if (childTf.gameObject.activeSelf)
                    continue;
                if(childTf.TryGetComponent(out returnObject))
                {
                    returnObject.gameObject.SetActive(true);
                    isNewGenerated = false;
                    return returnObject;
                }
            }
            (returnObject = Object.Instantiate(prefab)).transform.SetParent(nest, false);
            returnObject.gameObject.SetActive(true);
            isNewGenerated = true;
            return returnObject;
        }
        public T Get()
        {
            return Get(out bool _);
        }
        public void InactivateAll()
        {
            nest.ActiveAllChidren(false);
        }
        public void DestroyAll()
        {
            nest.DestroyAllChildren();
        }
    }

}