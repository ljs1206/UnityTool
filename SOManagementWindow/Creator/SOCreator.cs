using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace LJS.Editing.SOManagement
{
    public class SOCreator : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        private Action<ScriptableObject> _endCallback;

        #region MyElements
        
            private TextField _nameField;
            private Button _createBtn;
            private Button _cancelBtn;
            
        #endregion

        /// <summary>
        /// 생성할 SO의 Type
        /// </summary>
        private Type _createSOType;
        /// <summary>
        /// 생성할 위치
        /// </summary>
        private string _createPath;

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            root.style.flexGrow = 1;
            
            VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
            root.Add(labelFromUXML);
            
            _nameField = root.Q<TextField>("NameField");
            _createBtn = root.Q<Button>("CreateBtn");
            _cancelBtn = root.Q<Button>("CancelBtn");

            _createBtn.clicked += HandleCreateBtnClickEvent;
            _cancelBtn.clicked += HandleCancelBtnClickEvent;
        }

        /// <summary>
        /// 취소 버튼 클릭 이벤트
        /// </summary>
        private void HandleCancelBtnClickEvent()
        {
            Close();
        }

        /// <summary>
        /// 생성 버튼 클릭 이벤트
        /// </summary>
        private void HandleCreateBtnClickEvent()
        {
            if (_createPath.Length <= 0)
            {
                _createPath = "Assets";
            }
            
            ScriptableObject newItem = CreateInstance(_createSOType);
            Guid typeGuid = Guid.NewGuid();
            if (_nameField.text.Length == 0)
            {
                newItem.name = typeGuid.ToString();
                Debug.Log($"a random GUID was assigned due to the absence of input.");
                Debug.Log($"random GUID : {typeGuid}");
            }
            else
            {
                newItem.name = _nameField.text;
            }
            
            AssetDatabase.CreateAsset(newItem,
                $"{_createPath}/{newItem.name}.asset");
            
            Debug.
                Log($"Success Create SO, Name : {newItem.name} Path : {_createPath}/{newItem.name}.asset");
            
            AssetDatabase.SaveAssets();
            
            _endCallback?.Invoke(newItem);
            Close();
        }

        public void SettingInfo(Type createType, string path, Action<ScriptableObject> endCallback)
        {
            _createSOType = createType;
            _createPath = path;
            _endCallback = endCallback;
        }

        private void OnDisable()
        {
            _createBtn.clicked -= HandleCreateBtnClickEvent;
            _cancelBtn.clicked -= HandleCancelBtnClickEvent;
        }
    }
}
