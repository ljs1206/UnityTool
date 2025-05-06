using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static Define;

public class PrefabManager : EditorWindow
{
    private readonly string _prefabSavePath = "Assets/Test/CreatePrefabWindow/03_Prefab/";
    
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;
    [SerializeField]
    private PrefabTableSO _prefabTable;
    
    private VisualElement _root;

    [MenuItem("Editor/Prefab/PrefabManager")]
    public static void CShowWindow()
    {
        PrefabManager wnd = GetWindow<PrefabManager>();
        wnd.titleContent = new GUIContent("PrefabManager");
        wnd.minSize = new Vector2(600, 800);
        wnd.maxSize = new Vector2(600, 800);
    }

    #region Elements
    private EnumField _viewType;
    private TextField _nameField2D;
    private ObjectField _visualField2D;
    private ObjectField _statField2D;
    private EnumField _aiType2D;
    private EnumField _colliderType2D;
    private TextField _nameField3D;
    private ObjectField _visualField3D;
    private ObjectField _statField3D;
    private EnumField _aiType3D;
    private EnumField _colliderType3D;
    #endregion
    
    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        _root = rootVisualElement;

        // Instantiate UXML
        VisualElement labelFromUxml = m_VisualTreeAsset.Instantiate();
        _root.Add(labelFromUxml);
        labelFromUxml.style.flexGrow = 1;
        
        _viewType = _root.Q<EnumField>("ViewEnum");
        VisualElement element2D = _root.Q<VisualElement>("2DSetting");
        VisualElement element3D = _root.Q<VisualElement>("3DSetting");
        Button makeBtn = _root.Q<Button>("CreateBtn");

        #region 2D
        _nameField2D = element2D.Q<TextField>("NameTag");
        _visualField2D = element2D.Q<ObjectField>("VisualField");
        _statField2D = element2D.Q<ObjectField>("StatField");
        _aiType2D = element2D.Q<EnumField>("AIType");
        _colliderType2D = element2D.Q<EnumField>("ColliderType");
        #endregion
        
        #region 3D
        _nameField3D = element3D.Q<TextField>("NameTag");
        _visualField3D = element3D.Q<ObjectField>("VisualField");
        _statField3D = element3D.Q<ObjectField>("StatField");
        _aiType3D = element3D.Q<EnumField>("AIType");
        _colliderType3D = element3D.Q<EnumField>("ColliderType");
        #endregion
        
        DefaultPrefabInfo2D prefabInfo2D = new();
        DefaultPrefabInfo3D prefabInfo3D = new();
        
        // Prefab의 2차원으로 만들지 3차원으로 만들지 Enum으로 Switch로 분류하는 Event이다.
        _viewType.RegisterValueChangedCallback(evt => {
            switch (_viewType.value)
            {
                case ViewSetting.None:
                    element2D.style.display = DisplayStyle.None;
                    element3D.style.display = DisplayStyle.None;
                    break;
                case ViewSetting.View2D:
                    element2D.style.display = DisplayStyle.Flex;
                    element3D.style.display = DisplayStyle.None;
                    break;
                case ViewSetting.View3D:
                    element3D.style.display = DisplayStyle.Flex;
                    element2D.style.display = DisplayStyle.None;
                    break;
            }
        });

