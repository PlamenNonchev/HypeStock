using System.ComponentModel.DataAnnotations;

namespace HypeStock.Data.Models
{
    public class EditorsPick
    {
        public int Id { get; set; }
        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; }
        
    }
}
