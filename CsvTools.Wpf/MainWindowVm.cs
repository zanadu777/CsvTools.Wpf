using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CsvTools.Wpf.Wpf;
using System.Configuration;
using Newtonsoft.Json;

namespace CsvTools.Wpf
{
  public class MainWindowVm:INotifyPropertyChanged
  {
    private string csvPath;
    public event PropertyChangedEventHandler? PropertyChanged;

    public string CsvPath
    {
      get => csvPath;
      set => SetField(ref csvPath, value);
    }

    public ICommand ProcessCsvCommand { get; set; }

    public ObservableCollection<KeyValuePair<string, bool>>SelectedFields { get; set; } = new();

    public MainWindowVm()
    {
      ProcessCsvCommand = new DelegateCommand(OnProcessCsv);
      RefreshFieldsCommand = new DelegateCommand(OnRefreshFields);
     // CsvPath = @"D:\Dev\Temp\20241208-0800-gleif-goldencopy-lei2-golden-copy.csv";
    }

    private void OnRefreshFields(object obj)
    {
      
    }

    public ICommand RefreshFieldsCommand { get; set; }

    private void OnProcessCsv(object obj)
    {
      var csv = new QuotedCsvFile { File = new FileInfo(CsvPath) };
      var fields = csv.ReadFields();
      var output = new FileInfo(@"D:\Dev\Temp\Output\output.csv");
      csv.SelectedFields = LoadSelectedFieldsFromConfig();
      csv.Filter(output);

      var appSate = JsonConvert.SerializeObject(this);
      IsolatedStorageHelper.WriteTextToIsolatedStorage("appState", appSate);
    }


    private List<string> LoadSelectedFieldsFromConfig()
    {
      var selectedFieldsConfig = ConfigurationManager.AppSettings["SelectedFields"];
      var fields = new List<string>();

      if (!string.IsNullOrEmpty(selectedFieldsConfig))
      {
        fields = selectedFieldsConfig.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
      }

      return fields;
    }
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
      if (EqualityComparer<T>.Default.Equals(field, value)) return false;
      field = value;
      OnPropertyChanged(propertyName);
      return true;
    }
  }
}