        makeBtn.clicked += ClickMakeBtnEvent;
    }

    private void ClickMakeBtnEvent()
    {
        if((ViewSetting)_viewType.value == ViewSetting.None) return;
        
        GameObject prefab;
        bool isSuccess, failCreate = false;
        Guid id = Guid.NewGuid();
        
        GameObject obj = new GameObject();  
        obj.name = id.ToString();
        
        switch (_viewType.value)
        {
            case ViewSetting.View2D:
                _prefabTable.prefabList.ForEach(x =>
                {
                    if (x.name == _nameField2D.value)
                    {
                        Debug.LogWarning($"you make already prefab please change your prefab name.... target : {x.name}");
                        DestroyImmediate(obj);
                        failCreate = true;
                    }
                });
                if(failCreate == true) return;
                
                #region SettingValue2D
                obj.AddComponent<DefaultPrefabInfo2D>();
                prefab = PrefabUtility.SaveAsPrefabAsset(obj,
                    $"{_prefabSavePath}{_nameField2D.value}.prefab" , out isSuccess);
                prefab.name = _nameField2D.value;

                DefaultPrefabInfo2D info2D = prefab.GetComponent<DefaultPrefabInfo2D>();
                info2D.VisualPrefab = _visualField2D.value as GameObject;
                SetAICompo(prefab);
                SetColliderCompo(prefab);
                info2D.AIType = (AIType)_aiType2D.value;
                info2D.ColliderType = (ColliderType2D)_colliderType2D.value;
                #endregion
                break;
            case ViewSetting.View3D:
                _prefabTable.prefabList.ForEach(x =>
                {
                    if (x.name == _nameField3D.value)
                    {
                        Debug.LogWarning($"you make already prefab please change your prefab name.... target : {x.name}");
                        DestroyImmediate(obj);
                        failCreate = true;
                    }
                });
                if(failCreate == true) return;
                
                #region SettingValue3D
                obj.AddComponent<DefaultPrefabInfo3D>();
                prefab = PrefabUtility.SaveAsPrefabAsset(obj,
                    $"{_prefabSavePath}{_nameField3D.value}.prefab" , out isSuccess);
                prefab.name = _nameField3D.value;
                
                DefaultPrefabInfo3D info3D = prefab.GetComponent<DefaultPrefabInfo3D>();
                info3D.VisualPrefab = _visualField3D.value as GameObject;
                SetAICompo(prefab);
                SetColliderCompo(prefab);
                info3D.AIType = (AIType)_aiType3D.value;
                info3D.ColliderType = (ColliderType3D)_colliderType3D.value;
                
                #endregion
                break; 
            default:
                DestroyImmediate(obj);
                return;
        }
        
        _prefabTable.prefabList.Add(prefab);
        
        if(isSuccess)
        {
            MakePrefabWindow._ShowItemEvent.Invoke(prefab.name);
            Debug.Log($"Success Create Prefab \n Name : {id} \nPath : {_prefabSavePath}{id}");
        }
        else{
            Debug.Log("Failure Create Prefab");
        }
        
        EditorUtility.SetDirty(_prefabTable);
        AssetDatabase.SaveAssets();
        DestroyImmediate(obj);
    }

    private void SetAICompo(GameObject obj)
    {
        if ((ViewSetting)_viewType.value == ViewSetting.View2D)
        {
            switch (_aiType2D.value)
            {
                case AIType.None:
                    return;
                case AIType.BT:
                    // Add BT Compo
                    Debug.Log("bt");
                    obj.AddComponent<BehaviourTreeRunner>();
                    return;
                case AIType.FSM:
                    // Add FSM Compo
                    return;
            }
        }
        else if ((ViewSetting)_viewType.value == ViewSetting.View3D)
        {
            switch (_aiType3D.value)
            {
                case AIType.None:
                    return;
                case AIType.BT:
                    // Add BT Compo
                    Debug.Log("bt");
                    obj.AddComponent<BehaviourTreeRunner>();
                    return;
                case AIType.FSM:
                    // Add FSM Compo
                    return;
            }
        }
    }
    
    private void SetColliderCompo(GameObject obj)
    {
        if ((ViewSetting)_viewType.value == ViewSetting.View2D)
        {
            switch (_colliderType2D.value)
            {
                case ColliderType2D.None:
                    return;
                case ColliderType2D.Box:
                    Debug.Log("Box");
                    obj.AddComponent<BoxCollider2D>();
                    return;
                case ColliderType2D.Capsule:
                    obj.AddComponent<CapsuleCollider2D>();
                    return;
                case ColliderType2D.Circle:
                    obj.AddComponent<CapsuleCollider2D>();
                    return;
            }
        }
        else if ((ViewSetting)_viewType.value == ViewSetting.View3D)
        {
            switch (_colliderType3D.value)
            {
                case ColliderType3D.None:
                    return;
                case ColliderType3D.Box:
                    obj.AddComponent<BoxCollider>();
                    return;
                case ColliderType3D.Capsule:
                    obj.AddComponent<CapsuleCollider>();
                    return;
                case ColliderType3D.Sphere:
                    obj.AddComponent<SphereCollider>();
                    return;
            }
        }
    }
}
