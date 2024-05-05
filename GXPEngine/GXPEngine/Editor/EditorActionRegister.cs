using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;

namespace GXPEngine.Editor.Exclusives
{
    public static class EditorActionRegister
    {
        static EditorAction[] actions = new EditorAction[128];
        static int currentAction = 0;
        static SceneEditor editor { get { return ((SceneEditor)Game.main); } }
        public static void Clear()
        {
            currentAction = 0;
            actions = new EditorAction[actions.Length];
        }
        public static void AddObject(EditorGameObject Added, string message = "Added new ")
        {
            EditorAction next = new AddNewObject(Added, message);
            next.Execute();
            AddAction(next);
        }
        public static void RemoveObject(EditorGameObject Removed)
        {
            EditorAction next = new RemoveGameObject(Removed);
            next.Execute();
            AddAction(next);
        }
        public static void SetPositionValue(Vector3 newValue, EditorGameObject obj, bool alreadySet = false, Vector3 oldValue = new Vector3())
        {
            EditorAction next = new ChangePositionValue(alreadySet ? oldValue : obj.position, newValue, obj);
            if (!alreadySet) next.Execute();
            AddAction(next);
        }
        public static void SetRotationValue(Quaternion newValue, EditorGameObject obj, bool alreadySet = false, Quaternion oldValue = new Quaternion())
        {
            EditorAction next = new ChangeRotationValue(alreadySet ? oldValue : obj.rotation, newValue, obj);
            if (!alreadySet) next.Execute();
            AddAction(next);
        }
        public static void SetScaleValue(Vector3 newValue, EditorGameObject obj, bool alreadySet = false, Vector3 oldValue = new Vector3())
        {
            EditorAction next = new ChangeScaleValue(alreadySet ? oldValue : obj.scaleXYZ, newValue, obj);
            if (!alreadySet) next.Execute();
            AddAction(next);
        }
        public static void SetConstructorValue(object newValue, int index)
        {
            EditorAction next = new ChangeConstructorValue(newValue, index);
            next.Execute();
            AddAction(next);
        }
        public static void SetFieldValue(object newValue, FieldInfo property, EditorGameObject obj)
        {
            EditorAction next = new ChangeFieldValue(newValue, property, obj);
            next.Execute();
            AddAction(next);
        }
        public static void SetPropertyValue(object newValue, PropertyInfo property, EditorGameObject obj)
        {
            EditorAction next = new ChangePropertyValue(newValue, property, obj);
            next.Execute();
            AddAction(next);
        }
        public static void Undo()
        {
            if (currentAction == 0)
            {
                //actions[currentAction]?.Undo();
                if (actions[currentAction] != null) 
                    Console.WriteLine("whoops! nothing left to undo");
                return;
            }
            currentAction--;
            actions[currentAction].Undo();
        }
        public static void Redo()
        {
            if (actions[currentAction] == null) return;
            Console.Write("Redo: ");
            actions[currentAction].Execute();
            currentAction++;
        }
        static void AddAction(EditorAction action)
        {
            if(currentAction != actions.Length-1)
            {
                actions[currentAction]?.Discard();
                actions[currentAction] = action;
                currentAction++;
            }
            else
            {
                actions[0]?.Complete();
                for(int i = 0; i< actions.Length-1; i++)
                    actions[i] = actions[i+1];
                actions[currentAction-1] = action;
                //Console.WriteLine("gruh"+currentAction);
            }
            //Utils.print(actions);
            //Console.WriteLine(currentAction);
        }


        private class ChangePositionValue : EditorAction
        {
            Vector3 from;
            Vector3 to;
            EditorGameObject inquestion;
            public ChangePositionValue(Vector3 from, Vector3 to, EditorGameObject inquestion)
            {
                this.from = from;
                this.to = to;
                this.inquestion = inquestion;
            }
            public override void Execute()
            {
                inquestion.position = to;
                editor.uiHandler.UpdateGameObjectPropertyMenu();
                Console.WriteLine("set position to " + to);
            }
            public override void Undo()
            {
                inquestion.position = from;
                editor.uiHandler.UpdateGameObjectPropertyMenu();
                Console.WriteLine("Undo: set position");
            }
            public override void Complete() { }
            public override void Discard() { }
        }
        private class ChangeRotationValue : EditorAction
        {
            Quaternion from;
            Quaternion to;
            EditorGameObject inquestion;
            public ChangeRotationValue(Quaternion from, Quaternion to, EditorGameObject inquestion)
            {
                this.from = from;
                this.to = to;
                this.inquestion = inquestion;
            }
            public override void Execute()
            {
                inquestion.rotation = to;
                editor.uiHandler.UpdateGameObjectPropertyMenu();
                Console.WriteLine("set rotation to " + to);
            }
            public override void Undo()
            {
                inquestion.rotation = from;
                editor.uiHandler.UpdateGameObjectPropertyMenu();
                Console.WriteLine("Undo: set rotation");
            }
            public override void Complete() { }
            public override void Discard() { }
        }
        private class ChangeScaleValue : EditorAction
        {
            Vector3 from;
            Vector3 to;
            EditorGameObject inquestion;
            public ChangeScaleValue(Vector3 from, Vector3 to, EditorGameObject inquestion)
            {
                this.from = from;
                this.to = to;
                this.inquestion = inquestion;
            }
            public override void Execute()
            {
                inquestion.scaleXYZ = to;
                editor.uiHandler.UpdateGameObjectPropertyMenu();
                Console.WriteLine("set scale to " + to);
            }
            public override void Undo()
            {
                inquestion.scaleXYZ = from;
                editor.uiHandler.UpdateGameObjectPropertyMenu();
                Console.WriteLine("Undo: set scale");
            }
            public override void Complete() { }
            public override void Discard() { }
        }

