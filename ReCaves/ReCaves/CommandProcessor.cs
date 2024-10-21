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

                #region Свитч опций
                if (args.Length > 0)
                {
                    switch (args[0])
                    {
                        case "startPos":    // Стартовая позиция для зоны генерации
                            if (float.TryParse(args[1], out float startX) && float.TryParse(args[2], out float startY))
                            {
                                GeneratorConfig.Instance.generationZone.startPos.X = startX;
                                GeneratorConfig.Instance.generationZone.startPos.Y = startY;
                            }
                            break;
                        case "endPos":      // Конечная позиция для зоны генерации
                            if (float.TryParse(args[1], out float endX) && float.TryParse(args[2], out float endY))
                            {
                                GeneratorConfig.Instance.generationZone.endPos.X = endX;
                                GeneratorConfig.Instance.generationZone.endPos.Y = endY;
                            }
                            break;
                        case "level":       // Сколько этажей будут генерироваться
                            if (uint.TryParse(args[1], out uint level))
                            {
                                GeneratorConfig.Instance.Levels = level;
                            }
                            break;

                        case "zones":       // Добавление "Пустот" в список
                            bool validArgsCount = (args.Length - 1) % 4 == 0;
                            if (validArgsCount)
                            {
                                for (int val = 1; val < args.Length - 3; val = val + 4)
                                {
                                    if (float.TryParse(args[val], out float zoneX) && float.TryParse(args[val + 1], out float zoneY) && float.TryParse(args[val + 2], out float zoneZ) && float.TryParse(args[val + 3], out float zoneRad))
                                    {
                                        GeneratorConfig.Instance.zonesOfInterest.Add(new GeneratorConfig.Zone
                                        {
                                            position = new Point3D { X = zoneX, Y = zoneY, Z = zoneZ },
                                            radius = zoneRad
                                        });
                                    }
                                    else
                                    {
                                        output.Append($"ERR: Wrond argument from index {val}, zones wasn't added");
                                    }
                                }
                            }
                            else
                            {
                                output.Append("ERR:Wrong size of array, zone wasn't added");
                            }
                            break;
                        case "hardtrans":   // Добавление "Жёстких переходов" в список
                            bool validArgsCountHT = (args.Length - 1) % 3 == 0;
                            if (validArgsCountHT)
                            {
                                for (int val = 1; val < args.Length - 2; val = val + 3)
                                {
                                    if (float.TryParse(args[val], out float zoneX) && float.TryParse(args[val + 1], out float zoneY) && float.TryParse(args[val + 2], out float zoneZ))
                                    {
                                        GeneratorConfig.Instance.hardTransitions.Add(new Point3D
                                        {
                                            X = zoneX,
                                            Y = zoneY,
                                            Z = zoneZ
                                        });
                                    }
                                    else
                                    {
                                        output.Append($"ERR: Wrond argument from index {val}, transitions wasn't added");
                                    }
                                }
                            }
                            else
                            {
                                output.Append("ERR:Wrong size of array, transitions wasn't added");
                            }
                            break;
                        case "exits":   // Добавление Выходов в список
                            bool validArgsCountEx = (args.Length - 1) % 3 == 0;
                            if (validArgsCountEx)
                            {
                                for (int val = 1; val < args.Length - 2; val = val + 3)
                                {
                                    if (float.TryParse(args[val], out float zoneX) && float.TryParse(args[val + 1], out float zoneY) && float.TryParse(args[val + 2], out float zoneZ))
                                    {
                                        GeneratorConfig.Instance.exits.Add(new Point3D
                                        {
                                            X = zoneX,
                                            Y = zoneY,
                                            Z = zoneZ
                                        });
                                    }
                                    else
                                    {
                                        output.Append($"ERR: Wrond argument from index {val}, exits wasn't added");
                                    }
                                }
                            }
                            else
                            {
                                output.Append("ERR:Wrong size of array, exits wasn't added");
                            }
                            break;
                        case "square":
                            if (uint.TryParse(args[1], out uint sqrSize))
                            {
                                GeneratorConfig.Instance.SquareSize = sqrSize;
                            }
                            else
                            {
                                output.Append("ERR:Wrong argument, square size wasn't set");
                            }
                            break;
                        default:
                            output.Append("ERR:Option not found");
                            break;
                    }
                }
                #endregion
            }
            else if (function == "generate")
            {
                if (args.Length < 2) return;
                try
                {
                    //Здесь реализуем генерацию                
                    if ((GeneratorConfig.Instance.generationZone.startPos.X == 0) && (GeneratorConfig.Instance.generationZone.startPos.Y == 0) && (GeneratorConfig.Instance.generationZone.endPos.X == 0) && (GeneratorConfig.Instance.generationZone.endPos.Y == 0))
                    {
                        output.Append("ERR:No data in startPos and endPos");
                    }
                    Generator generator = new Generator(0.01, 0.35);
                    generator.GenerateCave();
                    generator.PrintMap();

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
