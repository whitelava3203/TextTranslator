using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Console = Colorful.Console;
using System.Drawing;
using System.Diagnostics;
using System.Net.Http;
using System.Collections;
using System.Web.Script.Serialization;
using System.Net;

namespace TextTranslator
{
    class Program
    {

        static void Main(string[] args)
        {

            if (args.Length == 0)
            {
            start:

                List<string> inputs1 = new List<string>();
                List<string> inputs2 = new List<string>();
                string input, input2;
                int value;

                Random rand = new Random();
                Console.Clear();
                Console.WriteAscii("TextTranslator", Color.FromArgb(rand.Next(1, 255), rand.Next(1, 255), rand.Next(1, 255)));
                Console.WriteLine("텍스트 번역 도우미 version 2",Color.White);
                Console.WriteLine("개발자 : 텍스트게임 갤러리",Color.Green);
                Console.WriteLine();
                Console.WriteLine("1.translator파일 자동 생성",Color.LightGray);
                Console.WriteLine("2.translator 파일 사용해 텍스트 교체",Color.LightGray);
                Console.WriteLine("0.도움말", Color.LightGray);
                Console.WriteLine("1번 기능 약간 불안정함", Color.Red);
                input = Console.ReadLine();
                if(int.TryParse(input, out value))
                {
                    switch(value)
                    {
                        case 1:
                            Console.WriteLine("모드 선택(1.기본, 2.고급, 0.도움말)",Color.LightGray);
                            input = Console.ReadLine();
                            if(int.TryParse(input,out value))
                            {
                                switch(value)
                                {
                                    case 0:
                                        Console.WriteLine("기본 모드 : 기본 설정된 조건으로 변환.",Color.LightGray);
                                        Console.WriteLine("고급 모드 : 조건 설정 가능.",Color.LightGray);
                                        Console.WriteLine("raito : 높으면 겹치는 텍스트 변환 성공할 가능성 높아지는 대신 정확도가 떨어짐. 기본값 1.0", Color.LightGray);
                                        Console.WriteLine("stopper : 변환될 텍스트의 길이가 설정된 값 이상으로 차이나면 문장 그대로 변환. raito와 비슷하게 작동함. 기본값 4", Color.LightGray);
                                        Console.WriteLine("skip : 문자열이 달라지는 부분 이후부터 그대로 변환. 기본값 false", Color.LightGray);
                                        Console.WriteLine("forceskip : 무조건 그대로 변환. 기본값 false", Color.LightGray);
                                        break;
                                    case 1:
                                        createti:
                                        Console.WriteLine("원본 폴더나 파일 경로 입력(이 창에 끌어도 됨)");
                                        input = Console.ReadLine();
                                        Console.WriteLine("번역된 폴더나 파일 경로 입력(이 창에 끌어도 됨)");
                                        input2 = Console.ReadLine();
                                        TranslatorInfo.CreateInfoAll(input.TrimError(),input2.TrimError());
                                        Console.WriteLine("완료",Color.LightSteelBlue);
                                        break;
                                    case 2:
                                        Console.WriteLine("raito stopper skip forceskip 순서대로 입력(기본값 : 1.0 4 false false)");
                                        input = Console.ReadLine();
                                        string[] inputs = input.Split(' ');
                                        if (inputs.Length == 4)
                                        {
                                            if (double.TryParse(inputs[0], out TranslatorInfo.staticArguments.comparer.raito) && int.TryParse(inputs[1], out TranslatorInfo.staticArguments.comparer.stopper) && bool.TryParse(inputs[2], out TranslatorInfo.staticArguments.comparer.skip) && bool.TryParse(inputs[3], out TranslatorInfo.staticArguments.comparer.forceskip))
                                            {
                                                goto createti;
                                            }
                                            else
                                            {
                                                Console.WriteLine("double int bool bool 에 맞게 입력", Color.Red);
                                            }

                                        }
                                        else
                                        {
                                            Console.WriteLine("4개의 값을 스페이스바로 띄어서 입력", Color.Red);
                                        }
                                        break;
                                    default:
                                        Console.WriteLine("1-2 이내의 값을 입력", Color.Red);
                                        break;
                                }
                            }
                            else
                            {
                                Console.WriteLine("정수 값을 입력", Color.Red);
                            }
                            
                            
                            break;
                        case 2:
                            Console.WriteLine("폴더나 파일 경로 입력(이 창에 끌어도 됨)");
                            input = Console.ReadLine();
                            TranslatorInfo.ConvertAll(input.TrimError());
                            break;
                        default:
                            Console.WriteLine("1-2 이내의 값을 입력", Color.Red);
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("정수 값을 입력",Color.Red);
                }
                Console.ReadLine();
                goto start;

            }
            else if (args.Length == 1)
            {
                TranslatorInfo.ConvertAll(args[0]);
                Console.ReadLine();
                return;
            }
            else
            {
                Random rand = new Random();
                Console.WriteAscii("TextTranslator", Color.White);
                Console.WriteLine("폴더를 \"하나만\" 끌어서 실행", Color.FromArgb(rand.Next(1, 255), rand.Next(1, 255), rand.Next(1, 255)));
                Console.ReadLine();
                return;
            }
        }

        
    }