        private class ChangeConstructorValue : EditorAction
        {
            object from;
            object to;
            int index;
            EditorGameObject inquestion;
            public ChangeConstructorValue(object to, int index)
            {
                inquestion = editor.selectedGameobject;
                from = inquestion.ConstructorParameters[index]; 
                this.to = to; 
                this.index = index; 
            }
            public override void Execute()
            {
                inquestion.ConstructorParameters[index] = to; 
                inquestion.BuildObject(); 
                editor.uiHandler.UpdateGameObjectPropertyMenu();
                Console.WriteLine("set constructor parameter '" + inquestion.ConstructorParams[index].Name + "' to " + to);
            }
            public override void Undo()
            {
                inquestion.ConstructorParameters[index] = from;
                inquestion.BuildObject();
                editor.uiHandler.UpdateGameObjectPropertyMenu();
                Console.WriteLine("set constructor parameter '" + inquestion.ConstructorParams[index].Name + "'");
            }
            public override void Complete(){}
            public override void Discard(){}
        }
        private class ChangeFieldValue : EditorAction
        {
            object from;
            object to;
            FieldInfo property;
            EditorGameObject inquestion;
            public ChangeFieldValue(object to, FieldInfo field, EditorGameObject inquestion)
            {
                this.to = to;
                this.property = field;
                this.inquestion = inquestion;
                from = field.GetValue(inquestion.EditorDisplayObject);
            }
            public override void Execute()
            {
                property.SetValue(inquestion.EditorDisplayObject, to);
                Console.WriteLine("set field '" + property.Name + "' to " + to);
            }
            public override void Undo()
            {
                property.SetValue(inquestion.EditorDisplayObject, from);
                Console.WriteLine("Undo: set field '" + property.Name + "'");
            }
            public override void Complete() { }
            public override void Discard() { }
        }
        private class ChangePropertyValue : EditorAction
        {
            object from;
            object to;
            PropertyInfo property;
            EditorGameObject inquestion;
            public ChangePropertyValue(object to,  PropertyInfo property, EditorGameObject inquestion)
            {
                this.to = to;
                this.property = property;
                this.inquestion = inquestion;
                from = property.GetValue(inquestion.EditorDisplayObject);
            }
            public override void Execute()
            {
                property.SetValue(inquestion.EditorDisplayObject, to);
                Console.WriteLine("set property '" + property.Name + "' to " + to);
            }
            public override void Undo()
            {
                property.SetValue(inquestion.EditorDisplayObject, from);
                Console.WriteLine("Undo: set property '" + property.Name + "'");
            }
            public override void Complete(){}
            public override void Discard(){}
        }
        private class RemoveGameObject : EditorAction
        {
            EditorGameObject removed;
            GameObject Parent;
            public RemoveGameObject(EditorGameObject removed)
            {
                this.removed = removed;
                Parent = removed.parent;
            }
            public override void Execute()
            {
                editor.selectedGameobject = null;
                removed.Remove();
                Console.WriteLine("removed " + removed.ObjectType.Name);
            }
            public override void Undo()
            {
                editor.selectedGameobject = Parent is EditorGameObject ? (EditorGameObject)Parent : null;
                editor.AddGameObject(removed);
                Console.WriteLine("Undo: removed " + removed.ObjectType.Name);
            }
            public override void Complete()
            {
                removed.Destroy();
            }
            public override void Discard() { }
        }
        private class AddNewObject : EditorAction
        {
            EditorGameObject Added;
            EditorGameObject Parent;
            string Message;
            public AddNewObject(EditorGameObject added, string message = "Added new ")
            {
                Added = added;
                Message = message;
                Parent = editor.selectedGameobject;
            }

            public override void Execute()
            {
                editor.selectedGameobject = Parent;
                editor.AddGameObject(Added);
                Console.WriteLine(Message+Added.ObjectType.Name);
            }
            public override void Undo()
            {
                Added.Remove();
                editor.selectedGameobject = Parent;
                Console.WriteLine("Undo: "+ Message + Added.ObjectType.Name);
            }
            public override void Complete() { }
            public override void Discard()
            {
                Added.Destroy();
            }
        }

        abstract private class EditorAction
        {
            abstract public void Execute();
            abstract public void Undo();
            //complete is when it is finalized, and can never be undone again
            abstract public void Complete();
            //discard is when it is undone, and gets removed by being overriden by another action
            abstract public void Discard();
        }
    }
}
