using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Unigine;
using UnigineApp.source.GalaxyNetwork;

namespace UnigineApp
{
	class UnigineApp
	{
		[STAThread]
		static void Main(string[] args)
		{
			Engine.Init(args);
			
			AppSystemLogic systemLogic = new AppSystemLogic();
			AppWorldLogic worldLogic = new AppWorldLogic();
			AppEditorLogic editorLogic = new AppEditorLogic();
			UnigineGalaxyNetwork galaxyNetwork = new UnigineGalaxyNetwork();
			Engine.AddPlugin(galaxyNetwork);
			Engine.Main(systemLogic, worldLogic, editorLogic);
		 
			Engine.Shutdown();
		}
	}
}