using Abyss.EventSystem;
using UnityEngine;

public class Arena : MonoBehaviour
{
    public readonly static string EventPostfix = " Arena Entered/Exited";

    public string arenaName;
    public bool PlayerIn = false;
    public GameObject Player;

    [Tooltip("Defines collider bounds by top right bottom left anchor, overrides original")] public bool Movable = false;
    [SerializeField][Tooltip("Should not be made children of this gameobject")] Transform topRight, bottomLeft;
    float _width, _height;
    BoxCollider2D _collider2D;

    public enum Anchor { TopRight, BottomLeft, TopLeft, BottomRight, Center }

    void Start()
    {
        if (Movable)
        {
            _width = topRight.position.x - bottomLeft.position.x;
            _height = topRight.position.y - bottomLeft.position.y;
            var center = (topRight.position + bottomLeft.position) / 2;
            _collider2D = GetComponent<BoxCollider2D>();
            _collider2D.size = new Vector2(_width * 2, _height * 2);
            _collider2D.offset = Vector2.zero;
            transform.position = center;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player = other.gameObject;
            PlayerIn = true;
            EventManager.InvokeEvent(new GameEvent(EEEvent), true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player = null;
            PlayerIn = false;
            EventManager.InvokeEvent(new GameEvent(EEEvent), false);
        }
    }

    public void MoveTo(Vector3 position, Anchor anchor)
    {
        switch (anchor)
        {
            case Anchor.TopRight:
                transform.position = position - new Vector3(_width / 2, _height / 2, 0);
                break;
            case Anchor.BottomLeft:
                transform.position = position + new Vector3(_width / 2, _height / 2, 0);
                break;
            case Anchor.TopLeft:
                transform.position = position + new Vector3(_width / 2, -_height / 2, 0);
                break;
            case Anchor.BottomRight:
                transform.position = position + new Vector3(-_width / 2, _height / 2, 0);
                break;
            case Anchor.Center:
                transform.position = position;
                break;
        }
    }

    public Vector3 GetAnchorPos(Anchor anchor)
    {
        switch (anchor)
        {
            case Anchor.TopRight:
                return transform.position + new Vector3(_width / 2, _height / 2, 0);
            case Anchor.BottomLeft:
                return transform.position - new Vector3(_width / 2, _height / 2, 0);
            case Anchor.TopLeft:
                return transform.position + new Vector3(-_width / 2, _height / 2, 0);
            case Anchor.BottomRight:
                return transform.position - new Vector3(-_width / 2, _height / 2, 0);
            case Anchor.Center:
                return transform.position;
        }
        return Vector3.zero;
    }

    public string EEEvent => arenaName + EventPostfix;
}
