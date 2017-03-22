﻿#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.EditorVR.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.Experimental.EditorVR.Workspaces
{
	[MainMenuItem("Locked Objects", "Workspaces", "View all locked objects in your scene(s)")]
	class LockedObjectsWorkspace : HierarchyWorkspace, IUsesGameObjectLocking
	{
		[SerializeField]
		GameObject m_UnlockAllPrefab;

		string baseSearchQuery;
		string m_CachedSearchQuery;

		public override string searchQuery
		{
			get
			{
				var query = base.searchQuery;
				if (baseSearchQuery != query)
				{
					baseSearchQuery = query;
					m_CachedSearchQuery = string.Format("{0} {1}", baseSearchQuery, k_LockedQuery);
				}

				return m_CachedSearchQuery;
			}
		}

		public override List<string> filterList
		{
			set
			{
				m_FilterList = value;
				m_FilterList.Sort();
				
				if (m_FilterUI)
					m_FilterUI.filterList = value;
			}
		}

		public Action<GameObject, bool> setLocked { get; set; }
		public Func<GameObject, bool> isLocked { get; set; }

		public override void Setup()
		{
			base.Setup();

			if (m_UnlockAllPrefab)
			{
				var unlockAllUI = ObjectUtils.Instantiate(m_UnlockAllPrefab, m_WorkspaceUI.frontPanel, false);
				foreach (var mb in unlockAllUI.GetComponentsInChildren<MonoBehaviour>())
				{
					connectInterfaces(mb);
				}
				unlockAllUI.GetComponentInChildren<Button>(true).onClick.AddListener(UnlockAll);
			}
		}

		void UnlockAll()
		{
			UnlockAll(m_HierarchyData);
		}

		void UnlockAll(List<HierarchyData> hierarchyData)
		{
			if (hierarchyData == null)
				return;

			foreach (var hd in hierarchyData)
			{
				var go = (GameObject)EditorUtility.InstanceIDToObject(hd.index);
				setLocked(go, false);
				hd.locked = isLocked(go);

				UnlockAll(hd.children);
			}
		}
	}
}
#endif