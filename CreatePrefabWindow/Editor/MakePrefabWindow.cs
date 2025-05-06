using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static Define;

public class MakePrefabWindow : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;
    [SerializeField]
    private StyleSheet _styleSheet;
    [SerializeField]
    private PrefabTableSO _prefabTable;

    public static Action<string> _ShowItemEvent;

    private readonly string _prefabVisual = "prefab-visual";
    private readonly string _prefabLabel = "prefab-label";
    private readonly string _prefabVisualSelect = "prefab-visual-select";
    private readonly string _prefabFilePath = "Assets/Test/CreatePrefabWindow/03_Prefab";

    [MenuItem("Editor/Prefab/MakePrefabWindow")]
    public static void PShowWindow()
    {
        MakePrefabWindow wnd = GetWindow<MakePrefabWindow>();
        wnd.titleContent = new GUIContent("MakePrefabWindow");
        wnd.minSize = new Vector2(800, 600);
        wnd.maxSize = new Vector2(800, 600);
    }

    // 생성할때 Event 함수 구독
    private void OnEnable()
    {
        _ShowItemEvent += ViewItem;
    }

    // 생성할때 Event 함수 구독 취소
    private void OnDisable()
    {
        _ShowItemEvent -= ViewItem;
    }

    private VisualElement _root;
    private VisualElement _prefabView;
    private Label selectedLabel;
    private TextField fileNameField;

    protected Dictionary<string, Label> _viewLableDictionary;
    
    [HideInInspector] public VisualElement _selected;

    // 기본 생성 로직 및 이벤트 구독 처리 함수
    public void CreateGUI()
    {
        _viewLableDictionary = new();
        _root = rootVisualElement;
        
        VisualElement labelFromUxml = m_VisualTreeAsset.Instantiate();
        labelFromUxml.style.flexGrow = 1;
        _root.Add(labelFromUxml);

        #region GetObject
        
        Button makeBtn = _root.Q<Button>("MakeBtn");
        Button deleteBtn = _root.Q<Button>("DeleteBtn");
        _prefabView = _root.Q<VisualElement>("PrefabView");
        EnumField viewType = _root.Q<EnumField>("ViewSetting");
        fileNameField = _root.Q<TextField>("FileName");
        selectedLabel = _root.Q<Label>("NameLabel");

        #endregion
        
        // ViewItem을 Table에 들어있는 Prefab의 갯수만큼 실행
        if (_prefabTable.prefabList.Count > 0)
        {
            foreach(var item in _prefabTable.prefabList){
                ViewItem(item.name);
            }
        }
        
        // 선택된 Prefab의 file 이름을 변경하는 Event이다.
        fileNameField.RegisterValueChangedCallback(evt =>
        {
            if(_selected == null) return;
            
            _selected.Q<Label>("label").text = fileNameField.text;
            
            AssetDatabase.RenameAsset($"{_prefabFilePath}/{_selected.name}.prefab",
                fileNameField.text);
            _selected.name = fileNameField.text;
        });
    
        // BTN Event
        makeBtn.clicked += HandleMakeBtnCllikEvent;
        deleteBtn.clicked += HandleDeleteBtnClickEvent;
    }
    
    // TableSO에 들어있는 Prefab을 VisualElement으로 표현하는 함수이다.
    public void ViewItem(string vName){
        VisualElement element = new VisualElement();
        element.AddToClassList(_prefabVisual);
        element.name = vName;
        
        // Mouse Down Event
        element.RegisterCallback<PointerDownEvent>(ElementPointerDownEvent);

        Label label = new Label();
        label.name = "label";
        label.AddToClassList(_prefabLabel);
        label.text = vName;
        
        _viewLableDictionary.Add(vName, label);
        element.Add(label);
        _prefabView.Add(element);
    }

    // Element 위에서 마우스 클릭이 이루어 졌다면
    private void ElementPointerDownEvent(PointerDownEvent evt)
    {
        foreach(var item in _prefabTable.prefabList){
            VisualElement element = _root.Q<VisualElement>(item.name);
            List<string> classNames = element.GetClasses().ToList();
            
            // 모든 SelectEffect가 적용된 VisualElement에 Effect를 제거한다.
            foreach(string str in classNames){
                if(str == _prefabVisualSelect){
                    element.AddToClassList(_prefabVisual);
                    element.RemoveFromClassList(_prefabVisualSelect);
                }
            }
            
            // 클릭한 위치의 VisualElement와 현재 Element가 같다면? 그 VisualElement에 Effect를 부여하고
            // Toolbar의 Label과 FileNameField(선택된 Prefab의 FileName을 표기하는 TextField이다.)의 Value를 바꾸어 준다.
            if(evt.currentTarget.GetHashCode() == element.GetHashCode()){
                _selected = element;
                element.RemoveFromClassList(_prefabVisual);
                element.AddToClassList(_prefabVisualSelect);
                
                selectedLabel.text = element.name;
                fileNameField.value = element.name;
            }
        }
    }
    
    // prefab 생성 함수
    private void HandleMakeBtnCllikEvent()
    {
        PrefabManager wnd = GetWindow<PrefabManager>();
        wnd.titleContent = new GUIContent("PrefabManager");
        wnd.minSize = new Vector2(600, 800);
    }
    
    // prefab 삭제 함수
    private void HandleDeleteBtnClickEvent()
    {
        GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>($"{_prefabFilePath}/{_selected.name}.prefab");

        VisualElement deleteElement = _prefabView.Q<VisualElement>(_selected.name);
        _prefabView.Remove(deleteElement);

        _prefabTable.prefabList.Remove(obj);
        AssetDatabase.DeleteAsset($"{_prefabFilePath}/{_selected.name}.prefab");
        EditorUtility.SetDirty(_prefabTable);
        AssetDatabase.SaveAssets();
    }
}
