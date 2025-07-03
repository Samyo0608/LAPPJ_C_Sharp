using System.ComponentModel.DataAnnotations;

namespace backend.Models.Entities
{
  public class MeasurementData
  {
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }  = DateTime.UtcNow;

    public double Voltage { get; set; }

    public double MainGasFlow { get; set; }

    [StringLength(10)]
    public string MainGasFlowUnit { get; set; } = "";

    public double CarrierGasFlow { get; set; }

    [StringLength(10)]
    public string CarrierGasFlowUnit { get; set; } = "";

    [StringLength(20)]
    public string CarrierGasType { get; set; } = "";

    public double LaserPowerPercentage { get; set; }
    public bool IsLaserOn { get; set; } = false;
    public double HeaterTemperature { get; set; }

    public bool IsUltrasonicOn { get; set; } = false;
  }
}
