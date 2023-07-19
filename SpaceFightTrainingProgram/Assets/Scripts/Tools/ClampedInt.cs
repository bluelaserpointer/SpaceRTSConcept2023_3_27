using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ClampedInt
{
    public int min, max;
    public UnityEvent<int> onValueChange = new UnityEvent<int>();

    public ClampedInt(int min, int max)
    {
        this.min = min;
        this.max = max;
    }
    public int Value
    {
        get => _value;
        set {
            if(_value != (value = Mathf.Clamp(value, min, max)))
            {
                onValueChange.Invoke(_value = value);
            }
        }
    }
    int _value;

    public int ValueIndex => Value - min;
}
