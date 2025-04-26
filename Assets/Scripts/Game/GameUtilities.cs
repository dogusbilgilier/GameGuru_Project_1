using UnityEngine;

namespace Game
{
    public static class GameUtilities
    {
        #region Transform

        public static void DestroyAllChildren(this Transform transform)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
                GameObject.Destroy(transform.GetChild(i).gameObject);
        }

        public static void DestroyAllChildrenImmediate(this Transform transform)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
                GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
        }

        #endregion
    }
}