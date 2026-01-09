using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private Controls controls;
    
    private Camera mainCam;
    private bool isDragging = false;
    private GameObject knife;
    
    [SerializeField] private float zPos = 1; //카메라에서부터 떨어진 거리

    private void Awake()
    {
        controls = new Controls();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    void Start()
    {
        mainCam = Camera.main;
        //controls.Touch.TouchPress.started += ctx => StartTouch(ctx); (아래와 동일)
        controls.Interaction.Grab.performed += TryGrabKnife;
        controls.Interaction.Press.canceled += EndTouch;
    }
    
    private void TryGrabKnife(InputAction.CallbackContext context)
    {
        if(!isDragging)
            GrabKnife(controls.Interaction.Position.ReadValue<Vector2>());
    }
    
    private void EndTouch(InputAction.CallbackContext context)
    {
        if (isDragging)
            ReleaseKnife();
    }

    void Update()
    {
        if (isDragging)
        {
            MoveKnife(controls.Interaction.Position.ReadValue<Vector2>());
        }
    }
    
    void GrabKnife(Vector3 screenPos)
    {
        Ray ray = mainCam.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Knife"))
            {
                knife = hit.collider.transform.parent.gameObject; //<-나중에 삭제 (시작 시 칼 선택하고 선택한 칼로 미리 knife초기화하기)
                isDragging = true;
            }
        }
    }
    
    void MoveKnife(Vector3 screenPos)
    {
        Vector3 handlePos = knife.transform.GetChild(0).position;
        Vector3 offset = knife.transform.position-handlePos;
        knife.transform.position = ScreenToWorld(screenPos) + offset;   //칼손잡이 위치만큼 위치 보정
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
