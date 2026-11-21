
namespace DazzleJewels.Models
{
    public class PageView
    {
        public int Id { get; set; }

        public string PagePath { get; set; } = string.Empty;

        public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
    }
}
