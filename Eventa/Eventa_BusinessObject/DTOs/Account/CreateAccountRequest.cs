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
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Số điện thoại phải có chính xác 10 số")]
        [RegularExpression(@"^(03|05|07|08|09)\d{8}$", ErrorMessage = "Số điện thoại phải là số điện thoại Viet Nam")]
        [DisplayName("Số điện thoại")]
        public required string PhoneNumber { get; set; }
        [DisplayName("Email")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        [DisplayName("Mật khẩu")]
        public required string Password { get; set; }

        [DisplayName("Tên")]
        public string? UserName { get; set; } 

        [DisplayName("Ảnh đại diện")]
        public IFormFile? AvatarImage { get; set; }
    }
}
