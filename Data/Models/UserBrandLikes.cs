namespace HypeStock.Data.Models
{
    public class UserBrandLikes
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public int BrandId { get; set; }
        public Brand Brand { get; set; }
        public string Type { get; set; }
    }
}
