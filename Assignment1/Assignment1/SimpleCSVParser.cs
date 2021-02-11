using System;
using System.IO;
//using Microsoft.VisualBasic.FileIO;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
using System.Text.RegularExpressions;

namespace Assignment1
{
    public class SimpleCSVParser
    {


 //       public static void Main(String[] args)
 //       {
 //           SimpleCSVParser parser = new SimpleCSVParser();
 //           parser.parse(@"/Users/dpenny/Projects/Assignment1/Assignment1/sampleFile.csv");
 //       }

        /*
         * parse csv file and validate the csv rows and write the valid ones into output files
         * @outFilePath, the path of the output csv file
         * @filepath the path of csv to be read
         */
        public int[] parse(String outFilePath, String filepath)
        {

            String newRecord;
            

            int[] lines = { 0, 0 };

            StreamWriter csvsw = new StreamWriter(outFilePath, true);

            try 
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    PrepareHeaderForMatch = (header, index) => header.Replace(" ", string.Empty)
                };

                using(var reader = new StreamReader(filepath))
                using(var csv = new CsvReader(reader,config)) 
                {

                    //      csv.Configuration.HasHeaderRecord = true;

                    while(csv.Read())
                    {
                        //get the data from its directories
                        var record = csv.GetRecord<CustomerCSV>();

                        var dayFolder = Path.GetDirectoryName(filepath);
                        var monthFolder = Path.GetDirectoryName(dayFolder);
                        var yearFolder = Path.GetDirectoryName(monthFolder);

                        var dayInfo = Path.GetFileName(dayFolder);
                        var monthInfo = Path.GetFileName(monthFolder);
                        var yearInfo = Path.GetFileName(yearFolder);

                        String dataInfo = yearInfo + "-" + monthInfo + "-" + dayInfo;
                        
                        //get raw data from csv file. Because some rows contain quote which cannot be handled by CSVHelper. 
                        //These lines need to be processed speratively
                        var rawData = csv.Context.Parser.RawRecord;

                        //check if there is quote in this row
                        if (rawData.IndexOf("\"") >0)
                        {

                            String[] records = new String[10];

                            //check null field and the total number of fields. 
                            //because there are 10 fields in csv. the number is smaller than 10 means something is missing 
                            if (!checkNullValue(rawData) && splitQuoteDataLength(rawData) == 10)
                            {
                                newRecord = normalizeRawData(rawData);

                                lines[0]++;

                                csvsw.WriteLine(newRecord + "," + dataInfo );

                                //this method costs too much cache. too slow, cannot be used for program against large set of data
                                // File.AppendAllText(outfilePath, newRecord + "," + dataInfo +"\n");
                            }
                            else
                            {
                                lines[1]++;
                            }
                        } else
                        {
                            //check null field
                            if (!checkNullValue(record))
                            {
                                newRecord = record.FirstName + "," + record.LastName + "," + record.StreetNumber + "," + record.Street
                                    + "," + record.City + "," + record.Province + "," + record.PostalCode + "," + record.Country + ","
                                    + record.PhoneNumber + "," + record.emailAddress ;

                                lines[0]++;

                                csvsw.WriteLine(newRecord + "," + dataInfo );

                                //   File.AppendAllText(outfilePath, newRecord + "\n");


                            }
                            else
                            {
                                lines[1]++;
                            }

                        }


                    }
                }

/*                using (TextFieldParser parser = new TextFieldParser(fileName))
                {

                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    while (!parser.EndOfData)
                    {
                        //Process row
                    string[] fields = parser.ReadFields();
                    foreach (string field in fields)
                    {
                        Console.WriteLine(field);
                     }
                    }
                }*/



            }catch(IOException ioe){
              //      Console.WriteLine(ioe.StackTrace);
             } catch(Exception e){
                //    Console.WriteLine("exception" + e.StackTrace);
             }

            csvsw.Flush();
            csvsw.Close();

            return lines;
        }

        /*
         * check if there is null value field in this csv row
         * @customerCSV the format of csv
         * Return true: there is null value field; false there is no null value field
         */
        private Boolean checkNullValue(CustomerCSV customerCSV)
        {
            Boolean result = false;

            

            if (customerCSV.FirstName == null || customerCSV.FirstName == "")
                result = true;

            if (customerCSV.LastName == null || customerCSV.LastName == "")
                result = true;

            if (customerCSV.StreetNumber == null || customerCSV.StreetNumber == "")
                result = true;

            if (customerCSV.Street == null || customerCSV.Street == "")
                result = true;

            if (customerCSV.City == null || customerCSV.City == "")
                result = true;

            if (customerCSV.Province == null || customerCSV.Province == "")
                result = true;

            if (customerCSV.PostalCode == null || customerCSV.PostalCode == "")
                result = true;

            if (customerCSV.Country == null || customerCSV.Country == "")
                result = true;

            if (customerCSV.PhoneNumber == null || customerCSV.PhoneNumber == "")
                result = true;

            if (customerCSV.emailAddress == null || customerCSV.emailAddress == "" || ",".IndexOf(customerCSV.emailAddress) >=0 )
                result = true;

            return result;

        }

        /*
        * check if there is null value field in this csv row
        * @rawData raw data from csv file
        * Return true: there is null value field; false there is no null value field
        */
        private Boolean checkNullValue(String rawData)
        {
            Boolean result = false;

            String[] quoteSub = rawData.Split("\"");

            String[] commSub = rawData.Split(",");

            for(int i=0;i<quoteSub.Length; i++)
            {
                if(quoteSub[i]== "" || quoteSub[i]==null)
                {
                    i = quoteSub.Length + 1;
                    result = true;
                }
            }

            if (!result)
            {
                for (int i = 0; i < commSub.Length; i++)
                {
                    if (commSub[i] == "" || commSub[i] == null)
                    {
                        i = commSub.Length + 1;
                        result = true;
                    }
                }
            }
            
            return result;

        }

        /*
        * calculate the number of fields of csv row
        * @rawData raw data from csv file
        * return the number of fields of csv row
        */
        private int splitQuoteDataLength(String data)
        {
            String[] result;

            result = data.Split(",");

            int length = result.Length;
            int mark = -1;

            for (int i = 0; i < result.Length; i++)
            {
                if(mark==-1)
                {
                    if(result[i].StartsWith("\"") && !result[i].EndsWith("\""))
                    {
                        mark = i;
                    }
                } else
                {
                    if (result[i].EndsWith("\""))
                    {
                        length -= i - mark;
                        mark = 1;
                    }
                }
            }


            return length;
        }

        /*
        * delete the enter char at the end of raw data
        * @rawData raw data from csv file
        * return the data without enter char
        */
        private String normalizeRawData(String rawData)
        {
            String result;

            if (rawData.EndsWith("\n"))
            {
                result = rawData.Substring(0,rawData.Length-1);
            } else
            {
                result = rawData;
            }

            return result;
        }

    }
}
