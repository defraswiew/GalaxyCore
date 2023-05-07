using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Unigine;

namespace UnigineApp
{
	class AppWorldLogic : WorldLogic
	{
		// World logic, it takes effect only when the world is loaded.
		// These methods are called right after corresponding world script's (UnigineScript) methods.

		public AppWorldLogic()
		{
		}

		public override bool Init()
		{
			// Write here code to be called on world initialization: initialize resources for your world scene during the world start.

			return true;
		}

		// start of the main loop
		public override bool Update()
		{
			// Write here code to be called before updating each render frame: specify all graphics-related functions you want to be called every frame while your application executes.

			return true;
		}

		public override bool PostUpdate()
		{
			// The engine calls this function after updating each render frame: correct behavior after the state of the node has been updated.

			return true;
		}

		public override bool UpdatePhysics()
		{
			// Write here code to be called before updating each physics frame: control physics in your application and put non-rendering calculations.
			// The engine calls UpdatePhysics() with the fixed rate (60 times per second by default) regardless of the FPS value.
			// WARNING: do not create, delete or change transformations of nodes here, because rendering is already in progress.

			return true;
		}
		// end of the main loop

		public override bool Shutdown()
		{
			// Write here code to be called on world shutdown: delete resources that were created during world script execution to avoid memory leaks.

			return true;
		}

		public override bool Save(Stream stream)
		{
			// Write here code to be called when the world is saving its state (i.e. state_save is called): save custom user data to a file.

			return true;
		}

		public override bool Restore(Stream stream)
		{
			// Write here code to be called when the world is restoring its state (i.e. state_restore is called): restore custom user data to a file here.

			return true;
		}
	}
}
