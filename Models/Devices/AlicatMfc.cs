using System.ComponentModel.DataAnnotations;

namespace backend.Models.Devices
{
  public class AlicatReading
  { 
        public int Id { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public double Pressure { get; set; }
        public double Temperature { get; set; }
        public double VolumetricFlow { get; set; }
        public double MassFlow { get; set; }
        public double Setpoint { get; set; }

        public string GasType { get; set; } = "";
        public string Unit { get; set; } = "";
  }
}