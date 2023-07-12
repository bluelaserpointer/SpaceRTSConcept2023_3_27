using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public static class GameObjectExtension
{
    public static List<T> FindComponentsWithTag<T>(string tag, Predicate<T> predicate = null)
    {
        List<T> components = new List<T>();
        foreach (GameObject go in GameObject.FindGameObjectsWithTag(tag))
        {
            if (go.TryGetComponent(out T component) && (predicate?.Invoke(component) ?? true))
            {
                components.Add(component);
            }
        }
        return components;
    }
}
