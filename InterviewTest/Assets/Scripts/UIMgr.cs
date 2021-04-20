using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIBase : MonoBehaviour
{
    public string Identity => GetType().FullName;

    private List<Button> btnEffects = new List<Button>();


    public virtual void OnView()
    {
        gameObject.SetActive(true);
    }

    public virtual void OnDisView()
    {
        gameObject.SetActive(false);
    }

    public virtual void CloseSelf(bool destroy = false)
    {
        UIMgr.Instance.CloseUI(Identity, destroy);
    }
}

public class UIMgr : MonoSingleton<UIMgr>
{
    private const string UI_Root_Path = "Prefabs/UI/";

    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    private Dictionary<string, UIBase> _UICaches = new Dictionary<string, UIBase>();
    public Dictionary<string, UIBase> UICaches
    {
        get
        {
            return _UICaches;
        }
    }
    public GameObject UIRoot;
    public RectTransform UIRoot_Rect;

    public float canvasWidth;
    public float canvasHeight;

    public UIBase TopUI;

    public override void Awake()
    {
        base.Awake();
        if (UIRoot != null) return;
        UIRoot = GameObject.Find("Canvas");
        try
        {
            DontDestroyOnLoad(UIRoot);
        }
        catch (Exception ex)
        {

        }

        UIRoot_Rect = UIRoot.GetComponent<RectTransform>();
        //Debug.Log(UIRoot_Rect.sizeDelta);

        var canvas = UIRoot.GetComponent<Canvas>();
        if (canvas.worldCamera != null)
            DontDestroyOnLoad(canvas.worldCamera.gameObject);
    }

    private void Start()
    {
        canvasWidth = UIRoot.GetComponent<RectTransform>().sizeDelta.x;
        canvasHeight = UIRoot.GetComponent<RectTransform>().sizeDelta.y;
    }

    private void LateUpdate()
    {
        if (disposeDirty)
        {
            Resources.UnloadUnusedAssets();
            disposeDirty = false;
        }
    }

    public T OpenUI<T>(string uiName = "")
        where T : UIBase
    {
        T ui = default;
        if (uiName == "")
        {
            uiName = typeof(T).ToString();
        }

        var parent = UIRoot.transform;

        if (!ContainUI(uiName))
        {
            var go = Instantiate(Resources.Load(UI_Root_Path + uiName) as GameObject, parent);

            if (go == null)
            {
                Debug.LogError("Can't Find UI by Name = " + uiName);
                return null;
            }

            ui = go.GetComponent<T>();
            _UICaches.Add(uiName, ui);
        }
        else
        {
            ui = _UICaches[uiName] as T;
        }

        if (ui == null) return ui;
        ui.transform.SetSiblingIndex(parent.childCount - 1);
        ui.OnView();
        FindTopUI();

        //UpdateUIShow(b);

        return ui;
    }

    public void CloseUI(string uiName, bool Destroy = false)
    {
        if (!ContainUI(uiName)) return;
        var ui = _UICaches[uiName];
        ui.OnDisView();

        if (Destroy)
            DisposeUI(uiName);

        FindTopUI();
    }

    bool disposeDirty = false;
    void DisposeUI(string uiName)
    {
        if (!ContainUI(uiName)) return;
        var ui = _UICaches[uiName];
        ui.OnDisView();

        _UICaches.Remove(uiName);
        GameObject.DestroyImmediate(ui.gameObject);

        disposeDirty = true;
        FindTopUI();
    }

    private void FindTopUI()
    {
        for (var i = UIRoot.transform.childCount - 1; i >= 0; i--)
        {
            if (UIRoot.transform.GetChild(i).GetComponent<UIBase>() == null ||
                !UIRoot.transform.GetChild(i).gameObject.activeSelf) continue;
            if ("UI_RewardPanel".Equals(UIRoot.transform.GetChild(i).GetComponent<UIBase>().Identity))
            {
                continue;
            }
            TopUI = UIRoot.transform.GetChild(i).GetComponent<UIBase>();
            break;
        }
    }

    private bool ContainUI(string uiName)
    {
        return _UICaches.ContainsKey(uiName);
    }

    public T FindUI<T>()
        where T : UIBase
    {
        var uiName = typeof(T).ToString();
        if (ContainUI(uiName))
        {
            return _UICaches[uiName] as T;
        }

        return default;
    }

    public T FindUI<T>(string _uiName)
       where T : UIBase
    {
        if (_uiName == "")
        {
            _uiName = typeof(T).ToString();
        }
        if (ContainUI(_uiName))
        {
            return _UICaches[_uiName] as T;
        }
        if (ContainUI("Guide/UI_" + _uiName))
        {
            return _UICaches["Guide/UI_" + _uiName] as T;
        }

        return default;
    }

    public int ComparerIndex<T1, T2>()
        where T1 : UIBase
        where T2 : UIBase
    {
        string uiName1 = typeof(T1).ToString();
        string uiName2 = typeof(T2).ToString();
        if (!ContainUI(uiName1))
        {
            return 1;
        }
        if (!ContainUI(uiName2))
        {
            return 0;
        }
        if (FindUI<T1>().transform.GetSiblingIndex() > FindUI<T2>().transform.GetSiblingIndex())
        {
            return 0;
        }
        if (FindUI<T1>().transform.GetSiblingIndex() > FindUI<T2>().transform.GetSiblingIndex())
        {
            return 1;
        }
        return -1;
    }

    private List<string> ignoreView = new List<string> {
        "EventSystem",
    };

    public void ClearAllUI()
    {
        var uiRoot = Instance.UIRoot.transform;
        for (var i = uiRoot.childCount - 1; i >= 0; i--)
        {
            var child = uiRoot.GetChild(i);
            var indexOf = ignoreView.IndexOf(child.name);
            if (indexOf == -1)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        _UICaches.Clear();
        TopUI = null;
    }

    #region Static 方法

    public static T Open<T>(string uiName = "") where T : UIBase
    {
        return Instance.OpenUI<T>(uiName);
    }

    public static T Find<T>(string uiName = "") where T : UIBase
    {
        return Instance.FindUI<T>(uiName);
    }


    #endregion
}