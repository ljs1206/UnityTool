using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace LJS.Editing.SOManagement
{
    public class WindowSearchSystem
    {
        private ToolbarPopupSearchField _searchField;
        private ScriptableObject[] _searchFieldContainer;

        public event Action<ScriptableObject[]> OnSearchFieldValueChangedAction;

        public void SetupSearchSystem(ToolbarPopupSearchField searchField, ScriptableObject[] dataArray)
        {
            _searchField = searchField;
            _searchFieldContainer = dataArray;

            _searchField.RegisterValueChangedCallback(HandleCurrentValueChangeEvent);
        }

        private void HandleCurrentValueChangeEvent(ChangeEvent<string> evt)
        {
            List<ScriptableObject> containElementLists = new(_searchFieldContainer.Length);
            if (evt.newValue == "")
            {
                OnSearchFieldValueChangedAction?.Invoke(_searchFieldContainer);
                return;
            }
        
            for (int i = 0; i < _searchFieldContainer.Length; i++)
            {
                if (_searchFieldContainer[i].name.Contains(evt.newValue))
                {
                    containElementLists.Add(_searchFieldContainer[i]);
                }
            }
            OnSearchFieldValueChangedAction?.Invoke(containElementLists.ToArray());
        }
    }
}
