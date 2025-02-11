using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Eventa_BusinessObject.DTOs.Account
{
    public class CreateAccountRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        [DisplayName("Mật khẩu")]
        public string Password { get; set; } = string.Empty;

        [DisplayName("Tên")]
        public string? Name { get; set; }

        [DisplayName("Ngày sinh")]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Số điện thoại phải có chính xác 10 số")]
        [RegularExpression(@"^(03|05|07|08|09)\d{8}$", ErrorMessage = "Số điện thoại phải là số điện thoại Viet Nam")]
        [DisplayName("Số điện thoại")]
        public string PhoneNumber { get; set; } = string.Empty;

        [DisplayName("Địa chỉ")]
        public string? Address { get; set; } = string.Empty;

        [DisplayName("Ảnh đại diện")]
        public IFormFile? AvatarImage { get; set; }
    }
}
