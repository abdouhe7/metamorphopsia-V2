using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    private Dictionary<UIType, GameObject> UI_dic;

    public UIManager()
    {
        UI_dic = new Dictionary<UIType, GameObject>();
    }
    public GameObject GetSingleUI(UIType ui_type)
    {
        GameObject parent = GameObject.Find("Canvas");
        if (!parent)
        {
            Debug.LogError("there is no canvas, please check.");
            return null;
        }
        if (UI_dic.ContainsKey(ui_type))
        {
            return UI_dic[ui_type];
        }

        GameObject ui = GameObject.Instantiate(Resources.Load<GameObject>(ui_type.Path), parent.transform);
        ui.name = ui_type.Name;
        UI_dic.Add(ui_type, ui);

        return ui;
    }
    public void Destroy_UI(UIType ui_type)
    {
        if (UI_dic.ContainsKey(ui_type))
        {
            GameObject.Destroy(UI_dic[ui_type]);
            UI_dic.Remove(ui_type);
        }
    }
}
