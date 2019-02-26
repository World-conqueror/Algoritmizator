using System;
using System.Collections.Generic;
using System.Linq;
namespace Algoritmizator
{
    public class Bloxator
    {
        private string _textCode;
        public string TextCode
        {
            get { return _textCode; }
            set
            {
                if (_textCode != value)
                    ShapeCreated = false;
                _textCode = value;
            }
        }
        private List<Block> _complietBlocks;
        private List<Block> _notEndetBlocks;
        private BlockMethod _complietMethod;
        private BlockClass _complietClass;
        public bool ShapeCreated { get; protected set; }
        public Bloxator()
        {
            _textCode = null;
            _complietBlocks = new List<Block>();
            _notEndetBlocks = new List<Block>();
            _complietMethod = new BlockMethod();
            _complietClass = new BlockClass("defaultName");
            ShapeCreated = false;
        }
        private Command[] CommandSplit(string code, bool setName, bool addBeginEnd) //на 1 метод
        {
            List<Command> result = new List<Command>(); int i = 0;
            while (code[i] != '{')
                i++;
            if (setName)
            {
                string methodName = code.Substring(0, i);
                methodName = methodName.Trim();
                _complietMethod.Name = methodName;
            }
            i++;
            if (addBeginEnd)
            {
                Command begin = new Command("Начало", 0);
                begin.CodePath.CodePoints.Add(new PathPoint(1, 1, false));
                result.Add(begin);
            }
            int level = 1;
            while (i < code.Length)
            {
                string commandText = "";
                bool inStringState = false;
                char exitStringSymbol = ';';
                bool equalState = false;
                char exitEqualSymbol = ';';
                int parenthesisLevel = 0;
                bool inCommand = true;
                char[] ccc = new char[] { ';', '{' };
                while ((i < code.Length) && (inCommand))
                {
                    char c = code[i];
                    commandText += c;
                    if (parenthesisLevel == 0)
                    {
                        if (inStringState == false)
                        {
                            switch (c)
                            {
                                case '\'':
                                    inStringState = true;
                                    exitStringSymbol = '\''; break;
                                case '"':
                                    inStringState = true;
                                    exitStringSymbol = '"'; break;
                                default: break;
                            }
                        }
                        else
                            if (c == exitStringSymbol)
                            inStringState = false;
                    }
                    if (inStringState == false)
                    {
                        if (c == '(')
                            parenthesisLevel++;
                        if (c == ')')
                            parenthesisLevel--;
                    }
                    if ((inStringState == false) && (parenthesisLevel == 0) && (equalState == true))
                        if (c == exitEqualSymbol)
                        {
                            equalState = false; inCommand = false;
                        }
                    if ((inStringState == false) && (parenthesisLevel == 0) && (equalState == false))
                        if (c == '=')
                            equalState = true;
                    if ((inStringState == false) && (parenthesisLevel == 0) && (equalState == false))
                    {
                        if (c == '{')
                            level++;
                        if (c == '}')
                            level--;
                    }
                    if ((inStringState == false) && (parenthesisLevel == 0) && (equalState == false))
                        switch (c)
                        {
                            case ';':
                                inCommand = false; break;
                            case '{':
                                inCommand = false; break;
                            default: break;
                        }
                    i++;
                }
                Command newCommand = new Command(commandText, level);
                newCommand.Trim();
                if (newCommand.Text.Length > 6)
                {
                    if (newCommand.Text.Substring(0, 6) == "switch")
                    {
                        newCommand.Text = newCommand.Text + " { ";
                        int beginPos = i;
                        int currentLevel = 1;
                        while (currentLevel > 0)
                        {
                            if (_textCode[i] == '{')
                                currentLevel++;
                            if (_textCode[i] == '}')
                                currentLevel--;
                            i++;
                        }
                        newCommand.Text = newCommand.Text + _textCode.Substring(beginPos, i - beginPos);
                    }
                }
                if ((newCommand.Text != "{") && (newCommand.Text != "}"))
                {
                    result.Add(newCommand);
                }
            }
            if (addBeginEnd)
            {
                Command end = new Command("Конец", 0);
                end.CodePath.CodePoints.Add(new PathPoint(1, 1, false));
                result.Add(end);
            }
            return result.ToArray();
        }
        private void CommandsSwitchesToIfes(ref Command[] commands)
        {
            List<Command> commandList = commands.ToList();
            for (int i = 0; i < commandList.Count; i++)
            {
                Command item = commandList[i];
                if (item.Text.Length > 6)
                    if (item.Text.Substring(0, 6) == "switch")// преобразование switch в комплекс из if
                    {
                        int j = 5;
                        while (item.Text[j] != '(')
                            j++;
                        int beginVarPos = j;
                        while (item.Text[j] != ')')
                            j++;
                        j--;
                        string variableName = item.Text.Substring(beginVarPos + 1, j - beginVarPos);
                        int currentLevel = item.Level;
                        while (item.Text[j] != '{')
                            j++;
                        j++;
                        bool searchCase = true; bool switchEnd = false;
                        List<string> literators = new List<string>();
                        List<List<Command>> caseCodeBlocks = new List<List<Command>>();
                        caseCodeBlocks.Add(new List<Command>()
                        { new Command("bool _go_to_default_", currentLevel) });
                        while (!switchEnd)
                        {
                            if (searchCase)
                            {
                                while (item.Text.Substring(j, 4) != "case")
                                    j++;
                                int beginCasePos = j; j += 4;
                                while (item.Text[j] != ':')
                                    j++;
                                string literator = item.Text.Substring(beginCasePos + 4, j - beginCasePos);
                                literator = literator.Trim();
                                literators.Add(literator);
                                int k = j;
                                while (item.Text[k] != ';')
                                    k++;
                                int l = j;
                                while ((item.Text.Substring(l, 4) != "case") && (l < k))
                                    l++;
                                if (l == k)
                                    searchCase = false;
                            }
                            if (!searchCase)
                            {
                                j++; int beginCasePos = j;
                                while (item.Text.Substring(j, 5) != "break")
                                    j++;
                                string caseCode = item.Text.Substring(beginCasePos, j - beginCasePos);
                                j += 5; caseCode = " { " + caseCode + " } ";
                                List<Command> caseCommands = CommandSplit(caseCode, false, false).ToList();
                                foreach (Command com in caseCommands)
                                    com.Level += currentLevel + 1;
                                string ifText = "if (";
                                ifText += "(" + variableName + " == " + literators[0] + ")";
                                for (int k = 1; k < literators.Count; k++)
                                    ifText += " || (" + variableName + " == " + literators[k] + ")";
                                ifText += ")";
                                Command ifCommand = new Command(ifText, currentLevel);
                                ifCommand.CommandType = BlockType.Решение;
                                caseCommands.Insert(0, ifCommand);
                                caseCommands.Add(new Command("_go_to_default_ = false;", currentLevel + 1));
                                caseCodeBlocks.Add(caseCommands);
                            }
                        }
                    }
            }
            commands = commandList.ToArray();
        }
        private void CommandsParallelsCreate(ref Command[] commands)
        {
            int highLevel = 0;
            foreach (Command item in commands)
                if (item.Level >= highLevel)
                    highLevel = item.Level;
            Path codePath = new Path();
            for (int i = 0; i < highLevel + 1; i++)
            {
                PathPoint pp = new PathPoint(i, 0, false);
                codePath.CodePoints.Add(pp);
            }
            //int[] parallels = new int[highLevel + 1];
            for (int k = 0; k < commands.Length; k++)
            {
                Command item = commands[k];
                item.CodePath.CodePoints.Clear();
                for (int i = 0; i < item.Level + 1; i++)
                {
                    PathPoint pp = codePath.CodePoints[i];
                    item.CodePath.CodePoints.Add(new PathPoint(pp.Level, pp.Parallel, pp.IsIf));
                    //item.CodePath.CodePoints.Add(new PathPoint(i, parallels[i], false));
                }
                if (item.CommandType == BlockType.Решение)
                {
                    codePath.CodePoints[item.Level].IsIf = true;
                    //codePath.CodePoints[item.Level + 1].Parallel++;
                    if (item.Text == "else")
                        codePath.CodePoints[item.Level + 1].Parallel = 2;
                    else
                        codePath.CodePoints[item.Level + 1].Parallel = 1;
                    //parallels[item.Level + 1]++;
                }
                if (item.CommandType == BlockType.Подготовка)
                    codePath.CodePoints[item.Level].IsIf = false;
            }
        }
        private void CommandsCiclesEndsCreate(ref Command[] commands)
        {
            List<Command> commandList = commands.ToList();
            for (int i = 0; i < commandList.Count; i++)
                if (commandList[i].CommandType == BlockType.Подготовка)
                {
                    int j = i + 1;
                    while ((j < commandList.Count - 1) && (commandList[j].Level > commandList[i].Level))
                        j++;
                    if (j < commandList.Count - 1)
                    {
                        Command cicleEnd = new Command("", commandList[i].Level, commandList[i].CodePath.DeepCopy());
                        cicleEnd.CommandType = BlockType.Предел_цикла; commandList.Insert(j, cicleEnd);
                    }
                }
            commands = commandList.ToArray();
        }
        private void CommandsToBlocks(Command[] commands)
        {
            _notEndetBlocks.Clear();
            for (int i = 0; i < commands.Length; i++)
            {
                Command item = commands[i];
                Block newBlockComponent = new Block(item.Text, item.CommandType);
                newBlockComponent.Level = item.Level;
                newBlockComponent.CodePath = item.CodePath.DeepCopy();
                _notEndetBlocks.Add(newBlockComponent);
            }
            Block prevNEBlock = null;
            List<Block> lastBlocks = new List<Block>();
            for (int i = 0; i < _notEndetBlocks.Count; i++)
            {
                Block curentNEBlock = _notEndetBlocks[i];
                if (prevNEBlock != null)
                {
                    prevNEBlock.NextBlocks.Add(curentNEBlock);
                    lastBlocks.Remove(prevNEBlock);
                    prevNEBlock = null;
                }
                List<Block> listToRemove = new List<Block>();
                if (curentNEBlock.Text != "else")
                {
                    foreach (Block item in lastBlocks)
                    {
                        int parallelLoop = 0;
                        int minLevel = Math.Min(item.Level, curentNEBlock.Level) + 1;
                        while ((parallelLoop < minLevel) &&
                            (item.CodePath.CodePoints[parallelLoop].Parallel == curentNEBlock.CodePath.CodePoints[parallelLoop].Parallel))
                            parallelLoop++;
                        if (parallelLoop == minLevel)
                        {
                            item.NextBlocks.Add(curentNEBlock);
                            listToRemove.Add(item);
                        }
                    }
                }
                foreach (Block item in listToRemove)
                    lastBlocks.Remove(item);
                if (curentNEBlock.Type == BlockType.Подготовка)
                    prevNEBlock = curentNEBlock;
                if (curentNEBlock.Type == BlockType.Оконечная_фигура)
                    prevNEBlock = curentNEBlock;
                if (curentNEBlock.Type == BlockType.Решение)
                    prevNEBlock = curentNEBlock;
                lastBlocks.Add(curentNEBlock);
            }
            //обработка break и continue
            for (int i = 0; i < _notEndetBlocks.Count; i++)
            {
                Block item = _notEndetBlocks[i];
                if (item.NextBlocks != null)
                    for (int j = 0; j < item.NextBlocks.Count; j++)
                    {
                        if (item.NextBlocks[j].Text == "break")
                            for (int k = i; k < _notEndetBlocks.Count; k++)
                                if (_notEndetBlocks[k].Type == BlockType.Предел_цикла)
                                    item.NextBlocks[j] = _notEndetBlocks[k].NextBlocks[0];
                        if (item.NextBlocks[j].Text == "continue")
                            for (int k = i; k < _notEndetBlocks.Count; k++)
                                if (_notEndetBlocks[k].Type == BlockType.Предел_цикла)
                                    item.NextBlocks[j] = _notEndetBlocks[k];
                    }
            }
            //удаление break и continue
            for (int i = 0; i < _notEndetBlocks.Count; i++)
                if ((_notEndetBlocks[i].Text == "break") || (_notEndetBlocks[i].Text == "continue"))
                {
                    _notEndetBlocks.RemoveAt(i); i--;
                }
            //соединение if и else
            for (int i = 1; (i < _notEndetBlocks.Count); i++)
                if (_notEndetBlocks[i].Type == BlockType.Решение)
                    if (_notEndetBlocks[i].Text == "else")
                        for (int j = i - 1; j >= 0; j--)
                            if ((_notEndetBlocks[j].Level == _notEndetBlocks[i].Level)
                                && (_notEndetBlocks[j].Type == BlockType.Решение)) // найден соответствующий if
                            {
                                _notEndetBlocks[j].NextBlocks.Add(_notEndetBlocks[i].NextBlocks[0]); break;
                            }
            //удаление else
            for (int i = 0; i < _notEndetBlocks.Count; i++)
                if (_notEndetBlocks[i].Text == "else")
                {
                    _notEndetBlocks.RemoveAt(i); i--;
                }
            //выходы для if без else
            for (int i = 0; i < _notEndetBlocks.Count; i++)
            {
                Block item = _notEndetBlocks[i];
                if (item.Type == BlockType.Решение)
                    if (item.NextBlocks.Count == 1) // нашли if без else
                        for (int j = i + 1; j < _notEndetBlocks.Count; j++)
                            if (_notEndetBlocks[j].Level <= item.Level) // нашли выход для if
                            {
                                item.NextBlocks.Add(_notEndetBlocks[j]); break;
                            }
            }
            _complietBlocks = new List<Block>();
            foreach (Block item in _notEndetBlocks)
                _complietBlocks.Add(item);
        }
        private void RecurseSetTop(Block beginBlock)
        {
            if (beginBlock.NextBlocks.Count == 0)
                return;
            foreach (Block item in beginBlock.NextBlocks)
            {
                if (item.Top <= beginBlock.Top)
                    item.Top = beginBlock.Top + 1;
                RecurseSetTop(item);
            }
        }
        private void CorrectBlocksTop()
        {
            int maxTop = 0;
            foreach (Block item in _complietBlocks)
                if (item.Top > maxTop)
                    maxTop = item.Top;
            int botBorder = 0;
            while (botBorder < maxTop)
            {
                bool hasBlocksTop = false;
                foreach (Block item in _complietBlocks)
                    if (item.Top == botBorder)
                    {
                        hasBlocksTop = true; break;
                    }
                if (hasBlocksTop)
                    botBorder++;
                else
                {
                    int topBorder = maxTop;
                    foreach (Block item in _complietBlocks)
                        if ((item.Top > botBorder) && (item.Top < topBorder))
                            topBorder = item.Top;
                    int dev = topBorder - botBorder + 1;
                    foreach (Block item in _complietBlocks)
                        if (item.Top > botBorder)
                            item.Top -= dev;
                    maxTop -= dev;
                }
            }
        }
        private void BlocksGraphicPrepare()
        {
            List<Block> ifs = new List<Block>();
            int maxWidth = 0;
            foreach (Block item in _complietBlocks)
            {
                if (item.Type == BlockType.Решение)
                    ifs.Add(item);
                int width = 0;
                foreach (PathPoint item2 in item.CodePath.CodePoints)
                {
                    if (item2.IsIf)
                        width++;
                }
                if (width > maxWidth)
                    maxWidth = width;
            }
            foreach (Block item in _complietBlocks)
            {
                int left = 0;
                for (int i = 0; i < item.CodePath.CodePoints.Count; i++)
                {
                    PathPoint pp = item.CodePath.CodePoints[i];
                    if (i > 0)
                        if (item.CodePath.CodePoints[i - 1].IsIf)
                        {
                            if (pp.Parallel == 1)
                                left -= Convert.ToInt32(Math.Pow(2, maxWidth - pp.Level + 1));
                            if (pp.Parallel == 2)
                                left += Convert.ToInt32(Math.Pow(2, maxWidth - pp.Level + 1));
                        }
                }
                item.Left = left;
            }
            RecurseSetTop(_complietBlocks[0]);
            CorrectBlocksTop();
        }
        public void CreateShape(string textMethod)
        {
            Command[] commands = CommandSplit(textMethod, true, true);
            CommandsSwitchesToIfes(ref commands);
            CommandsParallelsCreate(ref commands);
            CommandsCiclesEndsCreate(ref commands);
            CommandsToBlocks(commands);
            BlocksGraphicPrepare();
            _complietMethod.Blocks = _complietBlocks;
            _complietBlocks = new List<Block>();
            ShapeCreated = true;
        }
        public BlockMethod GetComplietShape()
        {
            if (ShapeCreated)
                return _complietMethod;
            return null;
        }
    }
    public class Command
    {
        public int Level { get; set; }
        public Path CodePath { get; set; }
        public BlockType CommandType { get; set; }
        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                CommandType = BlockType.Процесс;
                SetCommandType();
            }
        }
        public Command(string text, int level)
        {
            Text = text; Level = level;
            CodePath = new Path();
            CommandType = BlockType.Процесс;
            SetCommandType();
        }
        public Command(string text, int level, Path path)
        {
            Text = text; Level = level;
            CodePath = path;
            CommandType = BlockType.Процесс;
            SetCommandType();
        }
        private void SetCommandType()
        {
            bool isFit = false;
            for (int i = 0; i < CommandStringsType.CommandStringsTypes.Length; i++)
            {
                for (int j = 0; j < CommandStringsType.CommandStringsTypes[i].KeyWords.Length; j++)
                {
                    string keyWord = CommandStringsType.CommandStringsTypes[i].KeyWords[j];
                    for (int k = 0; k < Text.Length; k++)
                    {
                        int l = 0;
                        while (Text[k] == keyWord[l])
                        {
                            l++; k++;
                            if (l == keyWord.Length)
                                break;
                            if (k == Text.Length)
                                break;
                        }
                        if (l == keyWord.Length)
                        {
                            isFit = true;
                            CommandType = CommandStringsType.CommandStringsTypes[i].CommandType;
                            break;
                        }
                    }
                    if (isFit)
                        break;
                }
                if (isFit)
                    break;
            }
        }
        public void Trim()
        {
            int i = 0; bool inText = false;
            char[] notTextSymbols = new char[] { ';', ' ', '\n', '\r', '{', '}' };
            while ((inText == false) && (i < Text.Length))
            {
                char c = Text[i];
                inText = true;
                foreach (char item in notTextSymbols)
                    if (c == item)
                        inText = false;
                i++;
            }
            Text = Text.Substring(i - 1);
            inText = false; i = Text.Length;
            while ((inText == false) && (i > 0))
            {
                i--; char c = Text[i];
                if (c == '{')
                    Level--;
                inText = true;
                foreach (char item in notTextSymbols)
                    if (c == item)
                        inText = false;
            }
            Text = Text.Substring(0, i + 1);
        }
    }
    class CommandStringsType
    {
        public string[] KeyWords { get; set; }
        public BlockType CommandType { get; set; }
        public CommandStringsType(string[] keyWords, BlockType commandType)
        {
            KeyWords = keyWords; CommandType = commandType;
        }
        public static CommandStringsType[] CommandStringsTypes { get; }
        static CommandStringsType()
        {
            CommandStringsTypes = new CommandStringsType[3];
            CommandStringsTypes[0] = new CommandStringsType(new string[] { "for", "while", "foreach" }, BlockType.Подготовка);
            CommandStringsTypes[1] = new CommandStringsType(new string[] { "Начало", "Конец" }, BlockType.Оконечная_фигура);
            CommandStringsTypes[2] = new CommandStringsType(new string[] { "if", "else" }, BlockType.Решение);
        }
    }
}