using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchPadSystem : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public GameObject cube;

    public float Distance => _touchID != -1
        ? Vector2.Distance(_touchPadBackObj.transform.position, (Vector3) Input.GetTouch(_touchID).position)
        : 0;

    /// <summary>
    /// 터치 패드 방향(모바일2D 화면 방향)
    /// </summary>
    public Vector2 DirectionByVector2
    {
        get
        {
            Vector2 direction = Vector2.zero;
            if (_touchID != -1)
            {
                direction = Input.GetTouch(_touchID).position - (Vector2) _touchPadBackObj.transform.position;
            }

            return direction;
        }
    }

    /// <summary>
    /// 터치 패드 방향(인게임3D 방향)
    /// </summary>
    public Vector3 DirectionByVector3 => new Vector3(DirectionByVector2.x, 0, DirectionByVector2.y);

    /// <summary>
    /// 터치 패드 배경 이미지 오브젝트
    /// </summary>
    [SerializeField] private GameObject _touchPadBackObj;

    /// <summary>
    /// 터치 패드 앞 이미지 오브젝트
    /// </summary>
    [SerializeField] private GameObject _touchPadFrontObj;

    /// <summary>
    /// 터치 패드 area를 터치할 시 touchID를 넣어둠
    /// 기본 값은 -1
    /// </summary>
    private int _touchID = -1;

    void Update()
    {
        if (Distance > 40)
        {
            cube.transform.position += DirectionByVector3.normalized * 5 * Time.deltaTime;
        }

        moveTouchPad();
    }

    /// <summary>
    /// 터치 패드 움직임 관련 함수
    /// </summary>
    private void moveTouchPad()
    {
        // 터치 패드를 누린 touchID 가 있다면
        if (_touchID > -1)
        {
            float distance = Vector2.Distance(_touchPadBackObj.transform.position,
                (Vector3) Input.GetTouch(_touchID).position);
            _touchPadFrontObj.transform.position = _touchPadBackObj.transform.position +
                                                   (Vector3) DirectionByVector2.normalized * Mathf.Min(distance, 130);
        }
    }

    /// <summary>
    /// 터치 패드 공간 터치 후 뗐을 때
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        _touchPadBackObj.SetActive(false);
        _touchID = -1;
    }

    /// <summary>
    /// 터치 패드 공간 터치 시
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        _touchID = eventData.pointerId;
        _touchPadBackObj.transform.position = eventData.position;
        _touchPadFrontObj.transform.localPosition = Vector3.zero;
        _touchPadBackObj.SetActive(true);
    }
}
