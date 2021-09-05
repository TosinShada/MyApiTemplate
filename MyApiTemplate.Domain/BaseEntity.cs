using System.ComponentModel.DataAnnotations;

namespace MyApiTemplate.Domain
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
