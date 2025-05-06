using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public class CreateItemWindow : OdinMenuEditorWindow
{ 
    private readonly string _itemSODirectory = "Assets/00.Work/LJS/Editor/CreateSO";
    private readonly string _itemSOTableDirectory = "Assets/00.Work/PJH/05.SO/Item Data/ItemDataList.asset";
    private readonly string _SODirectory = "Assets/00.Work/PJH/05.SO/Item Data/Cloth";
    private readonly string defaultName = "CreateStaffSO";
    private ItemDataSO emptySO;
    private ItemDataSOList _itemList;

    public static void OpenWindow(){
        var window = GetWindow<CreateItemWindow>();
        window.minSize = new Vector2(700f, 400f);
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700f, 400f);
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        _itemList = AssetDatabase.LoadAssetAtPath<ItemDataSOList>(_itemSOTableDirectory);
        emptySO = CreateInstance("ItemDataSO") as ItemDataSO;
        var tree = new OdinMenuTree();
        tree.AddAllAssetsAtPath("Create Staff Type", _itemSODirectory, typeof(ItemDataSO));
        return tree;
    }

    protected override void OnBeginDrawEditors()
    {
        OdinMenuTreeSelection selected = this.MenuTree.Selection;
        
        if(selected.SelectedValue != null){
            Texture2D noneIcon = EditorIcons.TestNormal;
            if(SirenixEditorGUI.MenuButton(0, "Create", false, noneIcon)){
                ItemDataSO selectSOValue = selected.SelectedValue as ItemDataSO;

                if(selectSOValue.itemName == defaultName) {
                    Debug.LogWarning("can't use dedault Name plase Change Name");
                    base.OnBeginDrawEditors();
                    return;
                }

                var obj = ScriptableObject.CreateInstance<ItemDataSO>();
                GUID id = GUID.Generate();

                AssetDatabase.CreateAsset(obj, $"{_SODirectory}/Item_{selectSOValue.itemName}_{id}.asset");
                obj.itemName = selectSOValue.itemName;
                obj.itemId = selectSOValue.itemId;
                obj.icon = selectSOValue.icon;
                obj.price = selectSOValue.price;

                _itemList.items.Add(obj);

                AssetDatabase.SaveAssets();
            }
        }

        base.OnBeginDrawEditors();
    }
}
