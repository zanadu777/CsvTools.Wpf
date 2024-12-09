using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvTools.Wpf
{
  internal class QuotedCsvFile
  {
    public FileInfo File { get; set; }
    public List<string> Fields { get; set; }
    public List<string> SelectedFields { get; set; }

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
          return ParseQuotedFields(line);
        }
      }

      return new List<string>();
    }

    private List<string> ParseQuotedFields(string line)
    {
      var fields = new List<string>();
      var currentField = new StringBuilder();
      bool inQuotes = false;

      for (int i = 0; i < line.Length; i++)
      {
        char c = line[i];

        if (c == '"')
        {
          inQuotes = !inQuotes;
        }
        else if (c == ',' && !inQuotes)
        {
          fields.Add(currentField.ToString());
          currentField.Clear();
        }
        else
        {
          currentField.Append(c);
        }
      }

      fields.Add(currentField.ToString());
      return fields;
    }

    public string Filter(FileInfo outputFile)
    {
      if (File == null || !File.Exists)
      {
        throw new FileNotFoundException("The specified input file does not exist.");
      }

      if (SelectedFields == null || !SelectedFields.Any())
      {
        throw new InvalidOperationException("No fields have been selected for filtering.");
      }

      using (var reader = new StreamReader(File.FullName))
      using (var writer = new StreamWriter(outputFile.FullName, false, Encoding.UTF8, 65536)) // Use a large buffer size
      {
        var headerLine = reader.ReadLine();
        if (headerLine == null)
        {
          throw new InvalidOperationException("The input file is empty.");
        }

        var headers = ParseQuotedFields(headerLine);
        var selectedIndexes = SelectedFields.Select(field => headers.IndexOf(field)).Where(index => index >= 0).ToList();
        selectedIndexes.Sort();

        if (!selectedIndexes.Any())
        {
          throw new InvalidOperationException("None of the selected fields were found in the input file.");
        }

        // Write the selected headers to the output file
        writer.WriteLine(string.Join(",", selectedIndexes.Select(index => headers[index])));

        string line;
        while ((line = reader.ReadLine()) != null)
        {
          var selectedFields = ExtractSelectedFields(line, selectedIndexes);
          writer.WriteLine(string.Join(",", selectedFields));
        }
      }

      return $"Filtered data has been written to {outputFile.FullName}";
    }

    private List<string> ExtractSelectedFields(string line, List<int> selectedIndexes)
    {
      var fields = new List<string>();
      var currentField = new StringBuilder();
      bool inQuotes = false;
      int fieldIndex = 0;
      int selectedIndex = 0;

      for (int i = 0; i < line.Length; i++)
      {
        char c = line[i];

        if (c == '"')
        {
          inQuotes = !inQuotes;
        }
        else if (c == ',' && !inQuotes)
        {
          if (selectedIndex < selectedIndexes.Count && fieldIndex == selectedIndexes[selectedIndex])
          {
            fields.Add(currentField.ToString());
            selectedIndex++;
            if (selectedIndex >= selectedIndexes.Count)
            {
              break;
            }
          }
          currentField.Clear();
          fieldIndex++;
        }
        else
        {
          currentField.Append(c);
        }
      }

      if (selectedIndex < selectedIndexes.Count && fieldIndex == selectedIndexes[selectedIndex])
      {
        fields.Add(currentField.ToString());
      }

      return fields;
    }
  }
}