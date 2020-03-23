using UnityEngine;

public static class Helper
{
    // https://answers.unity.com/questions/893966/how-to-find-child-with-tag.html
    public static T FindComponentInChildWithTag<T>(this GameObject parent, string tag) where T : Component
    {
        var t = parent.GetComponentsInChildren<T>();
        foreach (var tr in t)
            if (tr.tag == tag)
                return tr;
        return null;
    }
}