    public class TranslatorInfo
    {
        public static Arguments staticArguments = new Arguments();
        public class Arguments
        {
            public string originPath;
            public string convertPath;

            public Comparer comparer = new Comparer();


            public class Comparer
            {
                public double raito = 1d;
                public int stopper = 4;
                public bool skip = false;
                public bool forceskip = false;
            }
        }

        private enum State
        {
            Normal,
            Translator
        }

        public static void ConvertAll(string path)
        {
            if (Directory.Exists(path))
            {
                DirectoryInfo di = new DirectoryInfo(path);
                staticArguments.originPath = di.FullName;
                staticArguments.convertPath = Path.Combine(new DirectoryInfo(staticArguments.originPath).Parent.FullName, new DirectoryInfo(staticArguments.originPath).Name + "_Translated");
                ConvertDirectory("");
            }
            else if (File.Exists(path))
            {
                FileInfo fi = new FileInfo(path);
                staticArguments.originPath = fi.FullName;
                staticArguments.convertPath = Path.Combine(fi.Directory.FullName, fi.Name + "_Translated");
                ConvertFile("");
            }
            else
            {
                Console.WriteLine(String.Format("오류 : 파일 또는 폴더 {0} 을/를 불러올수 없습니다.", path), Color.Red);
            }


        }

        public static void CreateInfoAll(string originpath, string convertpath)
        {
            if (Directory.Exists(originpath) && Directory.Exists(convertpath))
            {
                staticArguments.originPath = originpath;
                staticArguments.convertPath = convertpath;
                TranslatorInfo.CreateTranslatorInfoWithFolder("");
            }
            else if (File.Exists(originpath) && File.Exists(convertpath))
            {
                staticArguments.originPath = originpath;
                staticArguments.convertPath = convertpath;
                TranslatorInfo.CreateTranslatorInfoWithFile("");
            }
            else
            {
                Console.WriteLine(String.Format("오류 : 파일 또는 폴더 {0} 을/를 불러올수 없습니다.", originpath), Color.Red);
            }
        }

