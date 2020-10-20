using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_UI.Models
{
    public class Author
    {
        public int Id { get; set; }
        [Required]
        [Display(Name ="First Name")]
        public string Firstname { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string Lastname { get; set; }
        [Required]
        [Display(Name = "Biyography")]
        [StringLength(250)]
        public string Bio { get; set; }
        public virtual IList<Book> Books { get; set; }
    }
}
