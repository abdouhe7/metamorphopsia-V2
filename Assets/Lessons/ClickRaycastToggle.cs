using UnityEngine;
using UnityEngine.InputSystem;

public class ClickRaycastToggle : MonoBehaviour
{
    void Update()
    {
        if (Mouse.current == null) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // مهم: إذا ضغطتِ على النص، نطلع للأب (اللوحة)
            var toggle = hit.transform.GetComponentInParent<ToggleHelloPanel>();
            if (toggle != null)
                toggle.Toggle();
        }
    }
}
