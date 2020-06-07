/*! \cond PRIVATE */
using System;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
	[Serializable]
	// ReSharper disable once CheckNamespace
	public class CGKCustomEventCategory {
		public string CatName = LevelSettings.NoCategory;
		public bool IsExpanded = true;
		public bool IsEditing = false;
		public bool IsTemporary = false;
		public string ProspectiveName = LevelSettings.NoCategory;
	}
}
/*! \endcond */