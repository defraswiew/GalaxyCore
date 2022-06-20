using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Unigine;

namespace UnigineApp
{
	
	class AppEditorLogic : EditorLogic
	{
		// Editor logic, it takes effect only when the UnigineEditor is loaded.
		// These methods are called right after corresponding editor script's (UnigineScript) methods.

		public AppEditorLogic()
		{
		}

		public override bool Init()
		{
			// Write here code to be called on editor initialization.

			return true;
		}

		// start of the main loop
		public override bool Update()
		{
			// Write here code to be called before updating each render frame when editor is loaded.

			return true;
		}

		public override bool Render()
		{
			// Write here code to be called before rendering each render frame when editor is loaded.

			return true;
		}
		// end of the main loop

		public override bool Shutdown()
		{
			// Write here code to be called on editor shutdown.

			return true;
		}

		public override bool WorldInit()
		{
			// Write here code to be called on world initialization when editor is loaded.
	
			return true;
		}

		public override bool WorldShutdown()
		{
			// Write here code to be called on world shutdown when editor is loaded.
	
			return true;
		}

		public override bool WorldSave()
		{
			// Write here code to be called on world save when editor is loaded.
	
			return true;
		}
	}
}
