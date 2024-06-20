namespace RepositoryLayer.Entities
{
    public class UserEntity
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string HashedPassword { get; set; }
    }
}
