#if FNA
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Terraria
{
	internal static class FNALaunch
	{
		private static void Main(string[] args) {
			/*Workaround for Steam setting ANGLE environment variables incorrectly*/
			if (args.Any(s => s.Contains("-vulkan")))
				Environment.SetEnvironmentVariable("ANGLE_DEFAULT_PLATFORM", "vulkan");
			else if (args.Any(s => s.Contains("-d3d11")))
				Environment.SetEnvironmentVariable("ANGLE_DEFAULT_PLATFORM", "d3d11");

			AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs sargs) {
				string resourceName = new AssemblyName(sargs.Name).Name + ".dll";
				string text = Array.Find<string>(typeof(Program).Assembly.GetManifestResourceNames(), (string element) => element.EndsWith(resourceName));
				if (text == null) {
					return null;
				}

				Assembly result;
				using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(text)) {
					byte[] array = new byte[manifestResourceStream.Length];
					manifestResourceStream.Read(array, 0, array.Length);
					result = Assembly.Load(array);
				}

				return result;
			};
			Environment.SetEnvironmentVariable("FNA_WORKAROUND_WINDOW_RESIZABLE", "1");
			// SDL2 automatically disables HIGHDPI support if it isn't needed, so we just set this in case
			Environment.SetEnvironmentVariable("FNA_GRAPHICS_ENABLE_HIGHDPI", "1");
			Program.LaunchGame(args, true);
		}
	}
}
#endif