        static void ConvertDirectory(string basepath)
        {
            Directory.CreateDirectory(Path.Combine(staticArguments.convertPath, basepath));
            DirectoryInfo basedi = new DirectoryInfo(Path.Combine(staticArguments.originPath, basepath));
            basedi.GetFiles().ToList().ForEach((fi) => { ConvertFile(Path.Combine(basepath, fi.Name)); });
            basedi.GetDirectories().ToList().ForEach((di) => { ConvertDirectory(Path.Combine(basepath, di.Name)); });
        }
        static void ConvertFile(string basepath)
        {

            TranslatorInfo ti;
            if (basepath.EndsWith(".translate")) return;
            Console.WriteLine("로그 : 파싱 시작 : " + basepath);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            if (TranslatorInfo.TryGetTranslatorInfo(Path.Combine(staticArguments.originPath, basepath), out ti))
            {
                Console.WriteLine("로그 : 파싱 성공 : " + basepath);
                StringBuilder sb = new StringBuilder(File.ReadAllText(Path.Combine(staticArguments.originPath, basepath)));
                Translate(ref sb, ti);
                File.WriteAllText(Path.Combine(staticArguments.convertPath, basepath), sb.ToString());
            }
            else
            {
                Console.WriteLine("로그 : 무시됨 : " + basepath);
            }
            stopwatch.Stop();
            Console.WriteLine("로그 : 소요시간 : " + stopwatch.ElapsedMilliseconds / (double)1000 + "초", Color.LightCyan);
        }

        private static void Translate(ref StringBuilder text, TranslatorInfo ti)
        {
            foreach (ConvertString cs in ti.CsList)
            {
                string str = cs.replace;
                변환(ref str);
                text.Replace(cs.origin, str);
            }
        }






        public static void 변환(ref string str)
        {
            while (str.Contains("을/를"))
            {
                int index = str.IndexOf("을/를");
                if (받침확인(str[index - 1]))
                {
                    str = str.Remove(index, 3).Insert(index, "을");
                }
                else
                {
                    str = str.Remove(index, 3).Insert(index, "를");
                }
            }
            while (str.Contains("이/가"))
            {
                int index = str.IndexOf("이/가");
                if (받침확인(str[index - 1]))
                {
                    str = str.Remove(index, 3).Insert(index, "이");
                }
                else
                {
                    str = str.Remove(index, 3).Insert(index, "가");
                }
            }
        }
        private static bool 받침확인(char ch)
        {
            if (ch < 0xAC00 || ch > 0xD7A3)
            {
                return false;
            }
            return (ch - 0xAC00) % 28 > 0;
        }

        public static bool CreateTranslatorInfoWithFolder(string basepath)
        {
            try
            {
                DirectoryInfo origindi = new DirectoryInfo(Path.Combine(staticArguments.originPath, basepath));
                origindi.GetFiles().ToList().ForEach((fi) => { CreateTranslatorInfoWithFile(Path.Combine(basepath, fi.Name)); });
                origindi.GetDirectories().ToList().ForEach((di) => { CreateTranslatorInfoWithFolder(Path.Combine(basepath, di.Name)); });
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e, Color.Red);
                return false;
            }
            
        }
        public static bool CreateTranslatorInfoWithFile(string basepath)
        {
            return CreateTranslatorInfoWithFile(basepath,@"&&",@"=>",@"==");
        }
        public static bool CreateTranslatorInfoWithFile(string basepath, string codebase, string readorigin, string readconvert)
        {
            
            string originpath = Path.Combine(staticArguments.originPath,basepath);
            string convertpath = Path.Combine(staticArguments.convertPath,basepath);
            
            if(!File.Exists(originpath))
            {
                Console.WriteLine("로그 : 무시됨 : " + originpath);
                return false;
            }
            else if(!File.Exists(convertpath))
            {
                Console.WriteLine("로그 : 무시됨 : " + convertpath);
                return false;
            }
            Console.WriteLine("로그 : 비교 시작 : " + originpath);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();


            string[] origins = File.ReadAllLines(originpath);
            string[] converts = File.ReadAllLines(convertpath);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using CodeBase:"+codebase);
            sb.AppendLine("using ReadOrigin:"+readorigin);
            sb.AppendLine("using ReadConvert:"+readconvert);
            sb.AppendLine(codebase+"SetOriginLanguage:en");
            sb.AppendLine(codebase+"TranslateStart");

            if (TryCreateCsList(origins, converts, out List<ConvertString> cslist))
            {
                if (TryCheckCsList(ref cslist))
                {
                    cslist.ForEach((cs) =>
                    {
                        sb.AppendLine(cs.origin);
                        sb.AppendLine(readorigin);
                        sb.AppendLine(cs.replace);
                        sb.AppendLine(readconvert);
                    });
                }
            }
            else
            {
                stopwatch.Stop();
                return false;
            }

            sb.Remove(sb.Length-2,2);
            File.WriteAllText(originpath + ".translate",sb.ToString());
            stopwatch.Stop();
            Console.WriteLine("로그 : 소요시간 : " + stopwatch.ElapsedMilliseconds / (double)1000 + "초", Color.LightCyan);
            return true;
            //마지막 엔터 제거
        }
        private static bool TryCheckCsList(ref List<ConvertString> cslist)
        {
            if (cslist.Count == 0)
            {
                return false;
            }

            List<ConvertString> cslist2 = cslist;
            int[] arr = new int[cslist.Count+1];
            cslist.Add(cslist[0]);
            int 중복 = -1;
            int i = 0;
            foreach(ConvertString cs in cslist)
            {
                foreach(ConvertString cs2 in cslist)
                {
                    if(cs.Equals(cs2))
                    {
                        중복++;
                    }
                }
                arr[i++] = 중복;
                중복 = -1;
            }

            int temp, smallest;
            for (i = 0; i < cslist.Count - 1; i++)
            {
                smallest = i;
                for (int j = i + 1; j < cslist.Count; j++)
                {
                    if (arr[j] < arr[smallest])
                    {
                        smallest = j;
                    }
                }
                temp = arr[smallest];
                arr[smallest] = arr[i];
                arr[i] = temp;

                var temp2 = cslist[smallest];
                cslist[smallest] = cslist[i];
                cslist[i] = temp2;


            }
            cslist = cslist.Distinct().ToList();

            return true;
        }

