using UserManagementApp.Models;

public class UserViewModel
{
    public List<User> Users { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
}
