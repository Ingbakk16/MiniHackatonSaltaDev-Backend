namespace AnimalesPerdidos.Dtos.Usuario
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = null!;
        public bool Success { get; set; }
        public IEnumerable<string>? Errors { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
