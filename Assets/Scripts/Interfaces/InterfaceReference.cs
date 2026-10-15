using UnityEngine;

[System.Serializable]
public struct InterfaceReference<TInterface> where TInterface : class
{
    [SerializeField] private Object underlyingObject;

    public readonly TInterface Value
    {
        get
        {
            if (!underlyingObject) { return null; }
            return underlyingObject as TInterface;
        }
    }
}
