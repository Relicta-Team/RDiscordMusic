using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReCaves
{
	public static class CommandProcessor
	{
		/// <summary>
		/// Сюда можно записывать куски даных
		/// </summary>
		private static StringBuilder partialBuffer = new StringBuilder();

		internal static bool debugPrinter = false;

		internal static void debugPrint(string data)
		{
			if (debugPrinter)
			{
				Console.WriteLine("RECAVES_EXT_DEBUG:" + data);
			}
		}

		/// <summary>
		/// Commands handler from RVEngine. This function called in main game thread.
		/// !!! This function should not throw unhandled exceptions, otherwise the application will crash
		/// </summary>
		/// <param name="output">Data for game in string repr</param>
		/// <param name="outputSize"></param>
		/// <param name="function">Command name</param>
		/// <param name="args">Arguments of strings</param>
		public static void ParseCommand(StringBuilder output, int outputSize, string function, string[] args)
		{
			//fixed size in 2.16 -> 20480
			//utf8 ru-letter -> 2 bytes
			int semiOutputSize = (int)Math.Floor((double)(outputSize / 2));

			if (debugPrinter)
			{
				debugPrint($"Handle command (args {args.Length}): {function}");
				for (int i = 0; i < (args.Length); i++)
				{
					debugPrint($"Arg {i}: {args[i]}");
				}
			}

			if (function == "set_option")
			{
				//Здесь конфигурируем
				output.Append("ERR:Option not found");
			}
			else if (function == "generate")
			{
				if (args.Length < 2) return;
				try
				{
					//Здесь реализуем генерацию
					output.Append("ERR:Not implemented");
					
				}
				catch (Exception ex)
				{
					output.Append($"ERR:{ex.GetType().FullName}".EncodingToRV());
				}
			}
			#region Команды для частичного возврата
			else if (function == "has_parts")
			{
				output.Append(partialBuffer.Length > 0);
			}
			else if (function == "next_read_part")
			{
				if (args.Length != 1)
				{
					output.Append($"ERR:Command {function} wrong param count: {args.Length}");
					return;
				}

				output.Append(
					partialBuffer.ToString(0, Math.Min(semiOutputSize, partialBuffer.Length))
						.EncodingToRV()
				);

				partialBuffer.Remove(0, Math.Min(semiOutputSize, partialBuffer.Length));
				return;
			}
			else if (function == "free_parts")
			{
				partialBuffer.Clear();
				output.Append(partialBuffer.Length == 0);
			}
			else if (function == "get_left_parts_count")
			{
				if (partialBuffer.Length == 0)
				{
					output.Append(0);
					return;
				}
				output.Append(Math.Ceiling((double)partialBuffer.Length / semiOutputSize));
			}
			#endregion
			else if (function == "set_debug")
			{
				if (args.Length != 1)
				{
					output.Append($"ERR:{function} args size missmatch -> {args.Length}");
					return;
				}
				debugPrinter = args[0] == "true";
				output.Append(debugPrinter);
			}
			else
			{
				output.Append($"ERR:No command found \"{function}\"");
			}

		}

		
	}
}
