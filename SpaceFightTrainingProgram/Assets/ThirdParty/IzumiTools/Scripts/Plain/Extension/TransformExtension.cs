using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtension
{
    public static Transform Find(this Transform parentTransform, Predicate<Transform> predicate)
    {
        foreach (Transform childTransform in parentTransform)
        {
            if (predicate.Invoke(childTransform))
                return childTransform;
        }
        return null;
    }
    public static T FindComponentInDirectChildren<T>(this Transform parentTransform, Predicate<T> predicate = null) where T : Component
    {

        foreach (Transform childTransform in parentTransform)
        {
            T component = childTransform.GetComponent<T>();
            if (component != null && (predicate?.Invoke(component) ?? true))
                return component;
        }
        return default;
    }
    public static List<T> FindComponentsInDirectChildren<T>(this Transform parentTransform, Predicate<T> predicate = null) where T : Component
    {
        List<T> components = new List<T>();
        foreach (Transform childTransform in parentTransform)
        {
            T component = childTransform.GetComponent<T>();
            if (component != null && (predicate?.Invoke(component) ?? true))
                components.Add(component);
        }
        return components;
    }
    public static void DestroyAllChildren(this Transform parent)
    {
        foreach (Transform each in parent)
            UnityEngine.Object.Destroy(each.gameObject);
    }
    public static void ActiveAllChidren(this Transform parent, bool cond)
    {
        foreach (Transform each in parent)
            each.gameObject.SetActive(cond);
    }
}
