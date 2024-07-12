using Microsoft.Win32;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Renci.SshNet;
using System.IO;
using System.Windows;
using System.Windows.Input;
using WpfPrismDemo.EnumDto;

namespace WpfPrismDemo.ViewModels;

public class ScpViewModel : BindableBase
{
    #region Command

    public ICommand ChooseLocalPathCommand { get; set; }
    public ICommand SendCommand { get; set; }
    public ICommand ClickProductCommand { get; set; }
    public ICommand SaveConfigCommand { get; set; }
    public ICommand ExcuteCommand { get; set; }
    public ICommand TestCommand { get; set; }

    #endregion

    #region Property

    private string? localPath;

    public string? LocalPath
    {
        get { return localPath; }
        set { localPath = value; RaisePropertyChanged(); }
    }

    private string? remotePath;

    public string? RemotePath
    {
        get { return remotePath; }
        set { remotePath = value; RaisePropertyChanged(); }
    }

    private string? searchPattern;

    public string? SearchPattern
    {
        get { return searchPattern; }
        set { searchPattern = value; RaisePropertyChanged(); }
    }


    private string? remoteHost;

    public string? RemoteHost
    {
        get { return remoteHost; }
        set { remoteHost = value; RaisePropertyChanged(); }
    }


    private string? remoteUserName;

    public string? RemoteUserName
    {
        get { return remoteUserName; }
        set { remoteUserName = value; RaisePropertyChanged(); }
    }

    private string? remotePassword;

    public string? RemotePassword
    {
        get { return remotePassword; }
        set { remotePassword = value; RaisePropertyChanged(); }
    }

    private string? commandText;

    public string? CommandText
    {
        get { return commandText; }
        set { commandText = value; RaisePropertyChanged(); }
    }


    private ProductCode selectedProduct;

    public ProductCode SelectedProduct
    {
        get { return selectedProduct; }
        set { selectedProduct = value; RaisePropertyChanged(); }
    }


    #endregion

    string configFileName = "scpSetting.json";
    string configPath => Path.Combine(Environment.CurrentDirectory, configFileName);

    ScpConfig? selectedConfig;

    List<ScpConfig>? scpConfigs;

    public ScpViewModel()
    {
        ChooseLocalPathCommand = new DelegateCommand(ChooseLocalPath);
        SendCommand = new DelegateCommand(() => _ = Send());
        SaveConfigCommand = new DelegateCommand(() => SaveConfig());
        TestCommand = new DelegateCommand(Test);
        ExcuteCommand = new DelegateCommand(Excute);

        ClickProductCommand = new DelegateCommand<ProductCode?>((code) =>
        {
            if (code is not null)
            {
                SelectedProduct = code.Value;
                LoadConfig();
            }
        });

        LoadConfig();
    }

    private void Test()
    {
        var a = 0;
        _ = 4 / a;
    }

    /// <summary>
    /// 执行脚本
    /// </summary>
    private void Excute()
    {
        if (string.IsNullOrWhiteSpace(CommandText))
        {
            MessageBox.Show("请输入脚本");
            return;
        }

        if (string.IsNullOrWhiteSpace(RemoteHost) || string.IsNullOrWhiteSpace(RemoteUserName) || string.IsNullOrWhiteSpace(RemotePassword))
        {
            MessageBox.Show("请填好Linux配置");
            return;
        }

        using var client = new SshClient(RemoteHost, RemoteUserName, RemotePassword);

        client.Connect();

        using SshCommand cmd = client.RunCommand(CommandText);
        Console.WriteLine(cmd.Result);
    }

    private void ChooseLocalPath()
    {
        var folderDialog = new OpenFolderDialog
        {
            Title = "Select Folder",
            //InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
            AddToRecent = true,
        };
        if (folderDialog.ShowDialog() == true)
        {
            LocalPath = folderDialog.FolderName;
        }
    }

