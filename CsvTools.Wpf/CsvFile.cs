using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvTools.Wpf
{
  public class CsvFile
  {
    public FileInfo File { get; set; }

    public List<string> ReadFields()
    {
      if (File == null || !File.Exists)
      {
        throw new FileNotFoundException("The specified file does not exist.");
      }

      using (var reader = new StreamReader(File.FullName))
      {
        var line = reader.ReadLine();
        if (line != null)
        {
          return line.Split(',').ToList();
        }
      }

      return new List<string>();
    }
    
  }
}
