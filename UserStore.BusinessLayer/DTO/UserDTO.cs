namespace UserStore.BusinessLayer.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public int? DepartmentId { get; set; }
        //public string Department { get; set; }
    }
}
