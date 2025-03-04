using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_BusinessObject.DTOs.Email
{
    public class CompleteRegistrationRequest
    {
        public string Token { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [StringLength(25, MinimumLength = 3, ErrorMessage = "Tên đăng nhập phải có từ 3 đến 25 ký tự")]
        [RegularExpression(@"^(?!_)(?!.*\s)[a-zA-Z0-9_]{3,25}(?<!_)$",
    ErrorMessage = "Tên đăng nhập chỉ được chứa chữ cái, số, dấu gạch dưới (_), không có khoảng trắng, không bắt đầu hoặc kết thúc bằng dấu gạch dưới.")]
        [DisplayName("Tên đăng nhập")]
        public required string UserName { get; set; }
        public string? FullName { get; set; }
        public string? ProfilePicture { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        [DisplayName("Mật khẩu")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]{8,}$",
             ErrorMessage = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public required string Password { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Số điện thoại phải có chính xác 10 số")]
        [RegularExpression(@"^(03|05|07|08|09)\d{8}$", ErrorMessage = "Số điện thoại phải là số điện thoại Viet Nam")]
        [DisplayName("Số điện thoại")]
        public string? PhoneNumber { get; set; }

    }
}
