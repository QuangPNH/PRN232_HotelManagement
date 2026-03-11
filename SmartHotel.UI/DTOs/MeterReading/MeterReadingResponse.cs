using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHotel.UI.DTOs.MeterReading
{
    public class MeterReadingResponse
    {
        public int MeterReadingId { get; set; }

        public int ContractId { get; set; }
        public string RoomNumber { get; set; } 
        public string ServiceName { get; set; } 

        public int OldIndex { get; set; }
        public int NewIndex { get; set; }
        public int Consumption => NewIndex - OldIndex; 

        public DateTime ReadingDate { get; set; }
    }
}
