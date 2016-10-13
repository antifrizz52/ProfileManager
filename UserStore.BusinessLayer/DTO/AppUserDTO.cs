namespace UserStore.BusinessLayer.DTO
{
    public class AppUserDTO
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
    }

    public enum Role
    {
        Admin = 1,
        User = 2
    }
}
