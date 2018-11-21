// -----------------------
//    能动少C71尤佳睿
//    xjtu-blacksmith
// E-mail: yjr134@163.com
// -----------------------

// beautiWorld Compiler 2.0
// Work started: Sep. 22th, 2018
// 1.0 finished: Sep. 26th, 2018
// 2.0 started : Oct.  8th, 2018
// 2.0 finished: Oct. 10th, 2018

#region using section
using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Xml;
using System.ComponentModel;
#endregion

namespace Compiler
{
    public enum LANGUAGE { C, Cpp, Cs, Python, VB, Pascal, Unknown } ;  // 表示编程语言的枚举类型
    public enum COMPRES { AC, WA, CE };                                 // 表示评测结果的枚举类型

    /// <summary>
    /// 基本信息类
    /// </summary>
    public class Info
    {
        public string name;         // 姓名
        public string course;       // 课程名
        public int round;           // 课程轮次
        public int week;            // 周次
        public int index;           // 题目序号
        public LANGUAGE language;   // 编程语言
        public Info()
        {
            name = "admin";
            course = "default";
            round = 1;
            week = 1;
            index = 1;
            language = LANGUAGE.C;
        }
        public Info(string initName, string initCourse, int initRound, int initWeek, int initIndex, LANGUAGE initLanguage)
        {
            name = initName;
            course = initCourse;
            round = initRound;
            week = initWeek;
            index = initIndex;
            language = initLanguage;
        }
    }

    /// <summary>
    /// 编辑处理XML的静态类，包含了读取样例输入输出、基本信息和输出结果的函数
    /// </summary>
    static class XmlHelper
    {
        private static string Bool2String(bool flag)
        {
            if (flag == true)
                return "正确";
            else
                return "错误";
        }

        /// <summary>
        /// 读取节点（样例）信息
        /// </summary>
        /// <param name="xmlAlt">待读取的xml文件路径</param>
        /// <param name="key">标签名称，只能选input（样例输入）或output（样例输出）</param>
        /// <param name="stringList">用于储存样例数据的字符串数组</param>
        /// <returns>一个布尔值，表示是否读取成功</returns>
        public static bool ReadNodes(string xmlAlt, string key, out string[] stringList)
        {

            XmlDocument xml = new XmlDocument();
            xml.Load(xmlAlt);
            XmlNode node = xml.SelectSingleNode("/root/" + key);
            if (node.HasChildNodes == true)
            {
                XmlNodeList nodeList = node.ChildNodes;
                stringList = new string[nodeList.Count];
                for (int i = 0; i < nodeList.Count; i++)
                {
                    stringList[i] = nodeList[i].InnerText;
                }

                return true;
            }
            else
            {
                stringList = null;
                return false;
            }
        }

