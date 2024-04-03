using GXPEngine.Core;
using GXPEngine.Editor;
using GXPEngine.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.Editor
{
    public class HierarchyItem : Panel
    {
        public static Dictionary<GameObject,HierarchyItem> references = new Dictionary<GameObject,HierarchyItem>();

        EditorGameObject gameObject;
        /// <summary>
        /// implement selecting objects from hierarchy as objectProxy.OnClick += SelectObject(gameObject)
        /// </summary>
        TextButton objectProxy;
        TextButton hideChildrenButton;
        TextPanel childrenCounter;
        int childrenCount;
        int iteration = 0;
        bool hideChildren = false;
        public List<HierarchyItem> children;
        public HierarchyItem(EditorGameObject gameObject, int iteration, int width, float x = 0, float y = 0) : base(width, 20, x, y, false)
        {
            this.gameObject = gameObject;
            this.iteration = iteration;
            if (!references.Keys.Contains(gameObject))
                references.Add(gameObject, this);
            color = 0xff00ffff;
            hideChildrenButton = new TextButton(20, 20, "↓", 10);
            hideChildrenButton.SetXY(0, 0, 0);
            hideChildrenButton.OnClick += ToggleChildren;
            AddChild(hideChildrenButton);

            objectProxy = new TextButton(width-25, 20, gameObject.ObjectType.Name, 10);
            objectProxy.SetXY(25, 0, 0);
            if (iteration < 16)
                objectProxy.color = 0xffffffff - (uint)(0x00000011 * iteration);
            else
                objectProxy.color = 0xffffff00;
            AddChild(objectProxy);

            children = new List<HierarchyItem>();

            childrenCounter = new TextPanel(20, 20, "0", 10,false);
            UpdateCounter(0);
            childrenCounter.SetXY(width-20,0,0);
            AddChild(childrenCounter);

            Console.WriteLine(references.Count);
        }
        void ToggleChildren()
        {
            hideChildren = !hideChildren;
            if (hideChildren) hideChildrenButton.color = 0xff9999ff;
            else hideChildrenButton.color = 0xffffffff;
        }
        public void UpdateDisplay()
        {
            GetContentHeight();
        }
        public int GetContentHeight()
        {
            int res = 20;
            if (children != null && children.Count > 0 && !hideChildren)
            {
                foreach (HierarchyItem item in children)
                {
                    item.y = y + res + 5;
                    item.x = x + 10;
                    res += item.GetContentHeight() + 5;
                }
            }

            if (height != res)
            {
                Resize(width, res);
            }
            return res;
        }
        public void UpdateCounter(int num)
        {
            childrenCount = num;
            childrenCounter.ClearTransparent();
            childrenCounter.Text(num.ToString(),5,10);
        }
        public void ReadChildren()
        {
            List<GameObject> kiddos = gameObject.GetChildren();
            if (kiddos.Count == 0)
            {
                UpdateCounter(kiddos.Count);
                return;
            }

            foreach (GameObject child in gameObject.GetChildren())
            {
                if (!(child is EditorGameObject)) continue;
                HierarchyItem childItem = new HierarchyItem((EditorGameObject)child,iteration+1, width-10, 10, 25);
                childItem.ReadChildren();
                children.Add(childItem);
                childItem.parent = this.parent;
                //AddChild(childItem);
            }
            if (childrenCount != kiddos.Count)
                UpdateCounter(kiddos.Count);
            GetContentHeight();
        }
        public void UpdateChildren()
        {
            if (hideChildren) return;
            //check each child in hierarchy, if there are some impostors in the scene that are not parented to the corresponding gameobject, remove them
            foreach (HierarchyItem child in children)
                if (child.gameObject.parent != gameObject)
                    UnregisterChild(child.gameObject, child);
            //opposite process here
            //check each child in scene, if there are some impostors in the hierarchy that are not parented to the corresponding item, add them
            foreach (GameObject child in gameObject.GetChildren())
                if (!references.Keys.Contains(child) && child is EditorGameObject)
                    RegisterChild((EditorGameObject)child, this);

            foreach (HierarchyItem child in children)
                child.UpdateChildren();
        }
        public static void RegisterChild(EditorGameObject gameObject, HierarchyItem parentItem = null)
        {
            if (parentItem == null)
                parentItem = references[gameObject.parent];
            HierarchyItem childItem = new HierarchyItem(gameObject, parentItem.iteration + 1, parentItem.width-10, 10, 25);
            parentItem.children.Add(childItem);
            childItem.parent = parentItem.parent;
            //parentItem.AddChild(childItem);
        }

        public static void UnregisterChild(GameObject gameObject, HierarchyItem item = null)
        {
            if (item == null)
                item = references[gameObject];
            HierarchyItem parentItem = (HierarchyItem)item.parent.parent;
            parentItem.children.Remove(item);
            item.LateDestroy();
            references.Remove(gameObject);
        }
    }
}
