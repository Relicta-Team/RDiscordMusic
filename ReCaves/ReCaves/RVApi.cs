﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ReCaves
{
#if !ISDLL
	// Класс-заглушка для тестирования приложения в режиме
	public class DllExport : Attribute
	{
		public CallingConvention CallingConvention;
		public DllExport(string v, CallingConvention c) { }
		public DllExport(string v) { }
	};
#endif

	/// <summary>
	/// Класс нижнего слоя взаимодействия с движком Армы.
	/// </summary>
	public static class RVApi
	{
		static Version version = new Version(1, 0);

		[DllExport("RVExtensionVersion", CallingConvention = CallingConvention.Winapi)]
		public static void RvExtensionVersion(StringBuilder output, int outputSize) { output.Append(version.ToString()); }

		[DllExport("RVExtension", CallingConvention = CallingConvention.Winapi)]
		public static void RvExtension(StringBuilder output, int outputSize, [MarshalAs(UnmanagedType.LPStr)] string function)
		{
#if ISDLL
			if (function == "get_version")
			{
				output.Append(version.ToString());
				return;
			}
			output.Append("ReCaves");
#else
			output.Append("Relicta cave generator. Version: " + version.ToString());
#endif
		}

		/// <summary>
		/// Функция вызывается из движка Армы. Выполняется в основном потоке. Сликом долгие действия залочат процесс до конца выполнения этой функции.
		/// </summary>
		/// <param name="output"></param>
		/// <param name="outputSize"></param>
		/// <param name="function"></param>
		/// <param name="args"></param>
		[DllExport("RVExtensionArgs", CallingConvention = CallingConvention.Winapi)]
		public static int RvExtensionArgs(StringBuilder output, int outputSize,
		   [MarshalAs(UnmanagedType.LPStr)] string function,
		   [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 4)] string[] args, int argCount)
		{
			CommandProcessor.ParseCommand(output, outputSize, function, args);
			if (CommandProcessor.debugPrinter)
			{
				CommandProcessor.debugPrint($"ONCALL({function});OUTPUT({outputSize})<{output}>END::");
			}
			return 0;
		}

		public static ExtensionCallback callback;
		public delegate int ExtensionCallback([MarshalAs(UnmanagedType.LPStr)] string name, [MarshalAs(UnmanagedType.LPStr)] string function, [MarshalAs(UnmanagedType.LPStr)] string data);

		[DllExport("RVExtensionRegisterCallback", CallingConvention = CallingConvention.Winapi)]
		public static void RVExtensionRegisterCallback([MarshalAs(UnmanagedType.FunctionPtr)] ExtensionCallback func)
		{
			callback = func;
		}

	}


	public static class Extensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#if ISDLL
		public static string EncodingToRV(this string str) => Encoding.GetEncoding(1251).GetString(Encoding.GetEncoding(65001).GetBytes(str));
#else
		public static string EncodingToRV(this string str) => str;
#endif
		/// <summary>
		/// Преобразует строку в родную кодировку C# 
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#if ISDLL
		public static string EncodingToEngine(this string str) => Encoding.GetEncoding(65001).GetString(Encoding.GetEncoding(1251).GetBytes(str));
#else
		public static string EncodingToEngine(this string str) => str;
#endif

		/// <summary>
		/// Преобразует строковое значение, пришедшее из RealVirtuality Engine в обычную строку
		/// <br>Убирает двойные кавычки из строки</br>
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string FormatToEngine(this string str) => str.Replace("\"", "");

		/// <summary>
		/// Преобразует строковое значение из C# в строку, заключенную в кавычки
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string FormatToRV(this string str) => $@"""{str}""";


		/// <summary>
		/// Преобразовывает число в армовскую строку с разделителем в виде точки
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string FormatToRV(this float str) => str.ToString(CultureInfo.InvariantCulture.NumberFormat);
		/// <summary>
		/// Преобразовывает указатель в армовскую строку 
		/// </summary>
		/// <param name="str"></param>
		/// <returns>Указатель UInt32 обёрнутый в кавычки</returns>
		public static string FormatToRV(this uint str) => $@"""{str}""";
	}
}
