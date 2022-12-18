using UnityEngine;

public class MoveTransform : MonoBehaviour, IMove
{
    public float moveSpeed = 5;
    public float TurnSpeed = 150;
    public float rotationOffset = 0f;

    private void OnEnable() {
        transform.localRotation = new Quaternion(0f, 0f, rotationOffset, 0f);
    }

    /// <summary>
    /// Move 2D sprite towards target
    /// </summary>
    /// <param name="target"></param>
    /// <param name="movementSpeed"></param>
    public void Move(float verticalInput, float horizontalInput)
    {
        //Move forward and backward
        transform.Translate(Vector3.up * verticalInput * moveSpeed * Time.deltaTime);

        //Rotate on Z Axis
        transform.Rotate(Vector3.forward * horizontalInput * TurnSpeed * Time.deltaTime);
    }
}