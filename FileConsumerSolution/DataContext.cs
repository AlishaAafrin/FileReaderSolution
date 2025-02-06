using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileConsumerSolution
{
    public class DataContext : DbContext
    {
        public DbSet<FileData> FileRecords { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
    }

    public class FileData
    {
        public int Id { get; set; }
        public string Content { get; set; }
    }
}

