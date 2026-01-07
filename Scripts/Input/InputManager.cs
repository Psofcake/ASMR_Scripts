using UnityEngine;

public class InputManager : MonoBehaviour
{
    private Camera mainCam;
    private float zPos = 5;
    private bool isDragging = false;
    private GameObject knife;
    
    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        HandleInput();
    }
    
    void HandleInput()
    {
//#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            TryGrabKnife(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            MoveKnife(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            ReleaseKnife();
        }
//#else
        if (Input.touchCount == 0) return;
        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began) // 터치 시작
        {
            TryGrabKnife(touch.position);
        }
        else if (touch.phase == TouchPhase.Moved && isDragging) // 드래그
        {
            MoveKnife(touch.position);
        }
        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) // 터치 종료
        {
            ReleaseKnife();
            
        }
        
//#endif
    }

    void TryGrabKnife(Vector3 screenPos)
    {
        Ray ray = mainCam.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Knife"))
            {
                zPos = hit.collider.gameObject.transform.position.z-hit.point.z;
                knife = hit.collider.gameObject; //<-나중에 삭제 (시작 시 칼 선택하고 선택한 칼로 미리 knife초기화하기)
                isDragging = true;
            }
        }
    }
    
    void MoveKnife(Vector3 screenPos)
    {
        //Vector3 handlePos = knife.transform.GetChild(0).position;
        //Vector3 offset = knife.transform.position-handlePos;
        knife.transform.position = ScreenToWorld(screenPos); //+offset
    }
    Vector3 ScreenToWorld(Vector3 screenPos)
    {
        screenPos.z = zPos;
        Vector3 worldPos = mainCam.ScreenToWorldPoint(screenPos);
        return worldPos;
    }

    void ReleaseKnife()
    {
        isDragging = false;
    }
}
