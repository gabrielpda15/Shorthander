using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shorthander
{
    static class Program
    {
        /// <summary>
        /// Usado para ler e escrever imagens do disco
        /// </summary>
        static ImgManager ImgManager { get; } = new ImgManager();
        static List<string> Screen { get; set; } = new List<string>();

        /// <summary>
        /// O ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main(params string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ConsoleManager.DefaultColor = ConsoleManager.Colors["&F0"];
            Screen.AddRange(StaticData.HEADER);

            do
            {
                try
                {
                    ConsoleManager.Write(Screen);
                    var result = ConsoleManager.Prompt(" Digite ou arraste a url da imagem");
                    Console.WriteLine();

                    if (!File.Exists(result))
                    {
                        ConsoleManager.Error(" Arquivo não encontrado!");
                        goto Reset;
                    }


                    var img = ImgManager.ReadImage(result);

                    var attempt = SteganographyHelper.ExtractText(img);

                    if (attempt.StartsWith(StaticData.MAGIC))
                    {
                        using (var saveDialog = new SaveFileDialog())
                        {
                            var fileName = new string(attempt.Skip(StaticData.MAGIC.Length).TakeWhile(x => x != ')').Skip(1).ToArray());
                            saveDialog.FileName = fileName;
                            saveDialog.Filter = "Text Files|*.txt";
                            saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                            if (saveDialog.ShowDialog() == DialogResult.OK)
                            {
                                var toSave = new string(attempt.Skip(StaticData.MAGIC.Length + fileName.Length + 2).ToArray());
                                File.WriteAllText(saveDialog.FileName, toSave);
                                ConsoleManager.Info("Arquivo salvo com sucesso!");
                                goto Reset;
                            }
                        }


                    }


                    result = ConsoleManager.Prompt(" Digite ou arraste um arquivo de texto");
                    Console.WriteLine();

                    if (!File.Exists(result))
                    {
                        ConsoleManager.Error(" Arquivo não encontrado!");
                        goto Reset;
                    }

                    if (Path.GetExtension(result) != ".txt")
                    {
                        ConsoleManager.Error(" Formato de arquivo não suportado, tente usar um .txt!");
                        goto Reset;
                    }

                    var text = StaticData.MAGIC + $"({Path.GetFileName(result)})" + File.ReadAllText(result);

                    using (var saveDialog = new SaveFileDialog())
                    {
                        saveDialog.Filter = "Image Files|*.png; *.jpg; *.bmp";
                        saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                        if (saveDialog.ShowDialog() == DialogResult.OK)
                        {
                            var imgResult = SteganographyHelper.EmbedText(text, img);
                            ImgManager.WriteImage(imgResult, saveDialog.FileName);
                            ConsoleManager.Info("Arquivo salvo com sucesso!");
                            goto Reset;
                        }
                    }

                }
                catch (FileFormatException ex)
                {
                    ConsoleManager.Error(" " + ex.Message);
                    goto Reset;
                }

                Console.ReadKey();
            Reset:
                Console.Clear();
            } while (true);
        }

    }
}
