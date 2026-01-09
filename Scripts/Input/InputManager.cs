using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private Controls controls;
    
    private Camera mainCam;
    private float zPos = 5;
    private bool isDragging = false;
    private GameObject knife;

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
        Debug.Log("grab > "+controls.Interaction.Position.ReadValue<Vector2>());
        if(!isDragging)
            GrabKnife(controls.Interaction.Position.ReadValue<Vector2>());
    }
    
    private void EndTouch(InputAction.CallbackContext context)
    {
        Debug.Log("cancel > "+controls.Interaction.Position.ReadValue<Vector2>());
        if (isDragging)
            ReleaseKnife();
    }

    void Update()
    {
        Debug.Log("isDragging? "+isDragging);
        if (isDragging)
        {
            Debug.Log("drag > " + controls.Interaction.Position.ReadValue<Vector2>());
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
