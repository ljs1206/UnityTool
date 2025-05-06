using System;
using System.Collections.Generic;
using UnityEngine;

namespace LJS.Editing.SOManagement
{
    [Serializable]
    public struct SOInfo
    {
        public ScriptableObject so;
        public string filePath;
    }
    
    [CreateAssetMenu(fileName = "SOTypeTable", menuName = "SO/LJS/Editor/SOTypeTable")]
    public class SOTypeTable : ScriptableObject
    {
        [SerializeField] private List<SOInfo> SOTypeList = new();
        private List<Type> _typeList = new();
        public IReadOnlyList<Type> GetTypeList => _typeList;
        
        public SOTypeTable()
        {
            foreach (var soInfo in SOTypeList)
            {
                if(!_typeList.Contains(soInfo.so.GetType()))
                {
                    _typeList.Add(soInfo.so.GetType());
                }
            }
        }

        private List<Type> _typeArray = new();
        public void OnValidate()
        {
            if(SOTypeList.Count == 0)
            {
                _typeList.Clear();
                return;
            } 
            
            _typeArray.Clear();
            foreach (var soInfo in SOTypeList)
            {
                if(soInfo.so == null) continue;
                Type type = soInfo.so.GetType();
                if (_typeArray.Contains(type))
                {
                    Debug.LogError($"{type.Name} is already registered please not insert this type Scriptable Object");
                    SOTypeList.Remove(soInfo);
                    break;
                }
                _typeArray.Add(type);
                
                if (!_typeList.Contains(type))
                {
                    _typeList.Add(type);
                }
            }
        }

        public string ReturnPath(Type type)
        {
            foreach (var item in SOTypeList)
            {
                if(item.so.GetType() == type) return item.filePath;
            }

            return "";
        }
    }
}
