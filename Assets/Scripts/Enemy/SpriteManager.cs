
using UnityEngine;

/** <summary> Makes required transform operations on root object such that sprite reflects movement </summary> **/
public class SpriteManager : MonoBehaviour
{
    public Vector3 forward;

    public void Face(Vector3 target)
    {
        if ((target.x < transform.position.x && forward.x > 0) || (target.x > transform.position.x && forward.x < 0))
            Flip();
    }

    public void Flip()
    {
        forward.x *= -1;
        transform.localScale = new Vector3(forward.x * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    public void FaceMoveDir(Vector3 moveDir)
    {
        if ((moveDir.x < 0 && forward.x > 0) || (moveDir.x > 0 && forward.x < 0))
            Flip();
    }
}
