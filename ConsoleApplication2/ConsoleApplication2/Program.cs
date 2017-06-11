using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace ConsoleApplication2
{
    class sentence
    {
        public string context = "預設";
        public string classification = "預設";
    }
    class Program
    {
        static void Main(string[] args)
        {
            if (File.Exists(@"..\..\pcp.txt") && File.Exists(@"..\..\ncp.txt") && File.Exists(@"..\..\positivenum.txt") && File.Exists(@"..\..\negativenum.txt"))
            {
                Console.WriteLine("偵測到存在正負機率字典與總字數");
            }
            else 
            {
                Console.WriteLine("偵測到不存在正負機率字典與總字數，開始建立");
                pre();
                Console.WriteLine("字典與字數建立完成");
            }


            var pcp = File
                .ReadAllLines(@"..\..\pcp.txt")
                .Select(l => l.Split('^'))//抓到key value
                .ToDictionary(a => a[0], a => double.Parse(a[1]));//轉成新字典


            var ncp = File
                .ReadAllLines(@"..\..\ncp.txt")
                .Select(l => l.Split('^'))//抓到key value
                .ToDictionary(a => a[0], a => double.Parse(a[1]));//轉成新字典


            string[] p = File.ReadAllLines(@"..\..\positivenum.txt");
            int positivenum = Int32.Parse(p[0]);

            string[] n = File.ReadAllLines(@"..\..\negativenum.txt");
            int negativenum = Int32.Parse(n[0]);

            while (true)
            {

                Console.WriteLine("請輸入句子(-1離開):");
                string s = Console.ReadLine();


                if (s != "-1")
                {
                    naivebayes(s, pcp, ncp, positivenum, negativenum);
                }
                else
                {
                    break;
                }
                        
            }

        }
        static void pre()//前置處理 pre()
        {

            //首先讀黨 抓出driverless_2class.txt 全部句子 把句子和句子所屬傾向放到物件陣列
            string[] lines = System.IO.File.ReadAllLines(@"..\..\driverless_2class.txt");
            sentence[] tt1 = new sentence[560]; int cou1 = 0; int status1 = 0;
            foreach (string line in lines) 
            {
                if (status1==1)
                {
                    tt1[cou1].context = line;
                    status1 = 0;
                    cou1++;
                }
                else if (line.StartsWith("<id>")) 
                {
                    tt1[cou1] = new sentence();
                }
                else if (line.StartsWith("<class>"))
                {
                    tt1[cou1].classification = line.Substring(7,8);
                    status1 = 1;
                }
            }

            //跑多執行序統計物件陣列
            int MAX_COUNT = cou1;//0-559

            //控制Thread數
            int WORKER_COUNT = 8;//我是8核心故設定8
            Thread[] workers = new Thread[WORKER_COUNT];
            int jobsCountPerWorker = MAX_COUNT / 8;

            var pdict1 = new Dictionary<string, int>();
            var pdict2 = new Dictionary<string, int>();
            var pdict3 = new Dictionary<string, int>();
            var pdict4 = new Dictionary<string, int>();
            var pdict5 = new Dictionary<string, int>();
            var pdict6 = new Dictionary<string, int>();
            var pdict7 = new Dictionary<string, int>();
            var pdict8 = new Dictionary<string, int>();
            var ndict1 = new Dictionary<string, int>();
            var ndict2 = new Dictionary<string, int>();
            var ndict3 = new Dictionary<string, int>();
            var ndict4 = new Dictionary<string, int>();
            var ndict5 = new Dictionary<string, int>();
            var ndict6 = new Dictionary<string, int>();
            var ndict7 = new Dictionary<string, int>();
            var ndict8 = new Dictionary<string, int>();

            for (int i = 0; i < WORKER_COUNT; i++)
            {
                //將全部工作切成WORKER_COUNT份，
                //分給WORKER_COUNT個Thread執行
                int st = jobsCountPerWorker * i;        
                int ed = jobsCountPerWorker * (i + 1);  
                if (ed > MAX_COUNT) ed = MAX_COUNT;     
                if (ed < MAX_COUNT && i == 7) ed = MAX_COUNT;

                workers[i] = new Thread(() =>
                {
                    Console.WriteLine("LOOP: {0:N0} - {1:N0}", st, ed);
                    for (int j = st; j < ed; j++)
                    {
                        

                        if (tt1[j].classification == "positive")
                        {
                            if (st / jobsCountPerWorker == 0)
                            {
                                a1(tt1[j].context, pdict1);
                            }
                            else if (st / jobsCountPerWorker == 1)
                            {
                                a1(tt1[j].context, pdict2);
                            }
                            else if (st / jobsCountPerWorker == 2)
                            {
                                a1(tt1[j].context, pdict3);
                            }
                            else if (st / jobsCountPerWorker == 3)
                            {
                                a1(tt1[j].context, pdict4);
                            }
                            else if (st / jobsCountPerWorker == 4)
                            {
                                a1(tt1[j].context, pdict5);
                            }
                            else if (st / jobsCountPerWorker == 5)
                            {
                                a1(tt1[j].context, pdict6);
                            }
                            else if (st / jobsCountPerWorker == 6)
                            {
                                a1(tt1[j].context, pdict7);
                            }
                            else if (st / jobsCountPerWorker == 7)
                            {
                                a1(tt1[j].context, pdict8);
                            }
                        }
                        else if (tt1[j].classification == "negative")
                        {
                            if (st / jobsCountPerWorker == 0)
                            {
                                a1(tt1[j].context, ndict1);
                            }
                            else if (st / jobsCountPerWorker == 1)
                            {
                                a1(tt1[j].context, ndict2);
                            }
                            else if (st / jobsCountPerWorker == 2)
                            {
                                a1(tt1[j].context, ndict3);
                            }
                            else if (st / jobsCountPerWorker == 3)
                            {
                                a1(tt1[j].context, ndict4);
                            }
                            else if (st / jobsCountPerWorker == 4)
                            {
                                a1(tt1[j].context, ndict5);
                            }
                            else if (st / jobsCountPerWorker == 5)
                            {
                                a1(tt1[j].context, ndict6);
                            }
                            else if (st / jobsCountPerWorker == 6)
                            {
                                a1(tt1[j].context, ndict7);
                            }
                            else if (st / jobsCountPerWorker == 7)
                            {
                                a1(tt1[j].context, ndict8);
                            }
                        }
                    }
                });
                workers[i].Start();
            }
            for (int i = 0; i < WORKER_COUNT; i++)
                workers[i].Join();


            //用正則抓物件陣列句子中的字詞與所屬類別 來建立兩個正負類別字典 分別存放某字詞在該類別時出現次數 例如negative["bad"]=85 得到count(w,c)
            //類別字典的長度即是該類別的總出現次數 得到count(c) 
            var negative = ndict1.Concat(ndict2).Concat(ndict3).Concat(ndict4).Concat(ndict5).Concat(ndict6).Concat(ndict7).Concat(ndict8)//串聯分工結果
               .GroupBy(d => d.Key)//依照key GroupBy合併value
               .OrderBy(d => d.Key)
               .ToDictionary(t => t.Key, t => t.Sum(d => d.Value));//轉換成新的字典
            //合併字詞出現次數工作成果

            var positive = pdict1.Concat(pdict2).Concat(pdict3).Concat(pdict4).Concat(pdict5).Concat(pdict6).Concat(pdict7).Concat(pdict8)//串聯分工結果
               .GroupBy(d => d.Key)//依照key GroupBy合併value
               .OrderBy(d => d.Key)
               .ToDictionary(t => t.Key, t => t.Sum(d => d.Value));//轉換成新的字典
            //合併字詞出現次數工作成果

            //抓字詞時也計數到總種類字典 而字典長度得到v
            var total = negative.Concat(positive)
                .GroupBy(d => d.Key)
                .OrderBy(d => d.Key)
                .ToDictionary(t => t.Key, t => t.Sum(d => d.Value));
            //合併字詞出現次數工作成果

            int negativenum=negative.Sum(t=>t.Value);
            int positivenum=positive.Sum(t=>t.Value);
            //int totalnum = total.Sum(t=>t.Value);
            
            var ncp = new Dictionary<string, double>();
            var pcp = new Dictionary<string, double>();
            



            //跑總種類字典每一個字詞 算每一個字詞在正或負的類別其出現機率(基本要有平滑化+1+v) 存放到兩個字典 正或負機率字典 例如 ncp["bad"]=0.258
            foreach(var temptemptemp in total)
            {
                int actualValue = 0;
                if (positive.TryGetValue(temptemptemp.Key, out actualValue))
                {
                    pcp[temptemptemp.Key] = Math.Log10(((double)(positive[temptemptemp.Key]) + (double)(0.1)) /( (double)(positivenum) + (double)(0.1) * (double)(total.Count)));
                }
                if (negative.TryGetValue(temptemptemp.Key, out actualValue))
                {
                    ncp[temptemptemp.Key] = Math.Log10(((double)(negative[temptemptemp.Key]) + (double)(0.1)) /( (double)(negativenum) + (double)(0.1) * (double)(total.Count)));
                }
            }
            foreach (var te in pcp)
            {
                WriteTextAsync(te.Key + "^" + te.Value,"pcp");
            }
            foreach (var te in ncp)
            {
                WriteTextAsync(te.Key + "^" + te.Value,"ncp");
            }
            StreamWriter outputFile = new StreamWriter(@"..\..\positivenum.txt", true);
            outputFile.WriteLine(positivenum);//寫正向總字數檔案
            outputFile.Close();
            StreamWriter outputFile2 = new StreamWriter(@"..\..\negativenum.txt", true);
            outputFile2.WriteLine(negativenum);//寫負向總字數檔案
            outputFile2.Close();



        }
        static void a1(string sen, Dictionary<string, int> dict) 
        {
            string pattern = @"[a-zA-Z]+([-]{1}[a-zA-Z]+)+"//複合字 例如a-b-c-d
                            +@"|\:\("//表情符號
                            +@"|\;\)"//表情符號
                            +@"|\:\)"//表情符號
                            +@"|->"//箭頭
                            +@"|http:\/\/[^ \t]+"//網址
                            +@"|[\d]{1,3}([,]{1}[\d]{3})+"//數字
                            +@"|[^\W]+"//字詞
                            //+@"|[%\"":!?;,()\[\]\-\&]"//標點符號
                            +@"|[.]{4}"//省略符號或沉默
                            +@"|[.]{3}"//省略符號或沉默
                            +@"|[.]{1}";//句點

            foreach (Match m in Regex.Matches(sen, pattern))
            {
                int actualValue = 0;
                if (!dict.TryGetValue(m.Value, out actualValue))
                {
                    dict[m.Value] = 1;
                }
                else
                {
                    dict[m.Value]++;
                }
            }
            
        }
        static async void WriteTextAsync(string text,string name)//非同步方式輸出 會暫存等待 解決資料同步問題
        {
            using (StreamWriter outputFile = new StreamWriter(@"..\..\"+name+".txt", true))
            {
                await outputFile.WriteAsync(text + "\n");
            }
        }
        static void naivebayes(string temp, Dictionary<string, double> pcp, Dictionary<string, double> ncp, int positivenum, int negativenum)//實際計算時 naivebayes()
        {
            string pattern = @"[a-zA-Z]+([-]{1}[a-zA-Z]+)+"//複合字 例如a-b-c-d
                            + @"|\:\("//表情符號
                            + @"|\;\)"//表情符號
                            + @"|\:\)"//表情符號
                            + @"|->"//箭頭
                            + @"|http:\/\/[^ \t]+"//網址
                            + @"|[\d]{1,3}([,]{1}[\d]{3})+"//數字
                            + @"|[^\W]+"//字詞
                            //+ @"|[%\"":!?;,()\[\]\-\&]"//標點符號
                            + @"|[.]{4}"//省略符號或沉默
                            + @"|[.]{3}"//省略符號或沉默
                            + @"|[.]{1}";//句點

            Console.WriteLine("\n{0,-100}", "positive");
            Console.WriteLine("{0,-100}{1,-100}\n\n", "word", "Log10 probability");
            double prolog1 = 0,prolog2 = 0;

            //算出該類別出現機率 即p(c)
            prolog1 += Math.Log10((double)(positivenum) / (double)(positivenum + negativenum));
            Console.WriteLine("{0,-100}{1,-100}", "Log10 probability of positive", prolog1);

            //把輸入句子斷字 每一個字詞查機率字典表得到該字詞於類別出現機率 即p(w|c)的連乘 並取log相加 最後比較哪一個類別總出現log比較大 那這個句子就是這個類別
            foreach (Match m in Regex.Matches(temp, pattern))
            {
                double rr = 0.0;
                double actualValue = 0.0;

                if (pcp.TryGetValue(m.Value, out actualValue))//1假使輸入句子中某字詞都出現在機率字典 直接查表算就好 因為公式裡每個變數都存在
                {
                    rr += pcp[m.Value];
                }
                else //2如果某字詞不存在 則重算公式 分子裡c(w,c)就是零 其餘查其他表帶入
                {
                    rr += Math.Log10((double)(0.1) / ((double)(positivenum) + (double)(0.1) * (double)(pcp.Count + ncp.Count)));
                }
                //沒有其他假設了 因為這個語言模型是天真貝氏 字詞出現機率彼此獨立不相關 不會有ngram那種前面字是否不存在字典的考慮
                prolog1 += rr;
                Console.WriteLine("{0,-100}{1,-100}", m.Value, rr);
            }
            Console.WriteLine("\n{0,-100}{1,-100}\n\n", "The sum of all probabilities", prolog1);
            Console.WriteLine("\n{0,-100}", "---------------------------------------------------");
            Console.WriteLine("\n{0,-100}", "negative");
            Console.WriteLine("{0,-100}{1,-100}\n\n", "word", "Log10 probability");

            prolog2 +=Math.Log10((double)(negativenum) / (double)(positivenum + negativenum));
            Console.WriteLine("{0,-100}{1,-100}", "Log10 probability of negative", prolog2);

            foreach (Match m in Regex.Matches(temp, pattern))
            {
                double rr2 = 0.0;
                double actualValue = 0.0;

                if (ncp.TryGetValue(m.Value, out actualValue))//1假使輸入句子中某字詞都出現在機率字典 直接查表算就好 因為公式裡每個變數都存在
                {
                    rr2 += ncp[m.Value];
                }
                else //2如果某字詞不存在 則公式裡分子c(w,c)就是零 其餘查表帶入
                {
                    rr2 += Math.Log10((double)(0.1) / ((double)(negativenum) + (double)(0.1) * (double)(pcp.Count + ncp.Count)));
                }
                //沒有其他假設了 因為這個語言模型是天真貝氏 字詞出現機率彼此獨立不相關 不會有ngram那種前面字是否不存在字典的考慮
                prolog2 += rr2;
                Console.WriteLine("{0,-100}{1,-100}", m.Value, rr2);
            }
            Console.WriteLine("\n{0,-100}{1,-100}\n\n", "The sum of all probabilities", prolog2);

            if(prolog1>prolog2)
            {
                Console.WriteLine("\n{0,-100}\n\n", "正向");
            }
            else if (prolog1 < prolog2)
            {
                Console.WriteLine("\n{0,-100}\n\n", "負向");
            }
            else
            {
                Console.WriteLine("\n{0,-100}\n\n", "不相上下");
            }

        }
    }
}
