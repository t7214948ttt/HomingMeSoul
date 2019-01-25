using UnityEngine;
using System.Collections.Generic;

namespace BA_Studio.UnityLib.General
{
    [System.Serializable]
    public class GameObjectFilter
    {
        public List<GameObject> targetGOFilter, ignoreGOList;
        [TagSelector]
        public List<string> targetTagFilter, ignoreTagFilter;

        public LayerMask layerFilter = System.Int32.MaxValue;

        public bool Match (GameObject g)
        {
            if (targetGOFilter.Count > 0 && !targetGOFilter.Contains(g))
            {
                //Debug.Log("Target: False");
                return false;
            }
            if (ignoreGOList.Count > 0 && ignoreGOList.Contains(g))
            {
                //Debug.Log("Target: False");
                return false;
            }
            if (targetTagFilter.Count > 0 && !targetTagFilter.Contains(g.tag))
            {
                //Debug.Log("Tag: False");
                return false;
            }
            if (ignoreTagFilter.Count > 0 && ignoreTagFilter.Contains(g.tag))
            {
                //Debug.Log("Tag: False");
                return false;
            }
            if ((layerFilter.value & 1 << g.layer) == 0 )
            {
                //Debug.Log("Layer: False (" + layerFilter.value + "/" + g.layer + ")");
                return false;
            }
            return true;
        }
    }
}