        /// <summary>
        /// 读取一个Info类型的基本信息对象
        /// </summary>
        /// <param name="xmlAlt">待读取的xml文件路径</param>
        /// <param name="xmlInfo">一个Info类型的基本信息对象，用于接收</param>
        /// <returns>一个布尔值，表示是否读取成功</returns>
        public static bool ReadInfo(string xmlAlt, Info xmlInfo)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlAlt);
            XmlNode infoNode = xml.SelectSingleNode("/root/info");
            if (infoNode != null)
            {
                xmlInfo.name = infoNode.Attributes["name"].Value;
                xmlInfo.course = infoNode.Attributes["course"].Value;
                xmlInfo.round = Convert.ToInt32(infoNode.Attributes["round"].Value);
                xmlInfo.week = Convert.ToInt32(infoNode.Attributes["week"].Value);
                xmlInfo.index = Convert.ToInt32(infoNode.Attributes["index"].Value);
                string languageTmp = infoNode.Attributes["language"].Value;
                if (languageTmp == "Cs") { xmlInfo.language = LANGUAGE.Cs; }
                else { xmlInfo.language = LANGUAGE.C; }     // !!!此段判断还未写完！
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 输出储存评测结果信息的xml文件
        /// </summary>
        /// <param name="resultAlt">欲新创建的xml文件路径</param>
        /// <param name="xmlInfo">一个Info类型的基本信息对象，用于写入到xml文件中</param>
        /// <param name="result">一个Result类型的结果信息对象，用于写入到xml文件中</param>
        /// <returns>一个布尔值，表示是否储存成功</returns>
        public static bool WriteResult(string resultAlt, Info xmlInfo, Result result)
        {
            XmlDocument xmlResult = new XmlDocument();
            xmlResult.AppendChild(xmlResult.CreateXmlDeclaration("1.0", "utf-8", null));
            xmlResult.AppendChild(xmlResult.CreateElement("root"));
            XmlNode root = xmlResult.SelectSingleNode("root");
            XmlElement infoNode = xmlResult.CreateElement("info");
            infoNode.SetAttribute("name", xmlInfo.name);
            infoNode.SetAttribute("course", xmlInfo.course);
            infoNode.SetAttribute("round", xmlInfo.round.ToString());
            infoNode.SetAttribute("week", xmlInfo.week.ToString());
            infoNode.SetAttribute("index", xmlInfo.index.ToString());
            string lang = null;
            switch (xmlInfo.language)
            {
                case LANGUAGE.C:
                    lang = "C";
                    break;
                case LANGUAGE.Cs:
                    lang = "Cs";
                    break;
                default:
                    break;
            }
            infoNode.SetAttribute("language", lang);
            root.AppendChild(infoNode);
            XmlElement resultList = xmlResult.CreateElement("result");
            resultList.SetAttribute("score", (100).ToString());
            string stateDescription = null;
            switch (result.State)
            {
                case COMPRES.AC:
                    stateDescription = "结果正确";
                    break;
                case COMPRES.WA:
                    stateDescription = "结果错误";
                    break;
                case COMPRES.CE:
                    stateDescription = "编译错误";
                    break;
            }
            resultList.SetAttribute("state", stateDescription);
            root.AppendChild(resultList);
            if (result.State != COMPRES.CE)
            {
                for (int i = 0; i < result.answerList.Count; i++)
                {
                    XmlElement point = xmlResult.CreateElement("node");
                    point.SetAttribute("id", (i + 1).ToString());
                    point.SetAttribute("passed", Convert.ToString(result.answerList[i]));
                    point.InnerText = string.Format("你的结果是{0}的", Bool2String(result.answerList[i]));
                    resultList.AppendChild(point);
                }
            }
            else
            {
                XmlElement errorList = xmlResult.CreateElement("error");
                root.AppendChild(errorList);
                for (int i = 0; i < result.errorList.Count; i++)
                {
                    XmlElement errorPoint = xmlResult.CreateElement("node");
                    errorPoint.SetAttribute("id", (i + 1).ToString());
                    errorPoint.InnerText = result.errorList[i];
                    errorList.AppendChild(errorPoint);
                }
            }
            xmlResult.Save(resultAlt);
            return true;
        }
    }

    /// <summary>
    /// 调用cmd命令的类
    /// 摘自：https://www.cnblogs.com/njl041x/p/3881550.html
    /// </summary>
    public class CmdHelper
    {
        private static string CmdPath = @"C:\Windows\System32\cmd.exe";

