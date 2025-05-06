using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace LJS.Editing.SOManagement
{
    public class CheckingWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset m_VisualTreeAsset = default;

        private Label _description;
        private Action _acceptAction;

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
            root.Add(labelFromUXML);

            _description = root.Q<Label>("Description");
            root.Q<Button>("AcceptBtn").clicked += HandleAcceptButtonClickEvent;
            root.Q<Button>("RejectBtn").clicked += HandleRejectButtonClickEvent;
        }

        private void HandleRejectButtonClickEvent() => Close();

        private void HandleAcceptButtonClickEvent()
        {
            _acceptAction?.Invoke();
            Close();
        }

        public void SetInfo(string description, Action acceptAction)
        {
            _description.text = description;
            _acceptAction = acceptAction;
        }
    }
}