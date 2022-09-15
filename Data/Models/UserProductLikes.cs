namespace HypeStock.Data.Models
{
    public class UserProductLikes
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string Type { get; set; }
    }
}
