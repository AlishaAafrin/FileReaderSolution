using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using CsvHelper;

namespace FileReaderSolution
{
    public class FileReaderService
    {
        private const string QueueName = "fileProcessingQueue";
        private const string FilePath = "C:\\TestFileReader\\TestFile.csv";//FileInput.txt"; 

        public async Task ReadAndPublish()
        {
           try
            {
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using var connection = await factory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();

                await channel.QueueDeclareAsync(queue: QueueName,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var messages = new List<string>();

                if (FilePath.EndsWith(".csv"))
                {
                    using var reader = new StreamReader(FilePath);
                    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                    while (csv.Read())
                    {
                        var line = csv.GetField(0); // Read first column
                        messages.Add(line);
                    }
                }
                else
                {
                    foreach (var line in File.ReadLines(FilePath))
                    {
                        messages.Add(line);
                    }
                }

                foreach (var message in messages)
                {
                    var body = Encoding.UTF8.GetBytes(message);
                    await channel.BasicPublishAsync(exchange: "",
                                         routingKey: QueueName,
                                         body: body);
                    Console.WriteLine($" [x] Sent {message}");
                }
            }
            catch(Exception ex)

            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
