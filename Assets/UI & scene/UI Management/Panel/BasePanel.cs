using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel
{
    public BasePanel(UIType uiType) { ui_type = uiType; }
    public UIType ui_type { get; private set; }
    public UItools ui_tool { get; set; }
    public PanelManager panel_manager { get; set; }
    public UIManager ui_manager { get; set; }
    public virtual void OnAwake() { }
    public virtual void OnSleep() { ui_manager.Destroy_UI(this.ui_type); }
    public virtual void OnPause() { ui_tool.GetOrAddComponent<CanvasGroup>().interactable = false; }
    public virtual void OnResume() { ui_tool.GetOrAddComponent<CanvasGroup>().interactable = true; }
}
