namespace ProgressService.Models.Dto
{
    public class MeResponse
    {
        public int AdminId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
    }
}