using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.DAL.Models
{
    public class MeterReading
    {
        public int MeterReadingId { get; set; }

        public int ContractId { get; set; }
        public int ServiceId { get; set; }

        public int OldIndex { get; set; }
        public int NewIndex { get; set; }

        public DateTime ReadingDate { get; set; }

        public Contract Contract { get; set; }
        public Service Service { get; set; }
    }

}
