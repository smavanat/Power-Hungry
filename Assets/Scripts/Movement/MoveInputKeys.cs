using UnityEngine;

public class MoveInputKeys : MonoBehaviour
{
    private IMove iMove;

    private void OnEnable() {
        if(iMove == null){
            iMove = GetComponent<IMove>();
        }
    }

    private void Update() {
        iMove.Move(Input.GetAxis("Vertical"), -Input.GetAxis("Horizontal"));
    }
}