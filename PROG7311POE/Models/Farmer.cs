using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PROG7311POE.Models
{
    /// <summary>
    /// This guys sells all the farm products/yum-yums.
    /// </summary>
    [Serializable]
    public class Farmer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FarmerID { get; set; }
        public int UserID { get; set; }
        public string FarmName { get; set; }
        public string Location { get; set; }
    }
}
// 💻🌸✨💖 --<< End of File >>-- 💖✨🌸💻