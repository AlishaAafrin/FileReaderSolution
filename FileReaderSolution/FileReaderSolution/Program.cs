using FileReaderSolution;
using System;
using System.IO;
using System.Text;

class Program
{
    static async Task Main(string[] args)
    {
        var fileReader =  new FileReaderService();
        await fileReader.ReadAndPublish();
        Console.WriteLine("File processing completed.");
    }
}