        public static bool TryCreateCsList(string[] origins, string[] converts, out List<ConvertString> output)
        {
            output = null;
            if (origins.Length != converts.Length)
            {
                Console.WriteLine("오류 : 두 텍스트의 문장 수가 다름",Color.Red);
                return false;
            }


            List<ConvertString> cslist = new List<ConvertString>();
            for(int i=0;i<origins.Length; i++)
            {
                cslist.AddRange(GetDiff(origins[i], converts[i]));
            }
            output = cslist;
            return true;
        }
        

        private static Comparison<Tuple<int, int>> comp = (x, y) =>
        {
            double a = ((x.Item1 + x.Item2) * staticArguments.comparer.raito) + Math.Abs(x.Item1 - x.Item2);
            double b = ((y.Item1 + y.Item2) * staticArguments.comparer.raito) + Math.Abs(y.Item1 - y.Item2);
            if (a < b)
                return -1;
            else
                return 1;
        };

        public static List<ConvertString> GetDiff(string origin, string convert)
        {


            if (origin == convert)
            {
                return new List<ConvertString>();
                //두 문자열이 같으면 끝
            }

            List<ConvertString> cslist = new List<ConvertString>();

            if (staticArguments.comparer.forceskip)
            {
                cslist.Add(new ConvertString(origin,convert));
                return cslist;
                //forceskip 조건 true일때 통째로 리턴
            }


            int minlength = Math.Min(origin.Length, convert.Length);
            //문자열의 최소길이는 반복문 돌릴때 사용

            int startindex;
            
            for (startindex = 0; startindex < minlength && origin[startindex] == convert[startindex]; startindex++) ;
            //텍스트가 같은부분 넘김

            if (startindex == minlength)
            {
                cslist.Add(new ConvertString(origin, convert));
                return cslist;
            }
            //startindex 위치부터 시작

            List<Tuple<int, int>> list = new List<Tuple<int, int>>();

            for (int i = startindex; i < origin.Length; i++)
            {
                for (int j = startindex; j < convert.Length; j++)
                {
                    if (origin[i] == convert[j])
                    {
                        list.Add(new Tuple<int, int>(i, j));
                    }
                }
            }
            //변경점 이후에 같아지는 문자 위치들 찾기
            if(staticArguments.comparer.skip)
            {
                cslist.Add(new ConvertString(origin.Substring(startindex), convert.Substring(startindex)));
                return cslist;
                //skip 조건 true일때 통째로 리턴
            }


            if (list.Count == 0)
            {
                cslist.Add(new ConvertString(origin.Substring(startindex), convert.Substring(startindex)));
                return cslist;
                //같아지는 문자 위치가 없으면 남은거 통째로 리턴
            }
            else
            {



                Tuple<int, int> basevalue = list[0];
                for (int i = 0; i < list.Count; i++)
                {
                    int p = comp(basevalue, list[i]);
                    if (p > 0)
                    {
                        basevalue = list[i];
                    }
                }
                //같아지는 문자 위치들중 가장 적절한것 선정

                string o = origin.Substring(startindex, basevalue.Item1 - startindex);
                string c = convert.Substring(startindex, basevalue.Item2 - startindex);
                string oo = origin.Substring(basevalue.Item1);
                string cc = convert.Substring(basevalue.Item2);
                if (o.Length == 0 || c.Length == 0 || oo.Length == 0) 
                {
                    cslist.Add(new ConvertString(origin.Substring(startindex), convert.Substring(startindex)));
                    return cslist;
                    //이상하게 되면 남은거 통째로 리턴
                }
                if (o.Length * staticArguments.comparer.stopper > c.Length || c.Length * staticArguments.comparer.stopper > o.Length) 
                {
                    cslist.Add(new ConvertString(origin.Substring(startindex), convert.Substring(startindex)));
                    return cslist;
                    //이상하게 되면 남은거 통째로 리턴
                }
                
                cslist.Add(new ConvertString(o,c));
                cslist.AddRange(GetDiff(oo,cc));
                return cslist;
                //같아지는 문자 위치 이전까지 변경점으로 저장하고 이후의 문자열을 재귀함수 써서 다시 넣음
            }
        }


        


