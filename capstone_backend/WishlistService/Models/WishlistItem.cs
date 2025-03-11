using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WishlistService.Models
{
    public class WishlistItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public int Id { get; set; }
        public string UserEmail { get; set; }
        public string EventId { get; set; }
        public string EventName { get; set; }
        public string EventDate { get; set; }
        public string EventUrl { get; set; }
    }
}