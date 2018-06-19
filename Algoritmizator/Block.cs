using System;
using System.Collections.Generic;
using Microsoft.Office.Interop.Visio;
using Microsoft.Office.Interop.VisOcx;

namespace Algoritmizator
{
    public enum BlockType
    {
        Процесс, Оконечная_фигура, Заранее_определенный_процесс, Решение, Подготовка, Предел_цикла
    }
    public class BlockClass
    {
        public string Name { get; set; }
        public List<BlockMethod> Methods { get; set; }
        public List<string> Fields { get; set; }
        public BlockClass(string name)
        {
            Name = name; Fields = new List<string>();
            Methods = new List<BlockMethod>();
        }
    }
    public class BlockMethod
    {
        public string Name { get; set; }
        public List<Block> Blocks { get; set; }
        public BlockMethod()
        {
            Blocks = new List<Block>(); Name = "defaultName";
        }
        public BlockMethod(string name)
        {
            Blocks = new List<Block>(); Name = name;
        }
    }
    public class Block
    {
        public double[] X { get; set; }
        public double[] Y { get; set; }
        public string Text { get; set; }
        public string Comment { get; private set; }
        public BlockType Type { get; set; }
        public List<Block> NextBlocks { get; private set; }
        public Shape PrevShape { get; set; }
        public Block PrevBlock { get; set; }
        public int Level { get; set; }
        public Path CodePath { get; set; }
        public int Top { get; set; } //расстояние от верха страницы в ячейках
        public int Left { get; set; } // расстояние от центра страницы в ячейках
        public Block()
        {
            X = new double[3]; Y = new double[3];
            Text = ""; Comment = "";
            Type = BlockType.Решение;
            NextBlocks = new List<Block>();
            CodePath = new Path();
            Level = 0; Top = 0; Left = 0;
        }
        public Block(string text)
        {
            X = new double[3]; Y = new double[3];
            Text = text; Comment = "";
            Type = BlockType.Решение;
            NextBlocks = new List<Block>();
            CodePath = new Path();
            Level = 0; Top = 0; Left = 0;
        }
        public Block(string text, BlockType type)
        {
            X = new double[3]; Y = new double[3];
            Text = text; Comment = ""; Type = type;
            NextBlocks = new List<Block>();
            Level = 0; Top = 0; Left = 0;
        }
        public void SetComment(string commentText)
        {
            Comment = commentText;
        }
    }
    public class Path
    {
        public List<PathPoint> CodePoints { get; set; }
        public Path()
        {
            CodePoints = new List<PathPoint>();
        }
        public Path DeepCopy()
        {
            Path result = new Path();
            foreach (PathPoint item in CodePoints)
            {
                result.CodePoints.Add(new PathPoint(item.Level, item.Parallel, item.IsIf));
            }
            return result;
        }
    }
    public class PathPoint
    {
        public int Level { get; set; }
        public int Parallel { get; set; }
        public bool IsIf { get; set; }
        public PathPoint()
        {
            Level = 0; Parallel = 0; IsIf = false;
        }
        public PathPoint(int level, int parallel, bool isIf)
        {
            Level = level; Parallel = parallel; IsIf = isIf;
        }
    }
}