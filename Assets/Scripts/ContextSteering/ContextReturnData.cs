using UnityEngine;
public struct ContextReturnData{
    public Vector3 bestPoint;
    public int bestPointIndex;
    public float bestPointWeight;

    public ContextReturnData(Vector3 bestPoint, int bestPointIndex, float bestPointWeight)
    {
        this.bestPoint = bestPoint;
        this.bestPointIndex = bestPointIndex;
        this.bestPointWeight = bestPointWeight;
    }
}