        /// <summary>
        /// 执行cmd命令
        /// <![CDATA[
        /// &:同时执行两个命令
        /// |:将上一个命令的输出,作为下一个命令的输入
        /// &&：当&&前的命令成功时,才执行&&后的命令
        /// ||：当||前的命令失败时,才执行||后的命令]]>
        /// </summary>
        /// <param name="cmd">命令行代码</param>
        /// <param name="output">接受输出的字符串</param>
        public static void RunCmd(string cmd, out string output)
        {
            cmd = cmd.Trim().TrimEnd('&') + " & exit";      // 执行exit命令，否则当调用ReadToEnd()方法时，会处于假死状态
            using (Process p = new Process())
            {
                p.StartInfo.FileName = CmdPath;
                p.StartInfo.UseShellExecute = false;        // 是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true;   // 接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = true;  // 由调用程序获取输出信息
                p.StartInfo.RedirectStandardError = true;   // 重定向标准错误输出
                p.StartInfo.CreateNoWindow = true;          // 不显示程序窗口
                try { p.Start(); }                          // 启动程序
                #region exception processing
                catch (Win32Exception e)
                {
                    Console.WriteLine("错误： Win32错误 - " + e.Message + "（如果看到此消息，可能说明评测平台存在问题，请联系评测平台管理员！）");
                    Console.ReadKey();
                    Environment.Exit(1);
                }
                catch (Exception)
                {
                    Console.WriteLine("错误： 发生未知类型错误，请联系评测平台管理员！");
                    Console.ReadKey();
                    Environment.Exit(2);
                }
                #endregion

                // 向cmd窗口写入命令
                p.StandardInput.WriteLine(cmd);
                p.StandardInput.AutoFlush = true;

                // 获取cmd窗口的输出信息
                output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();                            // 等待程序执行完退出进程
                p.Close();
            }
        }

        public static void RunFile(string fileName, string input, out string output)
        {
            using (Process p = new Process())
            {
                p.StartInfo.FileName = fileName;
                p.StartInfo.UseShellExecute = false;        // 是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true;   // 接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = true;  // 由调用程序获取输出信息
                p.StartInfo.RedirectStandardError = true;   // 重定向标准错误输出
                p.StartInfo.CreateNoWindow = true;          // 不显示程序窗口
                try { p.Start(); }                          // 启动程序
                #region exception processing
                catch (Win32Exception e)
                {
                    Console.WriteLine("错误： Win32错误 - " + e.Message + "（如果看到此消息，可能说明评测平台存在问题，请联系评测平台管理员！）");
                    Console.ReadKey();
                    Environment.Exit(1);
                }
                catch (Exception)
                {
                    Console.WriteLine("错误： 发生未知类型错误，请联系评测平台管理员！");
                    Console.ReadKey();
                    Environment.Exit(2);
                }
                #endregion

                // 向cmd窗口写入命令
                p.StandardInput.WriteLine(input);
                p.StandardInput.AutoFlush = true;

                // 获取cmd窗口的输出信息
                output = String.Empty;
                string strBuf = p.StandardOutput.ReadLine();
                while (strBuf != null)
                {
                    output = output + strBuf;
                    strBuf = p.StandardOutput.ReadLine();
                }
                p.WaitForExit();                            // 等待程序执行完退出进程
                p.Close();
            }
        }
    }
    
