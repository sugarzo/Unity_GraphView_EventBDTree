using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using SugarFrame.Node;
using System.Linq;
using System;
using log4net.Core;

namespace SugarFrame.Node
{
    public class SearchMenuWindowProvider : ScriptableObject, ISearchWindowProvider
    {
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var entries = new List<SearchTreeEntry>();
            entries.Add(new SearchTreeGroupEntry(new GUIContent("创建新节点")));                //添加了一个一级菜单

            entries.Add(new SearchTreeGroupEntry(new GUIContent("触发器")) { level = 1 });      //添加了一个二级菜单
            var triggers = GetClassList(typeof(BaseTrigger));
            foreach(var trigger in triggers)
            {
                entries.Add(new SearchTreeEntry(new GUIContent(trigger.Name)) { level = 2,userData = trigger });
            }

            entries.Add(new SearchTreeGroupEntry(new GUIContent("行为")) { level = 1 });
            var actions = GetClassList(typeof(BaseAction));
            foreach(var action in actions)
            {
                entries.Add(new SearchTreeEntry(new GUIContent(action.Name)) { level = 2, userData = action });
            }

            entries.Add(new SearchTreeGroupEntry(new GUIContent("分支")) { level = 1 });
            var branchs = GetClassList(typeof(BaseBranch));
            foreach (var action in branchs)
            {
                entries.Add(new SearchTreeEntry(new GUIContent(action.Name)) { level = 2, userData = action });
            }

            entries.Add(new SearchTreeGroupEntry(new GUIContent("序列")) { level = 1 });
            var sq = GetClassList(typeof(BaseSequence));
            foreach (var action in sq)
            {
                entries.Add(new SearchTreeEntry(new GUIContent(action.Name)) { level = 2, userData = action });
            }


            return entries;
        }


        public delegate bool SerchMenuWindowOnSelectEntryDelegate(SearchTreeEntry searchTreeEntry, SearchWindowContext context);            //声明一个delegate类

        public SerchMenuWindowOnSelectEntryDelegate OnSelectEntryHandler;                              //delegate回调方法

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            if (OnSelectEntryHandler == null)
            {
                return false;
            }
            return OnSelectEntryHandler(searchTreeEntry, context);
        }
        private List<Type> GetClassList(Type type)
        {
            var q = type.Assembly.GetTypes()
                .Where(x => !x.IsAbstract)
                .Where(x => !x.IsGenericTypeDefinition)
                .Where(x => type.IsAssignableFrom(x));

            return q.ToList();
        }
    }
}