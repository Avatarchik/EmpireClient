using UnityEngine;

public abstract class MonoPlanetCustom : MonoBehaviour
{
    protected abstract void Start();

    protected abstract void DoEnable();

    protected abstract void DoDisable();

    public void SetActive(bool AValue)
    {
        if (AValue)
            DoEnable();
        else
            DoDisable();
    }
}