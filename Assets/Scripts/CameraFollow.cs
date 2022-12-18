using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform followTarget;
    public float smoothSpeed = 5f;
    public Vector3 offset;


    private void FixedUpdate() {
        if(followTarget != null){
            Vector3 desiredPosition = followTarget.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
    }
}