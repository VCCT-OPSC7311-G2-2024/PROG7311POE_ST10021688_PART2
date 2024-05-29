using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PROG7311POE.Models
{
    /// <summary>
    /// This is a yum-yum or farm stuffs, sold by the farmer.
    /// </summary>
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductID { get; set; }
        public int FarmerID { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public DateTime ProductionDate { get; set; }
    }
}
// 💻🌸✨💖 --<< End of File >>-- 💖✨🌸💻