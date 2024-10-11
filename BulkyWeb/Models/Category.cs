using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BulkyWeb.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        
        
        [Required]
        [DisplayName("Category Name")] //data annotation to show this name on client side UI
        [MaxLength(40)] //input validation name should be max of 40 characters
        public string Name { get; set; }
        
        
        [DisplayName("Display Order")]
        [Range(1,100,ErrorMessage ="Display order must be between 1-100")] //input validation min 1 max 100
        public int DisplayOrder { get; set; }


    }
}
