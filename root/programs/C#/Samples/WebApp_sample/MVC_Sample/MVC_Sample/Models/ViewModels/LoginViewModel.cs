
using System.ComponentModel.DataAnnotations;

namespace MVC_Sample.Models.ViewModels
{
    public class LoginViewModel
    {
        /// <summary>
        /// UserName
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        /// <summary>
        /// PWDS
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "passwowd")]
        public string Passwowd { get; set; }
    }
}