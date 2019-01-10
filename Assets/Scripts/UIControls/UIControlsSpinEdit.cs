/* еще один бредовый юнит, позволяющий менять число стрелочками вниз и вверх, скорее всего ьудет удален */
using UnityEngine;
using UnityEngine.UI;

public class UIControlsSpinEdit : MonoBehaviour
{

    public InputField EditValue;
    public Button ButtonInc;
    public Button ButtonDec;

    private int FValue;
    private int FMaxValue;

    // Use this for initialization
    void Start()
    {
        SetMaxValue(100);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void DoUpdateState(int AValue)
    {
        FValue = Mathf.Max(0, Mathf.Min(AValue, FMaxValue));
        EditValue.text = FValue.ToString();
        ButtonInc.gameObject.SetActive(FValue < FMaxValue);
        ButtonDec.gameObject.SetActive(FValue > 0);
    }

    public void SetMaxValue(int AMaxValue)
    {
        FMaxValue = AMaxValue;
        DoUpdateState(AMaxValue);
    }

    public void UpdateValue()
    {
        int LValue;
        if (int.TryParse(EditValue.text, out LValue))
            DoUpdateState(LValue);
        else
            DoUpdateState(FValue);
    }

    public void Increment()
    {
        if (FValue < FMaxValue)
            DoUpdateState(++FValue);
    }

    public void Decrement()
    {
        if (FValue > 0)
            DoUpdateState(--FValue);
    }
}
