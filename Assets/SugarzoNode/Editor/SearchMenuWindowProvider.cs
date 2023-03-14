using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Linq;
using System;

namespace SugarFrame.Node
{
    public class SearchMenuWindowProvider : ScriptableObject, ISearchWindowProvider
    {
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var entries = new List<SearchTreeEntry>();
            //添加一级菜单
            entries.Add(new SearchTreeGroupEntry(new GUIContent("创建新节点")));
            //获取所有脚本列表
            var triggers = GetClassList(typeof(BaseTrigger));
            var actions = GetClassList(typeof(BaseAction));
            var branchs = GetClassList(typeof(BaseBranch));
            var sequences = GetClassList(typeof(BaseSequence));

            //根据Type尝试获取节点名称&说明
            Func<Type, string> GetNodeTitle = (type) =>
            {
                string str = type.Name;
                if(type.IsDefined(typeof(NodeNoteAttribute),true))
                    str += " " + (System.Attribute.GetCustomAttribute(type, typeof(NodeNoteAttribute)) as NodeNoteAttribute).note;
                return str;
            };
            //获取节点隶属包
            Func<Type, string> GetNodePackage = (type) =>
            {
                string str = "UnityBase";
                if (type.IsDefined(typeof(NodeNoteAttribute), true))
                    str = (System.Attribute.GetCustomAttribute(type, typeof(NodeNoteAttribute)) as NodeNoteAttribute).packageType;
                return str;
            };

            //遍历所有包类型
           NodePackageType.GetAllTypeStr().ForEach(packageStr => {
                entries.Add(new SearchTreeGroupEntry(new GUIContent(packageStr)) { level = 1 });

                bool hasItem = false; //检查该分类下是否有3级目录内容，如果没有就不需要创建2级目录了
                foreach (var item in triggers)
                {
                    if(GetNodePackage(item).Equals(packageStr))
                    {
                        if(hasItem == false)
                            entries.Add(new SearchTreeGroupEntry(new GUIContent("触发器")) { level = 2 });
                        hasItem = true;
                        entries.Add(new SearchTreeEntry(new GUIContent(GetNodeTitle(item))) { level = 3, userData = item });
                    }  
                }

                hasItem = false;
                foreach (var item in actions)
                {
                    if (GetNodePackage(item).Equals(packageStr))
                    {
                        if (hasItem == false)
                            entries.Add(new SearchTreeGroupEntry(new GUIContent("行为")) { level = 2 });
                        hasItem = true;
                        entries.Add(new SearchTreeEntry(new GUIContent(GetNodeTitle(item))) { level = 3, userData = item });
                    }
                }

                hasItem = false;
                foreach (var item in branchs)
                {
                    if (GetNodePackage(item).Equals(packageStr))
                    {
                        if (hasItem == false)
                            entries.Add(new SearchTreeGroupEntry(new GUIContent("分支")) { level = 2 });
                        hasItem = true;
                        entries.Add(new SearchTreeEntry(new GUIContent(GetNodeTitle(item))) { level = 3, userData = item });
                    }
                }

                hasItem = false;
                foreach (var item in sequences)
                {
                    if (GetNodePackage(item).Equals(packageStr))
                    {
                        if (hasItem == false)
                            entries.Add(new SearchTreeGroupEntry(new GUIContent("序列")) { level = 2 });
                        hasItem = true;
                        entries.Add(new SearchTreeEntry(new GUIContent(GetNodeTitle(item))) { level = 3, userData = item });
                    }
                }
            });

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