        public static bool TryGetTranslatorInfo(string path, out TranslatorInfo output)
        {

            output = null;
            path = path + ".translate";
            if (File.Exists(path))
            {
                string[] lines = File.ReadAllLines(path);
                if (lines != null)
                {
                    if (lines.Length > 0)
                    {
                        State state = State.Normal;
                        Queue<string> StringQ = new Queue<string>();
                        StringBuilder ssb = new StringBuilder();
                        TranslatorInfo ti = new TranslatorInfo();
                        try
                        {
                            foreach (string line in lines)
                            {
                                string[] augs;
                                if (state == State.Normal)
                                {
                                    if (TrySepreate(line, out augs))
                                    {
                                    Behind:
                                        augs[0] = augs[0].ToLower();
                                        if (CheckCodeBase(ref augs[0], ti))
                                        {

                                            if (augs[0].StartsWith("setoriginlanguage"))
                                            {
                                                string[] defs;
                                                if(TryArgDef(augs[0], out defs))
                                                {
                                                    ti.OriginLanguage = defs[1];
                                                }
                                                else
                                                {
                                                    Console.WriteLine("오류 : 문법 오류 : " + line, Color.Red);
                                                }
                                            }
                                            else if (augs[0].StartsWith("setpapagopkey"))
                                            {
                                                string[] defs;
                                                if (TryArgDef(augs[0], out defs))
                                                {
                                                    ti.PapagoPKey = defs[1];
                                                }
                                                else
                                                {
                                                    Console.WriteLine("오류 : 문법 오류 : " + line, Color.Red);
                                                }
                                            }
                                            else if (augs[0].StartsWith("setpapagoskey"))
                                            {
                                                string[] defs;
                                                if (TryArgDef(augs[0], out defs))
                                                {
                                                    ti.PapagoSKey = defs[1];
                                                }
                                                else
                                                {
                                                    Console.WriteLine("오류 : 문법 오류 : " + line, Color.Red);
                                                }
                                            }
                                            else
                                            {
                                                switch (augs[0])
                                                {
                                                    case "translatestart":
                                                        state = State.Translator;
                                                        break;
                                                    default:
                                                        Console.WriteLine("오류 : 알수 없는 명령어 : " + line, Color.Red);
                                                        break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (augs.Length == 1)
                                            {
                                                if (augs[0] == ti.TranslateStart_Name)
                                                {
                                                    augs[0] = ti.CodeBase_Name + "translatestart";
                                                    goto Behind;
                                                }
                                                else if (augs[0] == ti.TranslateEnd_Name)
                                                {
                                                    augs[0] = ti.CodeBase_Name + "translateend";
                                                    goto Behind;
                                                }
                                                else
                                                {
                                                    Console.WriteLine("오류 : 문장 파싱 불가능 : " + line, Color.Red);
                                                }
                                            }
                                            else if (augs.Length == 2)
                                            {
                                                if (augs[0] == "using")
                                                {
                                                    string[] defs;
                                                    if (TryArgDef(augs[1], out defs))
                                                    {
                                                        defs[0] = defs[0].ToLower();
                                                        switch (defs[0])
                                                        {
                                                            case "codebase":
                                                                ti.CodeBase_Name = defs[1];
                                                                break;
                                                            case "readorigin":
                                                                ti.ReadOrigin_Name = defs[1];
                                                                break;
                                                            case "readconvert":
                                                                ti.ReadConvert_Name = defs[1];
                                                                break;
                                                            case "translatestart":
                                                                ti.TranslateStart_Name = defs[1];
                                                                break;
                                                            case "translateend":
                                                                ti.TranslateEnd_Name = defs[1];
                                                                break;
                                                            default:
                                                                Console.WriteLine("오류 : using 이후 파싱 불가 : " + line, Color.Red);
                                                                break;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("오류 : using 식 파싱 불가 : " + line, Color.Red);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("오류 : 문장 파싱 불가능 : " + line, Color.Red);
                                    }
                                }
                                else if (state == State.Translator)
                                {
                                    string newline = line;
                                    if (line == ti.ReadOrigin_Name)
                                    {
                                        ssb.ToString().EndsWith(Environment.NewLine).IfThen(() => ssb.Remove(ssb.Length - 2, 2));
                                        StringQ.Enqueue(ssb.ToString());
                                        ssb.Clear();
                                    }
                                    else if (line == ti.ReadConvert_Name)
                                    {
                                        ssb.ToString().EndsWith(Environment.NewLine).IfThen(() => ssb.Remove(ssb.Length - 2, 2));
                                        foreach(string str in StringQ)
                                        {
                                            ti.CsList.Add(new ConvertString(str, ssb.ToString()));
                                        }
                                        ssb.Clear();
                                        StringQ.Clear();
                                    }
                                    else if(CheckCodeBase(ref newline, ti))
                                    {
                                        if(TrySepreate(newline, out augs))
                                        {
                                            if(augs.Length == 1)
                                            {
                                                Console.WriteLine("오류 : 번역중 명령어 인식 불가 1 : " + line, Color.Red);
                                            }
                                            else if(augs.Length == 2)
                                            {
                                                augs[0] = augs[0].ToLower();
                                                switch (augs[0])
                                                {
                                                    case "translator":
                                                        augs[1] = augs[1].ToLower();
                                                        string translated;
                                                        switch (augs[1])
                                                        {
                                                            case "google":
                                                                
                                                                translated =TranslateCode.GoogleTranslate(StringQ.Peek(),ti);
                                                                ssb.AppendLine(translated);
                                                                break;
                                                            case "papago":
                                                                translated = TranslateCode.PapagoTranslate(StringQ.Peek(),ti);
                                                                ssb.AppendLine(translated);
                                                                break;
                                                            default:
                                                                Console.WriteLine("오류 : 알수 없는 번역기 : " + line, Color.Red);
                                                                break;
                                                        }
                                                        break;
                                                    default:
                                                        Console.WriteLine("오류 : 번역중 명령어 인식 불가 2 : " + line, Color.Red);
                                                        break;
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("오류 : 번역중 명령어 인식 불가 3 : " + line, Color.Red);
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("오류 : 번역중 명령어 인식 불가 4 : "+line,Color.Red);
                                        }
                                    }
                                    else
                                    {
                                        ssb.AppendLine(line);
                                    }
                                }
                            }
                            output = ti;
                            return true;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e,Color.Red);
                            return false;
                        }
                    }
                }
            }
            return false;
        }
        private static bool CheckCodeBase(ref string str, TranslatorInfo ti)
        {
            if (ti.CodeBase_Name == null)
                return false;
            if(str.StartsWith(ti.CodeBase_Name))
            {
                str =  str.Substring(ti.CodeBase_Name.Length);
                return true;
            }
            return false;
        }
        private static bool TrySepreate(string str, out string[] output)
        {
            output = null;
            if (str.Contains(" "))
            {
                string[] imshi = str.Split(' ');
                if (imshi.Length == 1)
                {
                    return false;
                }
                else
                {
                    output = imshi;
                    return true;
                }
            }
            else
            {
                string[] imshi = new string[1];
                imshi[0] = str;
                output = imshi;
                return true;
            }
        }
        private static bool TryArgDef(string str, out string[] output)
        {
            output = null;
            if (str.Contains(":"))
            {
                string[] imshi = str.Split(':');
                if (imshi.Length != 2)
                {
                    return false;
                }
                else
                {
                    output = imshi;
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public TranslatorInfo Copy()
        {
            TranslatorInfo ti = new TranslatorInfo();
            ti.CodeBase_Name = this.CodeBase_Name;
            ti.ReadOrigin_Name = this.ReadOrigin_Name;
            ti.ReadConvert_Name = this.ReadConvert_Name;
            ti.TranslateStart_Name = this.TranslateStart_Name;
            ti.TranslateEnd_Name = this.TranslateEnd_Name;
            ti.Translator_Name = this.Translator_Name;
            ti.GetTranslator_Name = this.GetTranslator_Name;
            ti.OriginLanguage = this.OriginLanguage;
            ti.CsList = this.CsList.ToArray().ToList();
            return ti;
        }

        public string CodeBase_Name;
        public string ReadOrigin_Name;
        public string ReadConvert_Name;
        public string TranslateStart_Name;
        public string TranslateEnd_Name;
        public string Translator_Name;
        public string GetTranslator_Name;

        public string OriginLanguage;

        public string PapagoPKey = "";
        public string PapagoSKey = "";

        public List<ConvertString> CsList = new List<ConvertString>();
    }

    
    public struct ConvertString : IEquatable<ConvertString>
    {
        public ConvertString(string o, string r)
        {
            origin = o;
            replace = r;
        }
        public string origin;
        public string replace;


        public static bool operator >(ConvertString x, ConvertString b)
        {
            if (x.origin.Length > b.origin.Length)
                return true;
            return false;
        }
        public static bool operator <(ConvertString x, ConvertString b)
        {
            if (x.origin.Length < b.origin.Length)
                return true;
            return false;
        }
        public bool Equals(ConvertString cs)
        {
            if (Object.ReferenceEquals(cs, null)) return false;

            if (Object.ReferenceEquals(this, cs)) return true;


            if (cs.origin == this.origin && cs.replace == this.replace)
                return true;
            return false;
        }
    }
    public static class ExtensionMethods
    {
        public static string TrimError(this string str)
        {
            if(str.StartsWith("\""))
            {
                str = str.Substring(1,str.Length-1);
            }
            if(str.EndsWith("\""))
            {
                str = str.Substring(0, str.Length - 1);
            }
            return str;
        }

        public static void Loop(this Action act, int count)
        {
            for (int i = 0; i < count; i++) act();
        }
        public static bool IfThen(this bool b, Action ac)
        {
            if (b) ac();
            return b;
        }
        public static bool ElseIfThen(this bool b, bool bb, Action ac)
        {
            if (!b&&bb) ac();
            return bb;
        }
        public static void ElseThen(this bool b, Action ac)
        {
            if (!b) ac();
        }

        public static void WhileThen(this ref bool b, Action ac)
        {
            while (b)
                ac();
        }

        public static T GetHighest<T>(this List<T> list, Func<T, int> func)
        {
            (list.Count == 0).IfThen(()=>throw new Exception("현재 리스트의 길이가 0"));

            int i = int.MinValue;
            T output = default;
            list.ForEach((item) => (i < func(item)).IfThen(() => { i = func(item);output = item; }));

            return output;
        }

        public static T GetLowest<T>(this List<T> list, Func<T,int>func)
        {
            (list.Count == 0).IfThen(() => throw new Exception("현재 리스트의 길이가 0"));

            int i = int.MaxValue;
            T output = default;
            list.ForEach((item) => (i > func(item)).IfThen(() => { i = func(item); output = item; }));

            return output;
        }

        public class LambdaUpgrade<T>
        {
            T obj;
            bool Finish = false;
            public LambdaUpgrade()
            {
            }
            public LambdaUpgrade(bool b)
            {
                this.Finish = b;
            }
            public static implicit operator T(LambdaUpgrade<T> mdobj)
            {
                
                return mdobj.obj;
            }
            public static implicit operator LambdaUpgrade<T>(T obj)
            {
                LambdaUpgrade<T> mdobj = new LambdaUpgrade<T>();
                mdobj.obj = obj;
                return mdobj;
            }

        }

        
    }

    public static class TranslateCode
    {
        public static string GoogleTranslate(string input, TranslatorInfo ti)
        {
            // Set the language from/to in the url (or pass it into this function)
            string url = String.Format
            ("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}",
             ti, "ko", Uri.EscapeUriString(input));
            HttpClient httpClient = new HttpClient();
            string result = httpClient.GetStringAsync(url).Result;

            // Get all json data
            var jsonData = new JavaScriptSerializer().Deserialize<List<dynamic>>(result);

            // Extract just the first array element (This is the only data we are interested in)
            var translationItems = jsonData[0];

            // Translation Data
            string translation = "";

            // Loop through the collection extracting the translated objects
            foreach (object item in translationItems)
            {
                // Convert the item array to IEnumerable
                IEnumerable translationLineObject = item as IEnumerable;

                // Convert the IEnumerable translationLineObject to a IEnumerator
                IEnumerator translationLineString = translationLineObject.GetEnumerator();

                // Get first object in IEnumerator
                translationLineString.MoveNext();

                // Save its value (translated text)
                translation += string.Format(" {0}", Convert.ToString(translationLineString.Current));
            }

            // Remove first blank character
            if (translation.Length > 1) { translation = translation.Substring(1); };

            // Return translation
            return translation;
        }

        public static string PapagoTranslate(string input, TranslatorInfo ti)
        {
            string pkey = "1yLwacsfeoJpL3YxT1E4";
            string skey = "rvbK4KI4H_";
            (ti.PapagoPKey != "").IfThen(() => pkey = ti.PapagoPKey);
            (ti.PapagoSKey != "").IfThen(() => skey = ti.PapagoSKey);

            string url = "https://openapi.naver.com/v1/papago/n2mt";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add("X-Naver-Client-Id", pkey);
            request.Headers.Add("X-Naver-Client-Secret", skey);
            request.Method = "POST";
            byte[] byteDataParams = Encoding.UTF8.GetBytes(String.Format("source={0}&target=ko&text=",ti.OriginLanguage) + input);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteDataParams.Length;
            Stream st = request.GetRequestStream();
            st.Write(byteDataParams, 0, byteDataParams.Length);
            st.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            string result = reader.ReadToEnd();
            stream.Close();
            response.Close();
            reader.Close();

            int index = result.IndexOf("translatedText");
            return result.Substring(index+17,result.Length-21-index);
        }
    }
    
}
