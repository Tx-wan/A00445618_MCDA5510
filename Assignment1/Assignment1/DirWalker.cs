using System;
using System.IO;
using System.Diagnostics;

namespace Assignment1
{


    public class DirWalker
    {
        private int[] lines;
        private int goodline = 0;
        private int badline = 0;

        public void walk(String path)
        {
            string[] list = Directory.GetDirectories(path);

            SimpleCSVParser scsvp = new SimpleCSVParser();

            String filename;
            String extension;

            //the output csv's path
            String outfilePath = @"C:\Users\tt_mo\Documents\@SMU\5510 Software Developement in Business Environment\GitRepos\A00445618_MCDA5510\Assignment1\Output\Output.csv";

            if (list == null) return;

            foreach (string dirpath in list)
               // foreach (String f : list)
            {
                if (Directory.Exists(dirpath))
                {
                    walk(dirpath);
               //     Console.WriteLine("Dir:" + dirpath);
                }
            }
            string[] fileList = Directory.GetFiles(path);
            
            //create a new csv output file
            createNewCSV(outfilePath);

            foreach (string filepath in fileList)
            {   
          //      Console.WriteLine("File:" + filepath);

                filename = Path.GetFileName(filepath);
                extension = Path.GetExtension(filepath);

                if (extension == ".csv")
                {
                    lines =scsvp.parse(outfilePath, filepath);
                    goodline += lines[0];
                    badline += lines[1];
                  //  blankline += lines[2];
                }
            }

        }

        public int getGoodLine()
        {
            return goodline;
        }

        public int getBadLine()
        {
            return badline;
        }


        public static void createNewCSV(String path)
        {
            if (!File.Exists(path))
            {
                //check if the cvs path is valid
                Exceptions outputChecker = new Exceptions();

                StreamWriter sw = outputChecker.OpenStream(path);

                String title = "First Name,Last Name,Street Number,Street,City,Province,Postal Code,Country,Phone Number,email Adress,data";

                sw.WriteLine(title);

                sw.Flush();
                sw.Close();
                //fs.Close();
            }
        }

        public static void Main(String[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Console.WriteLine("working...");

            DirWalker fw = new DirWalker();
            /*            String dir = AppDomain.CurrentDomain.BaseDirectory;


                        Console.WriteLine(dir);*/
            //fw.walk(@"../../../../../");
            //fw.walk(@"C:\Users\tt_mo\Documents\@SMU\5510 Software Developement in Business Environment\GitRepos\assignment\MCDA5510_Assignments\Assignment1\test");

            //the path of directories the sample csv files
            var path = @"C:\Users\tt_mo\Documents\@SMU\5510 Software Developement in Business Environment\GitRepos\A00445618_MCDA5510\Assignment1\Sample Data\Sample Data";

            //the path of log file
            var logpath = @"C:\Users\tt_mo\Documents\@SMU\5510 Software Developement in Business Environment\GitRepos\A00445618_MCDA5510\Assignment1\logs\log.txt";

            fw.walk(path);

            sw.Stop();

            TimeSpan duration = sw.Elapsed;

            //write logs
            File.AppendAllText(logpath, "Total excution time: " + duration.ToString(@"hh\:mm\:ss") + "\n");
            File.AppendAllText(logpath, "Total number of valid rows:" + fw.getGoodLine()+"\n");
            File.AppendAllText(logpath, "Total number of skipped rows:" + fw.getBadLine()+"\n");
            

            Console.WriteLine("good line" + fw.getGoodLine());
            Console.WriteLine("bad line" + fw.getBadLine());
           // Console.WriteLine("blank line" + fw.getblank());
            Console.WriteLine("Total excution time: " +duration.ToString(@"hh\:mm\:ss"));
        }

    }
}
