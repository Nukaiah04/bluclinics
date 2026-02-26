using System.ComponentModel.DataAnnotations;

namespace BluClinicsApi.Entitys
{
    public class IDGenerator
    {
        [Key]
        public int Id{get;set;}
        public string? IdPrefix{get;set;}
        public string? DateValue{get;set;}
        public int LastNumber{get;set;}
        public DateTime UpdatedAt{get;set;} = DateTime.UtcNow;
    }
}