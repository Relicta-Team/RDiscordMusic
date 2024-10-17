using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReCaves
{
	class Program
	{
		static void Main(string[] args)
		{
			StringBuilder output = new StringBuilder();
			CommandProcessor.ParseCommand(output, 1000, "test", new string[] { });
			Console.WriteLine($"Output: {output}");
			Console.WriteLine("Press any key to exit...");
			Console.ReadKey();
		}
	}
}
