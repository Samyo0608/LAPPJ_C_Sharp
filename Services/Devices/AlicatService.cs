using backend.Models.Devices;
using System.IO.Ports;
using System.Globalization;

namespace backend.Services.Devices
{
  public interface IAlicatService
  {
    bool Connect(string portName, string address = "A");
    void Disconnect();
    Task<AlicatReading> ReadAsync();
  }

  public class AlicatService : IAlicatService
  {
    private readonly SerialPort _serialPort;
    private readonly object _lock = new();
    // 依需求可以在建構子注入 SerialPort、Logger 等
    public AlicatService()
    {
      _serialPort = new SerialPort
      {
        BaudRate = 19200,
        Parity = Parity.None,
        DataBits = 8,
        StopBits = StopBits.One,
        Handshake = Handshake.None,
        ReadTimeout = 1000, // 這個是讀取超時時間
        WriteTimeout = 1000 // 這個是寫入超時時間
      };
    }

    private string _address = "A"; // 預設地址

    public bool Connect(string portName, string address)
    {
      // 如果有需要，可以在這裡處理地址
      // 例如設定 SerialPort 的 Address 屬性

      // 確保只有一個線程可以訪問 SerialPort
      if (string.IsNullOrEmpty(portName))
        throw new ArgumentException("Port name cannot be null or empty.", nameof(portName));

      lock (_lock)
      {
        if (_serialPort.IsOpen)
          _serialPort.Close();

        _serialPort.PortName = portName;
        _address = address; // 更新地址

        try
        {
          _serialPort.Open();
          return true;
        }
        catch (Exception ex)
        {
          // 可以記錄錯誤日誌
          Console.WriteLine($"無法連接到 {portName}: {ex.Message}");
          return false;
        }
      }
    }

    // void: 用於關閉連接
    public void Disconnect()
    {
      lock (_lock)
      {
        if (_serialPort.IsOpen)
        {
          try
          {
            _serialPort.Close();
          }
          catch (Exception ex)
          {
            // 可以記錄錯誤日誌
            Console.WriteLine($"關閉連接失敗: {ex.Message}");
          }
        }
      }
    }

    public string SendCommand(string command)
    {
      if (!_serialPort.IsOpen)
        throw new InvalidOperationException("尚未連線到 Alicat");

      lock (_lock)
      {
        // 清除輸入緩衝區，確保不會讀取到舊的資料
        _serialPort.DiscardInBuffer();
        // 寫入指令到 Alicat
        _serialPort.Write(command + "\r"); // Alicat 指令要用CR結尾
        try
        {
          string response = _serialPort.ReadLine();
          return response;
        }
        catch (TimeoutException)
        {
          throw new Exception("讀取 Alicat 超時");
        }
      }
    }

    public async Task<AlicatReading> ReadAsync()
    {
      if (!_serialPort.IsOpen)
        throw new InvalidOperationException("Alicat 未連線");

      string command = $"@{_address}?";
      string response;
      string[] parts;

      response = await Task.Run(() => SendCommand(command));
      // Alicat 回應格式通常像: "@A +14.700 +25.0 +1.230 +0.990 +1.500 N2"
      // 依據你的 Alicat 設定解析
      parts = response.Split(' ', StringSplitOptions.RemoveEmptyEntries);
      // 確保回應格式正確
      if (parts.Length < 7)
        throw new Exception($"Alicat 回應格式錯誤: {response}");

      return new AlicatReading
      {
        Pressure = double.Parse(parts[1], CultureInfo.InvariantCulture),
        Temperature = double.Parse(parts[2], CultureInfo.InvariantCulture),
        VolumetricFlow = double.Parse(parts[3], CultureInfo.InvariantCulture),
        MassFlow = double.Parse(parts[4], CultureInfo.InvariantCulture),
        Setpoint = double.Parse(parts[5], CultureInfo.InvariantCulture),
        GasType = parts[6],
        Unit = "slm"
      };
    }
  }
}