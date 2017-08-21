using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using CsvHelper;

namespace CSVJoiner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string inputPath = "input";
            var result = Directory.EnumerateFiles(inputPath, "*.csv");
            List<string> headers = new List<string>();
            List<Dictionary<string, string>> output = new List<Dictionary<string, string>>();
            // Figure out the headers
            foreach (var file in result)
            {
                StreamReader stream = new StreamReader(new FileStream(file, FileMode.Open));
                CsvReader parser = new CsvReader(stream);
                parser.Read();
                foreach (var h in parser.FieldHeaders)
                {
                    if (!headers.Contains(h))
                    {
                        headers.Add(h);
                    }
                }
                stream.Dispose();
            }

            foreach (var file in result)
            {
                Console.WriteLine($"Parsing {file}");
                CsvReader parser = new CsvReader(new StreamReader(new FileStream(file,FileMode.Open)));
                Dictionary<string, string> currentOutputRow;
                while(parser.Read())
                {
                    currentOutputRow = new Dictionary<string, string>();
                    foreach (var h in headers)
                    {
                        string tmpValue;
                        if (!parser.TryGetField(h, out tmpValue) || string.IsNullOrWhiteSpace(tmpValue))
                        {
                            tmpValue = "0";
                        }
                        currentOutputRow.Add(h, tmpValue);
                    }
                    output.Add(currentOutputRow);
                }
            }

            CsvWriter writer = new CsvWriter(new StreamWriter(new FileStream("output.csv", FileMode.Create)));

            foreach (var header in headers)
            {
                writer.WriteField(header);
            }

            writer.NextRecord();

            foreach (var row in output)
            {
                foreach (var column in row)
                {
                    writer.WriteField(column.Value);
                }
                writer.NextRecord();
            }

            Console.WriteLine("Complete -- Press Enter to close");
            Console.ReadLine();
        }
    }
}
