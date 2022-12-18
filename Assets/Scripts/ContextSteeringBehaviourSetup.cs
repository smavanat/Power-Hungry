using UnityEngine;

[CreateAssetMenu(fileName = "ContextSteeringBehaviour", menuName = "Steering Behaviour", order = 0)]
public class ContextSteeringBehaviourSetup : ScriptableObject {
    [Header("Context Variables")]
    [Range(1, 50)]
    public float circleRadius;
    [Range(1, 50)]
    public int resolution;
    [Range(-1.0f, 0f)]
    public float avoidAvoidanceWeight;
    [Range(-1.0f, 0f)]
    public float danagerAvoidanceWeight;

    [Header("Mask Filters")]
    public LayerMask interestLayermask;
    public LayerMask avoidLayerMask;
    public LayerMask dangerLayermask;

    [Header("Debug")]
    public bool showDebugLines; 
}