/* Бредовый юнит, призванный вращать любой объект. Скорее всего будет упразднен */
using UnityEngine;

public class UIControlsRotator : MonoBehaviour
{

    public int MinSpeed = 5;
    public int MaxSpeed = 20;
    public Vector3 Vector;

    private float FVectorUp;
    private float FVectorRight;
    private float FVectorForward;

    private float FRotationSpeed;

    private Renderer FRenderer;

    void Start()
    {
        FRenderer = transform.GetComponent<MeshRenderer>();

        FRotationSpeed = Random.Range(MinSpeed, MaxSpeed);

        if (Vector.sqrMagnitude == 0)
        {
            FVectorUp = Random.Range(MinSpeed, MaxSpeed);
            if (Random.Range(0, 100) < 50) FVectorUp *= -1;
            FVectorRight = Random.Range(MinSpeed, MaxSpeed);
            if (Random.Range(0, 100) < 50) FVectorRight *= -1;
            FVectorForward = Random.Range(MinSpeed, MaxSpeed);
            if (Random.Range(0, 100) < 50) FVectorForward *= -1;
            Vector = new Vector3(FVectorUp, FVectorRight, FVectorForward);
        }
    }

    void Update()
    {
        if ((!FRenderer) || (FRenderer.isVisible))
            transform.Rotate(Vector, FRotationSpeed * -Time.deltaTime);
    }
}
