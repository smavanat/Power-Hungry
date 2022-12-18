using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class ContextSteering : MonoBehaviour
{
    [Header("Behaviour Setup")]
    public ContextSteeringBehaviourSetup steeringBehaviour;

    private float circleRadius;
    [Range(2, 20)]
    public int resolution;
    private float avoidAvoidanceWeight;
    private float danagerAvoidanceWeight;
    private float combinedW;

    [Header("Mask Filters")]
    private LayerMask interestLayermask;
    private LayerMask avoidLayerMask;
    private LayerMask dangerLayermask;

    [Header("Debug")]
    private bool showDebugLines;
    
    //Local Vars
    private bool isInitialized;
    private Rigidbody2D myRigidBody;
    private Collider2D myCollider;

#region Weight Lists
    [Header("Weight Lists")]

    private List<Vector3> detectionRays;
    //[SerializeField]

    private List<float> interest;

    //[SerializeField]

    private List<float> danger;

    //[SerializeField]

    private List<float> avoid;

    //[SerializeField]

private List<float> combinedWeight;
    [SerializeField]
#endregion

//All set up methods for the class.
#region Initialization Methods
    private void Start() {
        this.circleRadius = steeringBehaviour.circleRadius;
        this.resolution = steeringBehaviour.resolution;
        this.avoidAvoidanceWeight = steeringBehaviour.avoidAvoidanceWeight;
        this.danagerAvoidanceWeight = steeringBehaviour.danagerAvoidanceWeight;
        this.interestLayermask = steeringBehaviour.interestLayermask;
        this.avoidLayerMask = steeringBehaviour.avoidLayerMask;
        this.dangerLayermask = steeringBehaviour.dangerLayermask;
        this.showDebugLines = steeringBehaviour.showDebugLines;

        this.myCollider = GetComponent<Collider2D>();
        this.myRigidBody = GetComponent<Rigidbody2D>();

        SetUpContexts();

        this.isInitialized = true;
    }

    /// <summary>
    /// Set up of internal lists for the contexts and the detection rays
    /// </summary>
    private void SetUpContexts(){
        detectionRays = new List<Vector3>();
        interest = new List<float>();

        danger = new List<float>();

        avoid = new List<float>();

        combinedWeight = new List<float>();


        for (int i = 0; i < resolution; i++){
            GetDetectionRayPoints(i, i + 1);
        }
    }

    /// <summary>
    /// The main purpose of this code section is to get the angle for each Detection ray based on the resolution.  
    ///The ray is then added to the detectionRays list and corresponding entries are added to the interest, danger, avoid, combined weights lists to accommodate the new ray as well.
    /// </summary>
    private void GetDetectionRayPoints(int resolutionIndex, int currentDetectionRayCount){
        //Divide the viewing angles into lines based on the resolution
        float viewAngle = 360 / resolution;
        //Store a ref to the view angle of each line
        Vector3 viewAngleDir;
        //calculate the direction of the line around the circle
        float viewAngleUpdated = viewAngle * resolutionIndex;
        //Calculate the angle of the line around the circle
        viewAngleDir = DirFromAngle(viewAngleUpdated, true);
        //Add detection ray point
        if(detectionRays.Count < currentDetectionRayCount) {
            detectionRays.Add(viewAngleDir); 
            interest.Add(0);
            danger.Add(0);
            avoid.Add(0);
            combinedWeight.Add(0);
        }
           
        
    }
#endregion

    /// <summary>
    /// This is the main update method that updates the Context Based Steering behaviour of the NPC.  This should be called from a central update method on the NPC.  
    ///First it checks to ensure it has been properly initialized and if not it will send a log error message and return out.  Secondly, it checks to make sure the 
    ///resolution is being displayed correctly in the game and if not it redraws the detection rays and recreates the detection lists to match the new resolution.  
    ///The last section is the one where all the detection rays under the detection ray list are looped over and drawn to the game.  We will be adding additional 
    ///functionality to this method to handle updates to the interest, avoid, danger lists.
    /// </summary>
    public virtual ContextReturnData Tick(Transform target){
        if(!isInitialized){
            Debug.LogError("Context Steering is not initialized.  Initialize context steering before calling the Tick method");
            return new ContextReturnData(Vector3.zero, -1, -1f);
        }

        //Resizes the lists based on changes to the resolution
        if(detectionRays.Count < resolution || detectionRays.Count > resolution)
            SetUpContexts(); 
        

        //Draws the detection rays
        for (int i = 0; i < detectionRays.Count; i++)
        {
            CalculatePathWeights(detectionRays[i], i, target);
            if(showDebugLines){
                DrawSteeringContextDebugLines(detectionRays[i], i);
            }
        }

        return ChooseBestPoint();
    }

#region Path Calculation
    private void CalculatePathWeights(Vector3 detectionRayPoint, int detectionRayIndex, Transform target){
        SetInterest(detectionRayPoint, detectionRayIndex, target);
        SetDanger(detectionRayPoint, detectionRayIndex);
        SetAvoid(detectionRayPoint, detectionRayIndex);

        combinedWeight[detectionRayIndex] = 
                                        Mathf.Clamp01(interest[detectionRayIndex] + 
                                        danger[detectionRayIndex] + 
                                        avoid[detectionRayIndex]);
    }

    int bestIndex;
    float bestIndexWeight;
    private ContextReturnData ChooseBestPoint(){
        //Get the index with the best weight
        bestIndex = combinedWeight.IndexOf(combinedWeight.Max());
        return new ContextReturnData(WeightDirectionVectorsUsingCombinedWeights(bestIndex), bestIndex, combinedWeight[bestIndex]);
    }

private Vector3 blendedDir;

    private Vector3 WeightDirectionVectorsUsingCombinedWeights(int bestCombinedWeightIndex){
        int leftDirIndex = GetDirToLeft(bestCombinedWeightIndex);
        int rightDirIndex = GetDirToRight(bestCombinedWeightIndex);
        
        blendedDir = 
                transform.position + 
                (
                    (
                        (detectionRays[bestCombinedWeightIndex] * combinedWeight[bestCombinedWeightIndex])  + 
                        (detectionRays[leftDirIndex] * combinedWeight[leftDirIndex]) + 
                        (detectionRays[rightDirIndex] * combinedWeight[rightDirIndex])
                    ).normalized * ScaleFloat(0,1,0,circleRadius, combinedWeight[bestCombinedWeightIndex])
                );

        if(showDebugLines){
            Debug.DrawLine(
                    transform.position, 
                    blendedDir,
                    Color.blue);
        }

        return blendedDir; 
    }
#endregion

#region Set the Weights
    private void SetInterest(Vector3 detectionRayPoint, int detectionRayIndex, Transform target){
        interest[detectionRayIndex] = Mathf.Clamp01(CheckDotProductOfVector(detectionRayPoint, target));
    }


    RaycastHit2D[] hitAvoid2D;
    private void SetAvoid(Vector3 detectionRayPoint, int detectionRayIndex){
        hitAvoid2D = CheckPathObstruction(detectionRays[detectionRayIndex]);

        if(hitAvoid2D.Length > 0){
            foreach (RaycastHit2D hit2D in hitAvoid2D){
                if(hit2D.collider != myCollider){
                    //Debug.Log(hit2D.collider.gameObject.name);
                    avoid[detectionRayIndex] = -Mathf.Clamp01(circleRadius - hit2D.distance); 
                    if(-hit2D.distance > avoidAvoidanceWeight){
                        avoid[detectionRayIndex] = -1;
                    }
                    break;
                }
                else{
                    avoid[detectionRayIndex] = 0;  //This sets the weight of this list index to 0 if no correct hit was found
                }
            }
        }
        else{
            avoid[detectionRayIndex] = 0;
        }
    }


    RaycastHit2D[] hitDanager2D;
    /// <summary>
    /// Create a raycast to detect the obstructions in the avoid mask
    /// </summary>

    private void SetDanger(Vector3 detectionRayPoint, int detectionRayIndex){
        hitDanager2D = CheckPathDanger(detectionRays[detectionRayIndex]);

        if(hitDanager2D.Length > 0){
            foreach (RaycastHit2D hit2D in hitDanager2D){
                if(hit2D.collider != myCollider){
                    danger[detectionRayIndex] = -Mathf.Clamp01(circleRadius - hit2D.distance); 
                    if(-hit2D.distance > danagerAvoidanceWeight){
                        danger[detectionRayIndex] = -1;
                    }
                    break;
                }
                else{
                    danger[detectionRayIndex] = 0;
                }
            }
        }
        else{
            danger[detectionRayIndex] = 0;
        }
    }

    /// <summary>
    /// Create a raycast to detect the obstructions in the avoid mask
    /// </summary>
    private RaycastHit2D[] CheckPathObstruction(Vector3 detectionRayPoint){
        return CreateObjectDetectionRay2D_ALL(
            avoidLayerMask, 
            transform, 
            detectionRayPoint, 
            circleRadius
        );
    }

    /// <summary>
    /// Create a raycast to detect the danagers in the dangers mask
    /// </summary>
    private RaycastHit2D[] CheckPathDanger(Vector3 detectionRayPoint){
        return CreateObjectDetectionRay2D_ALL(
            dangerLayermask, 
            transform, 
            detectionRayPoint, 
            circleRadius
        );
    }
#endregion

#region Helpers
    /// <summary>
    /// Gets the detection ray index to the right of the best direction ray
    /// </summary>
    private int GetDirToRight(int index){
        return (index == resolution - 1) ? 0 : index + 1;
    }

    /// <summary>
    /// Gets the detection ray index to the left of the passed direction ray with index n
    /// </summary>
    private int GetDirToLeft(int index){
        return ((index - 1) < 0) ? resolution - Mathf.Abs(index - 1) : index - 1;
    }
    
    /// <summary>
    /// Scale a float within a new range
    /// </summary>
    private float ScaleFloat(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue)
    {

        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;

        return (NewValue);
    }
    /// <summary>
    /// Used for visualizing the detection rays
    /// </summary>
    private void DrawSteeringContextDebugLines(Vector3 detectionRayPos, int detectionRayIndex){
        combinedW = combinedWeight[detectionRayIndex];

        if(combinedW > 1 * 0.9){
            Debug.DrawLine(
                transform.position, 
                transform.position + (detectionRayPos.normalized * ScaleFloat(0,1,0,circleRadius,combinedW)), 
                Color.green);
        } 
        else if(combinedW > 1 * 0.5){
            Debug.DrawLine(
                transform.position, 
                transform.position + (detectionRayPos.normalized * ScaleFloat(0,1,0,circleRadius,combinedW)), 
                Color.yellow);
        }
        else if(combinedW > 1 * 0.2){
            Debug.DrawLine(
                transform.position, 
                transform.position + (detectionRayPos.normalized * ScaleFloat(0,1,0,circleRadius,combinedW)), 
                Color.red);
        }
    }

    /// <summary>
    /// Create a ray according to the directions passed and return the RayCastHit Array of all colliders hit
    /// </summary>
    private RaycastHit2D[] CreateObjectDetectionRay2D_ALL(LayerMask layerMask, Transform transform, Vector3 dir, float scanDistance){
        return Physics2D.RaycastAll(transform.position, dir, scanDistance, layerMask);
    }

    /// <summary>
    /// Get the Dot product of the targetTransform and lines direction 
    /// Used to weight the interest of a specific direction
    /// </summary>
    private float CheckDotProductOfVector(Vector3 detectionRayPos, Transform target){
    //if(showDebugLines){
        //Debug.Log("Target Pos: " + target.position + " Target Transform:        "+ target);
    //}

        return Vector3.Dot((target.position - this.transform.position).normalized, detectionRayPos.normalized);
    }

    /// <summary>
    /// This one takes in an angle in degrees and can be used on a global angle or a local angle. Essentially this code is using trigonometry to calculate a new vector based on the passed angle.
    /// </summary>
    private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += -transform.eulerAngles.z;
        }

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0);
    } 
#endregion
}