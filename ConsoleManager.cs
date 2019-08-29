using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shorthander
{
    public static class ConsoleColorExtension
    {
        public static void Apply(this ConsoleManager.ConsoleColor color)
        {
            if (color == null)
                Console.ResetColor();
            else
            {
                Console.BackgroundColor = color.BackgroundColor;
                Console.ForegroundColor = color.ForegroundColor;
            }
        }
    }

    public static class ConsoleManager
    {
        private const string HEX_CHARS = "0123456789ABCDEF";

        

        public class ConsoleColor
        {
            public System.ConsoleColor ForegroundColor { get; set; }
            public System.ConsoleColor BackgroundColor { get; set; }
        }

        private static ConsoleColor _defaultColor = null;
        public static ConsoleColor DefaultColor 
        {
            get => _defaultColor;
            set
            {
                _defaultColor = value;
                value.Apply();
                Console.Clear();
            }
        }

        static ConsoleManager()
        {
            Colors = new Dictionary<string, ConsoleColor>();

            for (int i = 0; i < HEX_CHARS.Length; i++)
            {
                for (int j = 0; j < HEX_CHARS.Length; j++)
                {
                    Colors.Add($"&{HEX_CHARS[i]}{HEX_CHARS[j]}", new ConsoleColor
                    {
                        BackgroundColor = (System.ConsoleColor)i,
                        ForegroundColor = (System.ConsoleColor)j
                    });
                }
            }
        }

        public static IDictionary<string, ConsoleColor> Colors { get; }

        public static string GetForegroundColor(System.ConsoleColor color)
        {
            var f = HEX_CHARS[(int)color];
            return DefaultColor == null ? "&0" + f : "&" + HEX_CHARS[(int)DefaultColor.BackgroundColor] + f;
        }

        public static void Error(string message)
        {
            var color = GetForegroundColor(System.ConsoleColor.DarkRed);
            Write(color + message);
            Console.ReadKey(true);
        }

        public static void Info(string message)
        {
            var color = GetForegroundColor(System.ConsoleColor.DarkGreen);
            Write(color + message);
            Console.ReadKey(true);
        }

        public static string Prompt(string message)
        {
            WriteLine(message);
            Write(StaticData.CURSOR);
            Colors[GetForegroundColor(System.ConsoleColor.DarkYellow)].Apply();
            var result = Console.ReadLine();
            DefaultColor.Apply();
            return result;
        }

        public static void WriteLine(IEnumerable<string> messages)
        {
            Write(messages);
            Console.WriteLine();
        }

        public static void WriteLine(string message)
        {
            Write(message);
            Console.WriteLine();
        }

        public static void Write(IEnumerable<string> messages)
        {
            foreach (var message in messages)
            {
                Write(message);
            }
        }

        public static void Write(string message)
        {
            var writeEnd = true;
            for (int i = 0; i < message.Length - 2; i++)
            {
                if (message[i] == '&')
                {
                    var valid = false;
                    var key = new string(message.ToArray(), i, 3);
                    if (Colors.ContainsKey(key))
                    {
                        Colors[key].Apply();
                        i += 2;
                        valid = true;
                    } 
                    else if (key == "&NL")
                    {
                        Console.WriteLine();
                        i += 2;
                        valid = true;
                    }

                    if (valid && i == message.Length - 1) writeEnd = false;
                }
                else
                {
                    Console.Write(message[i]);
                }
            }

            if (writeEnd) Console.Write(message[message.Length - 2].ToString() + message[message.Length - 1].ToString());
            DefaultColor.Apply();
        }
        
    }
}
