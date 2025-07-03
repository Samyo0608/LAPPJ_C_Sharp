using System.ComponentModel.DataAnnotations;

namespace backend.DTOs.Alicat
{
    public class ConnectAlicatRequest
    {
        public string DeviceType { get; set; } = "Alicat MFC";

        [Required(ErrorMessage = "Port不能空白。")]
        [StringLength(20, ErrorMessage = "Port長度不能超過20個字元。")]
        public string Port { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address不能空白。")]
        [StringLength(20, ErrorMessage = "Address長度不能超過20個字元。")]
        public string Address { get; set; } = string.Empty;
    }
}