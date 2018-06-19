using System;
using System.Collections.Generic;
using Microsoft.Office.Interop.Visio;
using Microsoft.Office.Interop.VisOcx;

namespace Algoritmizator
{
    public class Designer
    {
        private static string templatePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + @"\BASFLO_M.VST";
        public string VisioTemplatePath
        {
            get { return templatePath; }
            set { templatePath = value; }
        }
        private static string fileName = Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + @"\MyDrawing.vsd";
        private static string stencilName = Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + @"\BASFLO_M.VSS";
        public string SaveFolder
        {
            get { return fileName; }
            set { fileName = value; }
        }
        public static void GenerateDiagram(BlockClass blockClass)
        {
            Application application = new Application();
            application.Visible = false;
            Document doc = application.Documents.Add(templatePath);
            Document stencil = application.Documents.OpenEx(stencilName,(short)VisOpenSaveArgs.visOpenHidden);
            Page page = application.Documents[1].Pages[1];
            for (int i = 0; i < blockClass.Methods.Count; i++)
            {
                if (i != 0)
                    page = application.Documents[1].Pages.Add();
                Shape shape = new Shape();
                List<Block> DrawListBlock = new List<Block>();
                DrawListBlock.Add(blockClass.Methods[i].Blocks[0]);
                Block activeBlock = DrawListBlock[0];
                shape = page.Drop(stencil.Masters["Оконечная фигура"], (105 + 12.5 * activeBlock.Left) / 25.4, (290 - 7.5 - 15 * activeBlock.Top) / 25.4);
                activeBlock.NextBlocks[0].PrevShape = shape;
                activeBlock.NextBlocks[0].PrevBlock = activeBlock;
                DrawListBlock.Add(activeBlock.NextBlocks[0]);
                shape.Text = activeBlock.Text;
                for (int j = 1; j < DrawListBlock.Count; j++)
                {
                    activeBlock = DrawListBlock[j];
                    switch (activeBlock.Type)
                    {
                        case BlockType.Процесс:
                            shape = page.Drop(stencil.Masters["Процесс"], (105 + 12.5 * activeBlock.Left) / 25.4, (290 - 7.5 - 15 * activeBlock.Top) / 25.4);
                            activeBlock.NextBlocks[0].PrevShape = shape;
                            activeBlock.NextBlocks[0].PrevBlock = activeBlock;
                            DrawListBlock.Add(activeBlock.NextBlocks[0]);
                            shape.Text = activeBlock.Text;
                            activeBlock.PrevShape.AutoConnect(shape, VisAutoConnectDir.visAutoConnectDirNone, stencil.Masters["Динамическая соединительная линия"]);
                            break;
                        case BlockType.Оконечная_фигура:
                            shape = page.Drop(stencil.Masters["Оконечная фигура"], (105 + 12.5 * activeBlock.Left) / 25.4, (290 - 7.5 - 15 * activeBlock.Top) / 25.4);
                            shape.Text = activeBlock.Text; break;
                        case BlockType.Заранее_определенный_процесс:
                            shape = page.Drop(stencil.Masters["Заранее определенный процесс"], (105 + 12.5 * activeBlock.Left) / 25.4, (290 - 7.5 - 15 * activeBlock.Top) / 25.4);
                            activeBlock.NextBlocks[0].PrevShape = shape;
                            activeBlock.NextBlocks[0].PrevBlock = activeBlock;
                            DrawListBlock.Add(activeBlock.NextBlocks[0]);
                            shape.Text = activeBlock.Text;
                            activeBlock.PrevShape.AutoConnect(shape, VisAutoConnectDir.visAutoConnectDirNone, stencil.Masters["Динамическая соединительная линия"]);
                            break;
                        case BlockType.Решение:
                            shape = page.Drop(stencil.Masters["Решение"], (105 + 12.5 * activeBlock.Left) / 25.4, (290 - 7.5 - 15 * activeBlock.Top) / 25.4);
                            activeBlock.NextBlocks[0].PrevShape = shape;
                            activeBlock.NextBlocks[1].PrevShape = shape;
                            activeBlock.NextBlocks[0].PrevBlock = activeBlock;
                            DrawListBlock.Add(activeBlock.NextBlocks[0]);
                            activeBlock.NextBlocks[1].PrevShape = shape;
                            DrawListBlock.Add(activeBlock.NextBlocks[1]);
                            shape.Text = activeBlock.Text;
                            activeBlock.PrevShape.AutoConnect(shape, VisAutoConnectDir.visAutoConnectDirNone, stencil.Masters["Динамическая соединительная линия"]);
                            break;
                        case BlockType.Подготовка:
                            shape = page.Drop(stencil.Masters["Подготовка"], (105 + 12.5 * activeBlock.Left) / 25.4, (290 - 7.5 - 15 * activeBlock.Top) / 25.4);
                            activeBlock.NextBlocks[0].PrevShape = shape;
                            activeBlock.NextBlocks[0].PrevBlock = activeBlock;
                            DrawListBlock.Add(activeBlock.NextBlocks[0]);
                            shape.Text = activeBlock.Text;
                            activeBlock.PrevShape.AutoConnect(shape, VisAutoConnectDir.visAutoConnectDirNone, stencil.Masters["Динамическая соединительная линия"]);
                            break;
                        case BlockType.Предел_цикла:
                            shape = page.Drop(stencil.Masters["Предел цикла"], (105 + 12.5 * activeBlock.Left) / 25.4, (290 - 7.5 - 15 * activeBlock.Top) / 25.4);
                            activeBlock.NextBlocks[0].PrevShape = shape;
                            activeBlock.NextBlocks[0].PrevBlock = activeBlock;
                            DrawListBlock.Add(activeBlock.NextBlocks[0]);
                            shape.Text = activeBlock.Text;
                            activeBlock.PrevShape.AutoConnect(shape, VisAutoConnectDir.visAutoConnectDirNone, stencil.Masters["Динамическая соединительная линия"]);
                            break;
                    }
                }
            }
            doc.SaveAs(fileName); doc.Close();
        }
        public static void Generate()
        {
            Application application = new Application();
            application.Visible = false;
            Document doc = application.Documents.Add(templatePath);
            Document stencil = application.Documents.OpenEx(stencilName, (short)VisOpenSaveArgs.visOpenHidden);
            Page page1 = application.Documents[1].Pages[1];
            Page page2 = application.Documents[1].Pages.Add();
            Shape shape1 = page1.Drop(stencil.Masters["Заранее определенный процесс"], 6, 5);
            Shape shape2 = page1.Drop(stencil.Masters["Решение"], 3, 9);
            Shape shape3 = page1.Drop(stencil.Masters["Заранее определенный процесс"], 6, 10);
            Shape shape = page1.Drop(stencil.Masters["Динамическая соединительная линия"], 1, 1);
            shape2.AutoConnect(shape1, VisAutoConnectDir.visAutoConnectDirNone, stencil.Masters["Динамическая соединительная линия"]);
            doc.SaveAs(fileName); doc.Close();
        }
    }
}