    private async void SaveConfig()
    {
        if (File.Exists(configPath))
        {
            var json = await File.ReadAllTextAsync(configPath);
            var configList = JsonConvert.DeserializeObject<List<ScpConfig>>(json);
            if (configList is null)
            {
                MessageBox.Show("读取配置文件失败");
                return;
            }

            var oldConfig = configList.FirstOrDefault(x => x.Product == SelectedProduct);
            if (oldConfig is not null)
            {
                oldConfig.LocalPath = LocalPath;
                oldConfig.RemotePath = RemotePath;
                oldConfig.RemoteHost = RemoteHost;
                oldConfig.RemotePassword = RemotePassword;
                oldConfig.RemoteUserName = RemoteUserName;
                oldConfig.SearchPattern = SearchPattern;
            }
            else
            {
                configList.Add(new ScpConfig
                {
                    LocalPath = LocalPath,
                    Product = SelectedProduct,
                    RemoteHost = RemoteHost,
                    RemoteUserName = RemoteUserName,
                    RemotePassword = RemotePassword,
                    RemotePath = RemotePath,
                    SearchPattern = SearchPattern,
                });
            }

            await File.WriteAllTextAsync(configPath, JsonConvert.SerializeObject(configList));
        }
        else
        {
            var list = new List<ScpConfig>();
            list.Add(new ScpConfig
            {
                LocalPath = LocalPath,
                Product = SelectedProduct,
                RemoteHost = RemoteHost,
                RemoteUserName = RemoteUserName,
                RemotePassword = RemotePassword,
                RemotePath = RemotePath,
                SearchPattern = SearchPattern,
            });
            await File.WriteAllTextAsync(configPath, JsonConvert.SerializeObject(list));
        }

        MessageBox.Show("保存成功");
    }

    /// <summary>
    /// scp上传文件
    /// </summary>
    /// <returns></returns>
    private async Task Send()
    {
        if (string.IsNullOrWhiteSpace(LocalPath) || !Directory.Exists(LocalPath))
        {
            MessageBox.Show("无效本地目录");
            return;
        }
        if (string.IsNullOrWhiteSpace(RemotePath))
        {
            MessageBox.Show("无效远程目录");
            return;
        }

        if (!RemotePath.EndsWith("/"))
        {
            RemotePath += "/";
        }

        List<string> files = new List<string>();
        if (string.IsNullOrWhiteSpace(SearchPattern))
        {
            files = Directory.GetFiles(LocalPath).ToList();
        }
        else
        {
            files = Directory.GetFiles(LocalPath, SearchPattern).ToList();
        }

        if (selectedConfig is null)
        {
            MessageBox.Show("没有配置文件，请先保存");
            return;
        }

        if (string.IsNullOrWhiteSpace(RemoteHost) || string.IsNullOrWhiteSpace(RemoteUserName) || string.IsNullOrWhiteSpace(RemotePassword))
        {
            MessageBox.Show("请填好Linux配置");
            return;
        }

        using var client = new SftpClient(RemoteHost, RemoteUserName, RemotePassword);
        await client.ConnectAsync(CancellationToken.None);

        //目前只发布api文件，都在单个文件夹内
        if (!client.Exists(RemotePath))
        {
            client.CreateDirectory(RemotePath);
        }

        files.ForEach(file =>
        {
            using (var inputStream = new FileStream(file, FileMode.Open))
            {
                //在Windows上操作，用Path.Combine生成的斜杆和Linux上反过来
                client.UploadFile(inputStream, RemotePath + Path.GetFileName(file));
            }
        });

        client.Disconnect();
        MessageBox.Show("上传成功");
    }

    /// <summary>
    /// 加载配置文件读取路径
    /// </summary>
    private void LoadConfig()
    {
        if (File.Exists(configPath))
        {
            var json = File.ReadAllText(configPath);

            scpConfigs = JsonConvert.DeserializeObject<List<ScpConfig>>(json);

            if (scpConfigs?.Any() is true)
            {
                selectedConfig = scpConfigs.FirstOrDefault(x => x.Product == SelectedProduct);
                LocalPath = selectedConfig?.LocalPath;
                RemotePath = selectedConfig?.RemotePath;
                RemoteHost = selectedConfig?.RemoteHost;
                RemoteUserName = selectedConfig?.RemoteUserName;
                RemotePassword = selectedConfig?.RemotePassword;
                SearchPattern = selectedConfig?.SearchPattern;

                CommandText = SelectedProduct switch
                {
                    ProductCode.Dol => "systemctl restart dolapi",
                    ProductCode.DolTest => "systemctl restart apitest",
                    ProductCode.陪诊 => "systemctl restart pzapi",
                    _ => string.Empty
                };
            }
        }
    }
}

internal class ScpConfig
{
    public ProductCode Product { get; set; }

    public string? LocalPath { get; set; }

    /// <summary>
    /// 筛选文件 a.*.dll
    /// </summary>
    public string? SearchPattern { get; set; }

    public string? RemotePath { get; set; }

    public string? RemoteUserName { get; set; } = "root";

    public string? RemoteHost { get; set; }
    public string? RemotePassword { get; set; }
}