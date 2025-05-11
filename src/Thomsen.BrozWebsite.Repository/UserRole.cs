namespace Thomsen.BrozWebsite.Repository;

public enum UserRoleEnum {
    None = 0,
    Editor = 10,
    Admin = 100
}

public record UserRole {
    public int Id { get; set; }
    public string Email { get; set; } = "";
    public UserRoleEnum Role { get; set; } = UserRoleEnum.None;
}
