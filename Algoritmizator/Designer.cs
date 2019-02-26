using System;
using System.Collections.Generic;
using Microsoft.Office.Interop.Visio;
using Microsoft.Office.Interop.VisOcx;
namespace Algoritmizator
{
    public class Designer
    {
        private static string templatePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\BASFLO_M.VST";
        public string VisioTemplatePath
        {
            get { return templatePath; }
            set { templatePath = value; }
        }
        private static string fileName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\MyDrawing.vsd";
        private static string stencilName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\BASFLO_M.VSS";
        public string SaveFolder
        {
            get { return fileName; }
            set { fileName = value; }
        }
        public static void GenerateDiagram(BlockMethod blokMethod)
        {
            Application application = new Application(); application.Visible = false;
            Document doc = application.Documents.Add(templatePath);
            Document stencil = application.Documents.OpenEx(stencilName,(short)VisOpenSaveArgs.visOpenHidden);
            Page page = application.Documents[1].Pages[1]; double x = 290;
            for (int i = 0; i < blokMethod.Blocks.Count; i++)
            {
                if (x - 20 * blokMethod.Blocks[i].Top < 7.5)
                {
                    page = application.Documents[1].Pages.Add(); x = x + 290;
                }
                switch (blokMethod.Blocks[i].Type)
                {
                    case BlockType.Процесс:
                        Shape shape0 = page.Drop(stencil.Masters["Процесс"], (105 + 25 * blokMethod.Blocks[i].Left) / 25.4, (x - 20 * blokMethod.Blocks[i].Top) / 25.4);
                        shape0.Text = blokMethod.Blocks[i].Text; blokMethod.Blocks[i].shape = shape0; break;
                    case BlockType.Оконечная_фигура:
                        Shape shape1 = page.Drop(stencil.Masters["Оконечная фигура"], (105 + 25 * blokMethod.Blocks[i].Left) / 25.4, (x - 20 * blokMethod.Blocks[i].Top) / 25.4);
                        shape1.Text = blokMethod.Blocks[i].Text; blokMethod.Blocks[i].shape = shape1; break;
                    case BlockType.Заранее_определенный_процесс:
                        Shape shape2 = page.Drop(stencil.Masters["Заранее определенный процесс"], (105 + 25 * blokMethod.Blocks[i].Left) / 25.4, (x - 20 * blokMethod.Blocks[i].Top) / 25.4);
                        shape2.Text = blokMethod.Blocks[i].Text; blokMethod.Blocks[i].shape = shape2; break;
                    case BlockType.Решение:
                        Shape shape3 = page.Drop(stencil.Masters["Решение"], (105 + 25 * blokMethod.Blocks[i].Left) / 25.4, (x - 20 * blokMethod.Blocks[i].Top) / 25.4);
                        shape3.Text = blokMethod.Blocks[i].Text; blokMethod.Blocks[i].shape = shape3; break;
                    case BlockType.Подготовка:
                        Shape shape4 = page.Drop(stencil.Masters["Подготовка"], (105 + 25 * blokMethod.Blocks[i].Left) / 25.4, (x - 20 * blokMethod.Blocks[i].Top) / 25.4);
                        shape4.Text = blokMethod.Blocks[i].Text; blokMethod.Blocks[i].shape = shape4; break;
                    case BlockType.Предел_цикла:
                        Shape shape5 = page.Drop(stencil.Masters["Предел цикла"], (105 + 25 * blokMethod.Blocks[i].Left) / 25.4, (x - 20 * blokMethod.Blocks[i].Top) / 25.4);
                        shape5.Text = blokMethod.Blocks[i].Text; blokMethod.Blocks[i].shape = shape5; break;
                }
            }
            for (int i = 0; i < blokMethod.Blocks.Count - 1; i++)
            {
                Block activeBlock = blokMethod.Blocks[i];
                if (activeBlock.Type == BlockType.Решение)
                {
                    if (activeBlock.NextBlocks[0] != null)
                    {
                        activeBlock.shape.AutoConnect(activeBlock.NextBlocks[0].shape, VisAutoConnectDir.visAutoConnectDirLeft, stencil.Masters["Динамическая соединительная линия"]);
                    }
                    if (activeBlock.NextBlocks[1] != null)
                    {
                        activeBlock.shape.AutoConnect(activeBlock.NextBlocks[1].shape, VisAutoConnectDir.visAutoConnectDirRight, stencil.Masters["Динамическая соединительная линия"]);
                    }
                }
                else
                {
                    if (activeBlock.NextBlocks[0] != null)
                    {
                        activeBlock.shape.AutoConnect(activeBlock.NextBlocks[0].shape, VisAutoConnectDir.visAutoConnectDirNone, stencil.Masters["Динамическая соединительная линия"]);
                    }
                }
            }
            doc.SaveAs(fileName); doc.Close(); stencil.Close();
        }
        public static void Generate()
        {
            Application application = new Application();
            application.Visible = false;
            Document doc = application.Documents.Add(templatePath);
            Document stencil = application.Documents.OpenEx(stencilName, (short)VisOpenSaveArgs.visOpenHidden);
            Page page1 = application.Documents[1].Pages[1];
            Page page2 = application.Documents[1].Pages.Add();
            Shape shape = page1.Drop(stencil.Masters["Оконечная фигура"], (105) / 25.4, (290) / 25.4);
            Shape shape1 = page2.Drop(stencil.Masters["Решение"], 3, 9);
            shape = page1.Drop(stencil.Masters["Заранее определенный процесс"], 6, 10);
            shape.AutoConnect(shape1, VisAutoConnectDir.visAutoConnectDirNone, stencil.Masters["Динамическая соединительная линия"]);
            doc.SaveAs(fileName); doc.Close(); stencil.Close();
        }
    }
}