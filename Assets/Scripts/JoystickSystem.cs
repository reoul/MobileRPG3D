using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickSystem : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public delegate void OnPointerUpEvent();
    public OnPointerUpEvent PointerUpEvent { get; set; }
    public delegate void OnPointerDownEvent();
    public OnPointerDownEvent PointerDownEvent { get; set; }
    
    /// <summary>
    /// 조이스틱 중심부에서의 거리
    /// </summary>
    public float Distance => _touchFingerID != -1
        ? Vector2.Distance(_joystickBackObj.transform.position, (Vector3) _joystickTouch.position)
        : 0;

    /// <summary>
    /// 조이스틱 방향(모바일2D 화면 방향)
    /// </summary>
    public Vector2 DirectionByVector2
    {
        get
        {
            Vector2 direction = Vector2.zero;
            if (_touchFingerID != -1)
            {
                direction = _joystickTouch.position - (Vector2) _joystickBackObj.transform.position;
            }

            return direction;
        }
    }

    /// <summary>
    /// 조이스틱 방향(인게임3D 방향)
    /// </summary>
    public Vector3 DirectionByVector3 => new Vector3(DirectionByVector2.x, 0, DirectionByVector2.y);

    private Touch _joystickTouch
    {
        get
        {
            Touch retTouch = Input.GetTouch(0);
            foreach (Touch touch in Input.touches)
            {
                if (touch.fingerId == _touchFingerID)
                {
                    retTouch = touch;
                    break;
                }
            }

            return retTouch;
        }
    }
    

    /// <summary>
    /// 조이스틱 배경 이미지 오브젝트
    /// </summary>
    [SerializeField] private GameObject _joystickBackObj;

    /// <summary>
    /// 조이스틱 앞 이미지 오브젝트
    /// </summary>
    [SerializeField] private GameObject _joystickFrontObj;

    /// <summary>
    /// 조이스틱 공간을 터치할 시 touchID를 넣어둠
    /// 기본 값은 -1
    /// </summary>
    private int _touchFingerID = -1;

    void Update()
    {
        //Input.touches[0].fingerId
        moveJoystick();
    }

    /// <summary>
    /// 조이스틱 움직임 관련 함수
    /// </summary>
    private void moveJoystick()
    {
        // 조이스틱을 누른 touchID 가 있다면
        if (_touchFingerID > -1)
        {
            float distance = Vector2.Distance(_joystickBackObj.transform.position,
                (Vector3) _joystickTouch.position);
            _joystickFrontObj.transform.position = _joystickBackObj.transform.position +
                                                   (Vector3) DirectionByVector2.normalized * Mathf.Min(distance, 130);
        }
    }

    /// <summary>
    /// 조이스틱 공간 터치 후 뗐을 때
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        _joystickBackObj.SetActive(false);
        _touchFingerID = -1;
        PointerUpEvent?.Invoke();
    }

    /// <summary>
    /// 조이스틱 공간 터치 시
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        _touchFingerID = eventData.pointerId;
        Debug.Log(_touchFingerID);
        _joystickBackObj.transform.position = eventData.position;
        _joystickFrontObj.transform.localPosition = Vector3.zero;
        _joystickBackObj.SetActive(true);
        PointerDownEvent?.Invoke();
    }
}
