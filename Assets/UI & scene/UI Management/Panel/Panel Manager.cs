using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager
{
    private Stack<BasePanel> panel_stack;
    private UIManager ui_manager;

    private BasePanel panel;

    public PanelManager()
    {
        panel_stack = new Stack<BasePanel>();
        ui_manager = new UIManager();
    }

    public void Push(BasePanel next_panel)
    {
        if (panel_stack.Count > 0)
        {
            panel_stack.Peek().OnPause();
        }
        panel_stack.Push(next_panel);
        GameObject panel_game = ui_manager.GetSingleUI(next_panel.ui_type);
        next_panel.ui_tool = new UItools(panel_game);
        next_panel.panel_manager = this;
        next_panel.ui_manager = ui_manager;
        next_panel.OnAwake();
    }
    public void Pop()
    {
        if (panel_stack.Count > 0)
        {
            panel_stack.Peek().OnSleep();
            panel_stack.Pop();
        }
        if (panel_stack.Count > 0)
        {
            panel_stack.Peek().OnResume();
        }
    }

    public string CurrentActivePanel()
    {
        return panel_stack.Peek().ui_type.Name;
    }

    public BasePanel GetPeek()
    {
        return panel_stack.Peek();
    }

    public void Clear()
    {
        while (panel_stack.Count != 0)
        {
            Pop();
        }
    }

    public bool Empty()
    {
        return panel_stack.Count == 0;
    }
}