    /// <summary>
    /// 主程序类
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            string source = @"sources\test.c";
            LANGUAGE currentLanguage = ChooseLanguage(source);
            string xmlFile = @"answers\test_pascal.xml";
            Info info = new Info("admin", "C#程序设计", 3, 10, 1, currentLanguage);
            CheckFile cmp = new CheckFile(source, currentLanguage, xmlFile, info);
            cmp.Compile();
        }

        /// <summary>
        /// 一个辅助函数，用于根据文件名确定编程语言类型
        /// </summary>
        /// <param name="alt">源文件路径（文件名）</param>
        /// <returns>一个LANUGAGE类型的变量，表示源文件的语言类型</returns>
        static LANGUAGE ChooseLanguage(string alt)
        {
            if (alt.IndexOf(".cpp") >= 0)
                return LANGUAGE.Cpp;
            else if (alt.IndexOf(".cs") >= 0)
                return LANGUAGE.Cs;
            else if (alt.IndexOf(".c") >= 0)
                return LANGUAGE.C;
            else if (alt.IndexOf(".py") >= 0)
                return LANGUAGE.Python;
            else if (alt.IndexOf(".pas") >= 0)
                return LANGUAGE.Pascal;
            else
                return LANGUAGE.Unknown;
        }
    }

    /// <summary>
    /// 输入文件类，包含了源文件，类型，评测结果等
    /// </summary>
    public class CheckFile
    {
        private StreamReader sourceFile;    // 源代码文件对象
        public string Alt { set; get; }     // 源代码路径
        public LANGUAGE Language { set; get; }
        private Info basicInformation;      // 基本信息对象
        private Result outCome;             // 结果集对象
        private string runtimeName;         // 应用程序名称
        public string[] stdInput;           // 样例输入
        public string[] stdOutput;          // 样例输出
        public string xmlFileAlt;           // 样例路径

        /// <summary>
        /// 构造函数，用于生成一个信息完整的CheckFile对象
        /// </summary>
        /// <param name="alt">源文件路径</param>
        /// <param name="language">一个LANGUAGE变量，表示源文件编程语言</param>
        /// <param name="input">样例xml文件路径</param>
        /// <param name="initInfo">一个Info对象，表示基本信息</param>
        public CheckFile(string alt, LANGUAGE language, string input, Info initInfo)
        {
            // 关联源代码文件并做异常处理
            try { sourceFile = new StreamReader(alt); }
            #region exception processing
            catch (DirectoryNotFoundException)  // 路径不存在
            {
                Console.WriteLine("错误： 找不到指定的路径，请检查路径是否有误！");
                Console.ReadKey();
                Environment.Exit(1);
            }
            catch (FileNotFoundException)       // 源文件不存在
            {
                Console.WriteLine("错误： 找不到指定的源代码文件，请检查路径是否有误！");
                Console.ReadKey();
                Environment.Exit(1);
            }
            catch (Exception)
            {
                Console.WriteLine("错误： 发生未知类型错误，请联系评测平台管理员！");
                Console.ReadKey();
                Environment.Exit(2);
            }
            #endregion

            Alt = alt;
            Language = language;
            runtimeName = @"compile\test.exe";
            xmlFileAlt = input;
            basicInformation = initInfo;
        }

        /// <summary>
        /// 编译的主函数，根据当前语言选择执行哪一个子编译函数，再评测样例和输出结果
        /// </summary>
        public void Compile()
        {
            // 按不同编程语言选择不同的编译器
            switch (Language)
            {
                case LANGUAGE.C: Compile_gcc(); break;
                case LANGUAGE.Cpp: Compile_cl(); break;
                case LANGUAGE.Cs: Compile_csc();  break;
                case LANGUAGE.Python: Compile_py(); break;
                case LANGUAGE.Pascal: Compile_fpc(); break;
                case LANGUAGE.VB: break;
            }

            if (outCome.State != COMPRES.CE)                                            // 若编译未通过，则不进行评测，否则进行评测
                if (CheckAnswer() == false)
                    outCome.State = COMPRES.WA;                                         // 因Result对象初始化为AC，故只需一个判断

            XmlHelper.WriteResult(@"results\report.xml", basicInformation, outCome);    // 将结果汇总到report.xml文件中
        }
        #region compile section
        private void Compile_gcc()
        {
            string cmd = @"g++ -o compile\test.exe " + Alt;
            string output = String.Empty;
            CmdHelper.RunCmd(cmd, out output);
            outCome = new Result();
            Filter.filterMessage(LANGUAGE.C, output, outCome);
        }
        private void Compile_cl()
        {
            string cmd = @"cl /EHsc /o compile\test.exe " + Alt;
            string output = String.Empty;
            CmdHelper.RunCmd(cmd, out output);
            outCome = new Result();
            Filter.filterMessage(LANGUAGE.Cpp, output, outCome);
        }
        private void Compile_csc()
        {
            string cmd = @"csc /out:compile/test.exe " + Alt;
            string output = String.Empty;
            CmdHelper.RunCmd(cmd, out output);
            outCome = new Result();
            Filter.filterMessage(LANGUAGE.Cs, output, outCome);
            
        }
        private void Compile_py()
        {
            string cmd = @"python " + Alt;
            string output = String.Empty;
            CmdHelper.RunCmd(cmd, out output);
            Console.WriteLine(output);
        }
        private void Compile_fpc()
        {
            string cmd = @"fpc " + Alt + " -ocompile/test.exe";
            string output = String.Empty;
            CmdHelper.RunCmd(cmd, out output);
            outCome = new Result();
            Filter.filterMessage(LANGUAGE.Pascal, output, outCome);
        }
        #endregion

        /// <summary>
        /// 将编译好的应用程序按样例输入与样例输出进行比对，返回评测结果
        /// </summary>
        /// <returns>返回一个布尔值，表示是否为AC通过</returns>
        public bool CheckAnswer()
        {
            bool flag = true;           // 默认通过
            string[] inputCodeList;     // 储存样例输入
            XmlHelper.ReadNodes(xmlFileAlt, "input", out inputCodeList);    // 从xml文件中读取
            string[] outputCodeList;    // 储存样例输出
            XmlHelper.ReadNodes(xmlFileAlt, "output", out outputCodeList);  // 从xml文件中读取

            for (int i = 0; i < inputCodeList.Length; i++)  // 逐点评测
            {
                // 进行评测
                outCome.answerList.Add(CheckPoint(inputCodeList[i], outputCodeList[i]));
                if (outCome.answerList[i] == false) { flag = false; }       // 一样例点报错便不通过
            }
            return flag;
        }

        /// <summary>
        /// 对每一个样例数据点进行评测
        /// </summary>
        /// <param name="inputCode">样例输入</param>
        /// <param name="outputCode">样例输出</param>
        /// <returns>布尔值，表示程序对该样例是否正确</returns>
        private bool CheckPoint(string inputCode, string outputCode)
        {
            string answerCode;                  // 储存程序的输出
            CmdHelper.RunFile(runtimeName, inputCode, out answerCode);  // 在cmd环境下运行程序
            return answerCode == outputCode;    // 将程序的输出与样例做比对
        }
    }

    /// <summary>
    /// 结果类，反映本题目的评测结果
    /// </summary>
    public class Result
    {
        public COMPRES State { set; get; }  // 记录评测结果
        public List<string> errorList;      // 储存编译错误的线性表
        public List<bool> answerList;       // 储存评测结果的线性表
        public Result()
        {
            State = COMPRES.AC;
            errorList = new List<string>();
            answerList = new List<bool>();
        }
    }

    /// <summary>
    /// 过滤规则集类，存储不同编译器环境下的输出信息过滤规则
    /// </summary>
    public static class Filter
    {
        private static string[] keyWordList;
        /// <summary>
        /// 选择过滤器对应的编程语言，以决定过滤规则
        /// </summary>
        /// <param name="lng">一个LANGUAGE类型的变量，表示编程语言类型</param>
        private static void setLanugage(LANGUAGE lng)
        {
            switch (lng)
            {
                case LANGUAGE.C:
                    keyWordList = new string[] { "error:" };
                    break;
                case LANGUAGE.Cpp:
                    keyWordList = new string[] { "error C" };
                    break;
                case LANGUAGE.Cs:
                    keyWordList = new string[] { "error CS" };
                    break;
                case LANGUAGE.Pascal:
                    keyWordList = new string[] { "Fatal" };
                    break;
            }
        }
        /// <summary>
        /// 对输出信息进行过滤，查找是否有错误信息
        /// </summary>
        /// <param name="language">一个LANGUAGE类型的变量，表示编程语言的类型</param>
        /// <param name="rawOutput">字符串，代表原始输出的信息集（含换行符）</param>
        /// <param name="resultMsg">一个Result变量，用于接收判断结果</param>
        public static void filterMessage(LANGUAGE language, string rawOutput, Result resultMsg)
        {
            setLanugage(language);
            string[] strList = rawOutput.Split('\n');
            for (int i = 0; i < strList.Length; i++)
                for (int j = 0; j < keyWordList.Length; j++)
                    if (strList[i].IndexOf(keyWordList[j]) >= 0)
                    {
                        resultMsg.State = COMPRES.CE;
                        resultMsg.errorList.Add(strList[i]);
                    }
        }
    }

}
